#include "sspkg.h"

#include <ghc/filesystem.hpp>
namespace fs = ghc::filesystem;

#include <zlib.h>
#include <minizip/zip.h>
//#include <zlib/contrib/minizip/zip.h>
#include <nlohmann/json.hpp>

#include <iostream>
#include <sstream>
#include <iomanip>
#include <string>


using json = nlohmann::json;

int CreateZipFile(std::string zippath ,  std::vector<std::string> paths , std::string remove_path )
{
    zipFile zf = zipOpen( zippath.c_str() , APPEND_STATUS_CREATE);
    if (zf == NULL)
        return 1;

    bool _return = true;
    for (size_t i = 0; i < paths.size(); i++)
    {
        std::fstream file(paths[i].c_str(), std::ios::binary | std::ios::in);
        if (file.is_open())
        {
            file.seekg(0, std::ios::end);
            long size = file.tellg();
            file.seekg(0, std::ios::beg);

            std::vector<char> buffer(size);
            if (size == 0 || file.read(&buffer[0], size))
            {
                zip_fileinfo zfi = { 0 };

                //std::string fileName = paths[i].substr(paths[i].rfind('\\') + 1);
                fs::path f = fs::path(paths[i]);

                std::string fileName = f.filename().string();
                fs::path p = f.parent_path();

                if (fileName == "sspkg.json" || fileName == "thumbnail.png")
                {
                    fileName = "meta/" + fileName;
                }

//                if (S_OK == zipOpenNewFileInZip(zf, std::string(fileName.begin(), fileName.end()).c_str(), &zfi, NULL, 0, NULL, 0, NULL, Z_DEFLATED, Z_DEFAULT_COMPRESSION))
                if (S_OK == zipOpenNewFileInZip(zf, fileName.c_str(), &zfi, NULL, 0, NULL, 0, NULL, Z_DEFLATED, Z_DEFAULT_COMPRESSION))
                    {
                    if (zipWriteInFileInZip(zf, size == 0 ? "" : &buffer[0], size))
                        _return = false;

                    if (zipCloseFileInZip(zf))
                        _return = false;

                    file.close();
                    continue;
                }
            }
            file.close();
        }
        _return = false;
    }

    if (zipClose(zf, NULL))
        return 3;

    if (!_return)
        return 4;
    return S_OK;
}

static void    createFileInfoJson(std::string versioninfo , std::string outputfilenamepath , std::vector<std::string> org_filelist)
{

    json j = {
      {"version", 1.0},
      {"ssversion", versioninfo},
    };

    j["filelist"] = org_filelist;


    std::ofstream o(outputfilenamepath);
    o << std::setw(4) << j << std::endl;   // std::setw でインデント幅を指定できる。

}

static fs::path tempdir;
static fs::path metadir;


std::string get_sspkg_temppath()
{
    return tempdir.string();
}
std::string get_sspkg_metapath()
{
    return metadir.string();
}

//テンポラリフォルダを作っておく
void init_sspkg(std::string outputdir , std::string pkgname)
{
    if (!fs::exists(fs::path(outputdir)))
    {
        fs::create_directory(fs::path(outputdir));
    }

    fs::path temp = fs::temp_directory_path();
    

//    tempdir = fs::path(outputdir);
    tempdir = temp;
    tempdir += fs::path("sspkg/");
    fs::create_directory(tempdir);

    tempdir += fs::path(pkgname+"/");
    fs::create_directory(tempdir);

    metadir = tempdir;
    metadir += fs::path("meta/");
    fs::create_directory(metadir);

}


std::vector<fs::path> cleaningFileList;


void make_sspkg( std::string ssversion , std::string pkgname , std::vector<std::string> filelist , std::string outputdir )
{

    std::vector<std::string> archive_file_lists;
    std::vector<std::string> org_file_lists;

    fs::path tempdir = get_sspkg_temppath();
    fs::path metadir = get_sspkg_metapath();

 //   fs::path ssfb_org = fs::path(outputdir).replace_filename(pkgname).replace_extension(".ssfb");
    
    fs::path ssfb_dst = fs::path(tempdir).replace_filename(pkgname).replace_extension(".ssfb");
        
//    fs::copy_file(ssfb_org , ssfb_dst, fs::copy_options::update_existing);
    archive_file_lists.push_back(ssfb_dst.string());


	for (auto i : filelist)
	{
		fs::path copyfilename = fs::path(tempdir).replace_filename(fs::path(i).filename());
		fs::copy_file(i, copyfilename, fs::copy_options::update_existing);

        archive_file_lists.push_back(copyfilename.string());
        org_file_lists.push_back(copyfilename.filename().string()+ copyfilename.filename().extension().string());
	}

//    fs::path archivefilename = fs::path(outputdir).replace_filename(pkgname).replace_extension(".zip");
    fs::path archivefilename = fs::path(outputdir).replace_filename(pkgname).replace_extension(".sspkg");

    fs::path jsonfilename = fs::path(metadir).replace_filename("sspkg").replace_extension(".json");
    archive_file_lists.push_back(jsonfilename.string());


    fs::path thumbnailename = fs::path(metadir).replace_filename("thumbnail").replace_extension(".png");
    archive_file_lists.push_back(thumbnailename.string());


    createFileInfoJson(ssversion, jsonfilename.string() , org_file_lists);


    CreateZipFile(archivefilename.string(), archive_file_lists , tempdir.string() );

    //テンポラリの削除
#ifndef _DEBUG

#endif
    cleaningFileList.clear();
    for (auto i : archive_file_lists)
    {
        cleaningFileList.push_back(i);
    }

}

void sspkg_cleanup_file()
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

}
