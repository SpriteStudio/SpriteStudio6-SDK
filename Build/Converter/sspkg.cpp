#include "sspkg.h"
#include "sscharconverter.h"



#include <zlib.h>
#include <contrib/minizip/zip.h>
#include <nlohmann/json.hpp>

#include <iostream>
#include <sstream>
#include <iomanip>
#include <string>

//#include "sscharconverter.h"


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

#ifdef _WIN32
                // fs::path 内で SJIS に対して utf8 to wchar 変換が行われ文字化けるため一時的に utf8 に戻す。(不本意)
                auto utf8_path = sscc::sjis_to_utf8(path);
#else
                auto utf8_path = path;
#endif
                // ファイルネーム単体の取得はエンコードを配慮したセパレータ検出のため fs::path 経由でないと不都合があった？と思われるためここは変えない。
                //std::string fileName = path.substr(path.rfind('\\') + 1);
                fs::path f = fs::path(utf8_path);
                std::string fileName = f.filename().string();
                //fs::path p = f.parent_path(); // unused

#ifdef _WIN32
                // 再び sjis に戻す。
                fileName = sscc::utf8_to_sjis(fileName);
#endif

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

static void   createFileInfoJson(const std::string& versioninfo , const fs::path& outputfilenamepath , const std::vector<std::string>& org_filelist)
{
    std::vector<std::string> paths;
    for (const auto& i : org_filelist)
    {
        std::string f = fs::path(i).filename().string();
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

    if (!fs::exists(fs::path(outputdir)))
    {
        if (!fs::create_directory(fs::path(outputdir)))
        {
            throw "create_directory failed.";
        }
    }

    fs::path temp = fs::temp_directory_path();

    //    tempdir = fs::path(outputdir);
    tempdir = temp;
    tempdir += fs::path("sspkg/");
    fs::create_directory(tempdir);
    //cleaningDir.push_back(tempdir);

    tempdir += fs::path(pkgname + "/");
    fs::create_directory(tempdir);
    cleaningDir.push_back(tempdir);

    metadir = tempdir;
    metadir += fs::path("meta/");
    fs::create_directory(metadir);
    cleaningDir.push_back(metadir);
}



void sspkg_info::set_sspkg_filelist(const std::string& ssversion, const std::string& pkgname, const std::vector<fs::path>& filelist, const fs::path& outputdir)
{
    archive_file_lists.clear();
    org_file_lists.clear();

    fs::path tempdir = get_sspkg_temppath();
    fs::path metadir = get_sspkg_metapath();

    fs::path ssfb_dst = fs::path(tempdir) / fs::path(pkgname).replace_extension(".ssfb");
    archive_file_lists.push_back(ssfb_dst.string());

    for (const auto& i : filelist)
    {
        org_file_lists.push_back(i.string());
    }

    archivefilepath = outputdir / fs::path(pkgname).replace_extension(".sspkg");
    jsonfilepath = metadir / fs::path("sspkg.json");

    //archive_file_lists.push_back(jsonfilepath.string());

    thumbnailefilepath = metadir / fs::path("thumbnail.png");

    //archive_file_lists.push_back(thumbnailefilepath.string());
    data_version = ssversion;

    createFileInfoJson(ssversion, jsonfilepath, org_file_lists);
}


bool sspkg_info::make_sspkg()
{

    /*
        fs::path tempdir = get_sspkg_temppath();
        fs::path metadir = get_sspkg_metapath();


        fs::path ssfb_dst = fs::path(tempdir).replace_filename(pkgname).replace_extension(".ssfb");
        archive_file_lists.push_back(ssfb_dst.string());


        for (auto i : filelist)
        {
            fs::path copyfilename = fs::path(tempdir).replace_filename(fs::path(i).filename());
            fs::copy_file(i, copyfilename, fs::copy_options::update_existing);

            archive_file_lists.push_back(copyfilename.string());
            org_file_lists.push_back(copyfilename.filename().string()+ copyfilename.filename().extension().string());
        }

        fs::path archivefilepath = fs::path(outputdir).replace_filename(pkgname).replace_extension(".sspkg");
        fs::path jsonfilepath = fs::path(metadir).replace_filename("sspkg").replace_extension(".json");

        archive_file_lists.push_back(jsonfilepath.string());

        fs::path thumbnailefilepath = fs::path(metadir).replace_filename("thumbnail").replace_extension(".png");
        archive_file_lists.push_back(thumbnailefilepath.string());
    */

    fs::path tempdir = get_sspkg_temppath();

    for (auto i : org_file_lists)
    {
        fs::path copyfilename = fs::path(tempdir).replace_filename(fs::path(i).filename());

        fs::copy_file(i, copyfilename, fs::copy_options::update_existing);

        archive_file_lists.push_back(copyfilename.string());
    }


    archive_file_lists.push_back(jsonfilepath.string());

    if (fs::exists(thumbnailefilepath)) {
        archive_file_lists.push_back(thumbnailefilepath.string());
    } else {
        std::cerr << "  not found thumbnail file: " << thumbnailefilepath << "\n";
    }

    // Windosでは最終的に zipOpen 内の fopen64 で SJIS が必要になるため。
    auto tmp_arch_path = sscc::convert_path_string(archivefilepath.string());
    decltype(archive_file_lists) tmp_archive_file_lists;

#ifdef _WIN32
    // std::fstream で要SJIS
    for (auto& path : archive_file_lists)
    {
        tmp_archive_file_lists.push_back(sscc::utf8_to_sjis(path));
    }
#else
    tmp_archive_file_lists = archive_file_lists;
#endif

    bool result = true;
    if (CreateZipFile(tmp_arch_path, tmp_archive_file_lists, tempdir.string()) != 0)
    {
        result = false;
    }

#ifndef _DEBUG

#endif
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
            fs::remove(i);
        }
        catch (...)
        {

        }
    }

    fs::path temp = fs::temp_directory_path();


    try {
        fs::remove(cleaningDir[1]);
    }
    catch (...) {}

    try {
        fs::remove(cleaningDir[0]);
    }
    catch (...) {}

}
