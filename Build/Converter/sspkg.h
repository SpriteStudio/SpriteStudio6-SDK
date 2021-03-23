#ifndef __SSPKG__
#define __SSPKG__

#include <string>
#include <vector>

void make_sspkg(std::string ssversion, std::string pkgname, std::vector<std::string> filelist, std::string outputdir);

void init_sspkg(std::string outputdir, std::string pkgname);
void sspkg_cleanup_file();

std::string get_sspkg_temppath();
std::string get_sspkg_metapath();

#endif


