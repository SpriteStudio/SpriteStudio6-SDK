#ifndef __SSPKG__
#define __SSPKG__

#include <string>
#include <vector>
#include <ghc/filesystem.hpp>
namespace fs = ghc::filesystem;
/*
void make_sspkg(std::string ssversion, std::string pkgname, std::vector<std::string> filelist, std::string outputdir);

void init_sspkg(std::string outputdir, std::string pkgname);
void sspkg_cleanup_file();

std::string get_sspkg_temppath();
std::string get_sspkg_metapath();
*/

class sspkg_info {
private:

    std::vector<fs::path> cleaningFileList;
    std::vector<fs::path> cleaningDir;

    static sspkg_info* myinst;

     fs::path tempdir;
     fs::path metadir;

    std::vector<std::string> archive_file_lists;
    std::vector<std::string> org_file_lists;


    fs::path archivefilename;
    fs::path jsonfilename;
    fs::path thumbnailename;
    std::string data_version;

public:
    sspkg_info() {}
    virtual ~sspkg_info() {}

    static sspkg_info* getInst() { return myinst; }
    static sspkg_info* create()
    {
        if (myinst == 0)
        {
            myinst = new sspkg_info();
        }
        return myinst;
    }
    static void destroy() {
        delete myinst;
        myinst = 0;
    }

    //-----------------------------------
    std::string get_sspkg_temppath();
    std::string get_sspkg_metapath();
    void init_sspkg(std::string outputdir, std::string pkgname);

    void set_sspkg_filelist(std::string ssversion, std::string pkgname, std::vector<std::string> filelist, std::string outputdir);

    void make_sspkg();
    void sspkg_cleanup_file();


};



#endif


