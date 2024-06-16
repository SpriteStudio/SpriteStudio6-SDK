#include <zlib.h>
#include <contrib/minizip/zip.h>
#include <nlohmann/json.hpp>

#include <iostream>
#include <fstream>
#include <iomanip>
#include <string>

#include "sspkg.h"
#include "sscharconverter.h"
#include "SSException.h"
#include "SsPlayerConverter.h"
#include "utils.h"

using json = nlohmann::json;

using sscc = spritestudio6::SsCharConverter;

int CreateZipFile(std::string zippath ,  std::vector<std::string> paths , std::string remove_path )
{
    zipFile zf = zipOpen( zippath.c_str() , APPEND_STATUS_CREATE);
    if (zf == NULL)
        return 1;

    bool _return = true;
    for (size_t i = 0; i < paths.size(); i++)
    {
        auto path = paths[i];
        std::fstream file(path.c_str(), std::ios::binary | std::ios::in);
        if (file.is_open())
        {
            file.seekg(0, std::ios::end);
            std::fstream::pos_type size = file.tellg();
            file.seekg(0, std::ios::beg);

            std::vector<char> buffer(size);
            if (size == 0 || file.read(&buffer[0], size))
            {
                zip_fileinfo zfi = { 0 };

                std::string utf8_path;
                if (isWindows()) {
                    // std::filesystem::path 内で SJIS に対して utf8 to wchar 変換が行われ文字化けるため一時的に utf8 に戻す。(不本意)
                    utf8_path = sscc::sjis_to_utf8(path);
                } else {
                    utf8_path = path;
                }

                // ファイルネーム単体の取得はエンコードを配慮したセパレータ検出のため std::filesystem::path 経由でないと不都合があった？と思われるためここは変えない。
                //std::string fileName = path.substr(path.rfind('\\') + 1);
                auto f = std::filesystem::path(utf8_path);
                std::string fileName = f.filename().string();
                //std::filesystem::path p = f.parent_path(); // unused

                if (isWindows()) {
                    // 再び sjis に戻す。
                    fileName = sscc::utf8_to_sjis(fileName);
                }

                if (fileName == "sspkg.json" || fileName == "thumbnail.png")
                {
                    fileName = "meta/" + fileName;
                }

//                if (S_OK == zipOpenNewFileInZip(zf, std::string(fileName.begin(), fileName.end()).c_str(), &zfi, NULL, 0, NULL, 0, NULL, Z_DEFLATED, Z_DEFAULT_COMPRESSION))
                if (0 == zipOpenNewFileInZip(zf, fileName.c_str(), &zfi, NULL, 0, NULL, 0, NULL, Z_DEFLATED, Z_DEFAULT_COMPRESSION))
                {
                    if (zipWriteInFileInZip(zf, size == 0 ? "" : &buffer[0], size))
                        _return = false;

                    if (zipCloseFileInZip(zf))
                        _return = false;

                    file.close();
                    continue;
                }
                else
                {
                    std::cerr << "  new zip failure: " << fileName << "\n";
                }
            }
            else
            {
                std::cerr << "  read failure: " << path << "\n";
            }
            file.close();
        }
        else
        {
            std::cerr << "  open failure: " << path << "\n";
        }
        _return = false;
    }

    if (zipClose(zf, NULL) != ZIP_OK)
        return 3;

    if (!_return)
        return 4;
    return 0;
}

static void   createFileInfoJson(const std::string& versioninfo , const std::filesystem::path& outputfilenamepath , const std::vector<std::string>& org_filelist)
{
    std::vector<std::string> paths;
    for (const auto& i : org_filelist)
    {
        std::string f = std::filesystem::path(i).filename().string();
        paths.push_back(f);
    }

    json j = {
      {"version", 1.0},
      {"ssversion", versioninfo},
    };

    j["filelist"] = paths;

    std::ofstream o(sscc::convert_path_string(outputfilenamepath.string()));
    o << std::setw(4) << j << std::endl;
}


sspkg_info* sspkg_info::myinst = 0;

std::string sspkg_info::get_sspkg_temppath()
{
    return tempdir.string();
}
std::string sspkg_info::get_sspkg_metapath()
{
    return metadir.string();
}

void sspkg_info::init_sspkg(std::string outputdir , std::string pkgname)
{

    if (!std::filesystem::exists(std::filesystem::path(outputdir)))
    {
        if (!std::filesystem::create_directory(std::filesystem::path(outputdir)))
        {
            throw SSException("create_directory failed.", SSPC_SSPKG_ERROR);
        }
    }

    std::filesystem::path temp = std::filesystem::temp_directory_path();

    //    tempdir = std::filesystem::path(outputdir);
    tempdir = temp;
    tempdir += std::filesystem::path("sspkg/");
    std::filesystem::create_directory(tempdir);
    //cleaningDir.push_back(tempdir);

    tempdir += std::filesystem::path(pkgname + "/");
    std::filesystem::create_directory(tempdir);
    cleaningDir.push_back(tempdir);

    metadir = tempdir;
    metadir += std::filesystem::path("meta/");
    std::filesystem::create_directory(metadir);
    cleaningDir.push_back(metadir);
}



void sspkg_info::set_sspkg_filelist(const std::string& ssversion, const std::string& pkgname, const std::vector<std::filesystem::path>& filelist, const std::filesystem::path& outputdir)
{
    archive_file_lists.clear();
    org_file_lists.clear();

    auto tempdir = get_sspkg_temppath();
    auto metadir = get_sspkg_metapath();

    std::filesystem::path ssfb_dst = std::filesystem::path(tempdir) / std::filesystem::path(pkgname).replace_extension(".ssfb");
    archive_file_lists.push_back(ssfb_dst.string());

    for (const auto& i : filelist)
    {
        org_file_lists.push_back(i.string());
    }

    archivefilepath = outputdir / std::filesystem::path(pkgname).replace_extension(".sspkg");
    jsonfilepath = metadir / std::filesystem::path("sspkg.json");

    //archive_file_lists.push_back(jsonfilepath.string());

    thumbnailefilepath = metadir / std::filesystem::path("thumbnail.png");

    //archive_file_lists.push_back(thumbnailefilepath.string());
    data_version = ssversion;

    createFileInfoJson(ssversion, jsonfilepath, org_file_lists);
}


bool sspkg_info::make_sspkg()
{

    /*
        std::filesystem::path tempdir = get_sspkg_temppath();
        std::filesystem::path metadir = get_sspkg_metapath();


        std::filesystem::path ssfb_dst = std::filesystem::path(tempdir).replace_filename(pkgname).replace_extension(".ssfb");
        archive_file_lists.push_back(ssfb_dst.string());


        for (auto i : filelist)
        {
            std::filesystem::path copyfilename = std::filesystem::path(tempdir).replace_filename(std::filesystem::path(i).filename());
            std::filesystem::copy_file(i, copyfilename, std::filesystem::copy_options::update_existing);

            archive_file_lists.push_back(copyfilename.string());
            org_file_lists.push_back(copyfilename.filename().string()+ copyfilename.filename().extension().string());
        }

        std::filesystem::path archivefilepath = std::filesystem::path(outputdir).replace_filename(pkgname).replace_extension(".sspkg");
        std::filesystem::path jsonfilepath = std::filesystem::path(metadir).replace_filename("sspkg").replace_extension(".json");

        archive_file_lists.push_back(jsonfilepath.string());

        std::filesystem::path thumbnailefilepath = std::filesystem::path(metadir).replace_filename("thumbnail").replace_extension(".png");
        archive_file_lists.push_back(thumbnailefilepath.string());
    */

    auto tempdir = get_sspkg_temppath();

    for (auto i : org_file_lists)
    {
        auto copyfilename = std::filesystem::path(tempdir).replace_filename(std::filesystem::path(i).filename());

        std::filesystem::copy_file(i, copyfilename, std::filesystem::copy_options::update_existing);

        archive_file_lists.push_back(copyfilename.string());
    }


    archive_file_lists.push_back(jsonfilepath.string());

    if (std::filesystem::exists(thumbnailefilepath)) {
        archive_file_lists.push_back(thumbnailefilepath.string());
    } else {
        std::cerr << "  not found thumbnail file: " << thumbnailefilepath << "\n";
    }

    // Windosでは最終的に zipOpen 内の fopen64 で SJIS が必要になるため。
    auto tmp_arch_path = sscc::convert_path_string(archivefilepath.string());
    decltype(archive_file_lists) tmp_archive_file_lists;

    if (isWindows()) {
        for (auto &path: archive_file_lists) {
            tmp_archive_file_lists.push_back(sscc::utf8_to_sjis(path));
        }
    } else {
        tmp_archive_file_lists = archive_file_lists;
    }

    bool result = true;
    if (CreateZipFile(tmp_arch_path, tmp_archive_file_lists, tempdir) != 0)
    {
        result = false;
    }

    cleaningFileList.clear();
    for (auto i : archive_file_lists)
    {
        cleaningFileList.push_back(i);
    }

    return result;
}

void sspkg_info::sspkg_cleanup_file()
{
    for (auto i : cleaningFileList)
    {
        try {
            std::filesystem::remove(i);
        }
        catch (...)
        {

        }
    }

    auto temp = std::filesystem::temp_directory_path();


    try {
        std::filesystem::remove(cleaningDir[1]);
    }
    catch (...) {}

    try {
        std::filesystem::remove(cleaningDir[0]);
    }
    catch (...) {}

}
