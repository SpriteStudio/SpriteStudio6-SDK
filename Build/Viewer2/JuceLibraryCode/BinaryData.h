/* =========================================================================================

   This is an auto-generated file: Any edits you make may be overwritten!

*/

#pragma once

namespace BinaryData
{
    extern const char*   sprite_fs;
    const int            sprite_fsSize = 964;

    extern const char*   sprite_vs;
    const int            sprite_vsSize = 581;

    extern const char*   sprite_pot_fs;
    const int            sprite_pot_fsSize = 775;

    extern const char*   apple_csv;
    const int            apple_csvSize = 269126;

    extern const char*   c932uni_dat;
    const int            c932uni_datSize = 75696;

    extern const char*   cp932_csv;
    const int            cp932_csvSize = 328435;

    extern const char*   eucscr_dat;
    const int            eucscr_datSize = 32916;

    extern const char*   eucsjis_dat;
    const int            eucsjis_datSize = 6375;

    extern const char*   eucuni_csv;
    const int            eucuni_csvSize = 511700;

    extern const char*   euc2uni_dat;
    const int            euc2uni_datSize = 77213;

    extern const char*   euc3uni_dat;
    const int            euc3uni_datSize = 63509;

    extern const char*   scoremap_csv;
    const int            scoremap_csvSize = 64787;

    extern const char*   sjiseuc_csv;
    const int            sjiseuc_csvSize = 11252;

    extern const char*   sjiseuc_dat;
    const int            sjiseuc_datSize = 5434;

    extern const char*   sjisscr_dat;
    const int            sjisscr_datSize = 30314;

    extern const char*   unic932_dat;
    const int            unic932_datSize = 156271;

    extern const char*   unieuc_dat;
    const int            unieuc_datSize = 187146;

    extern const char*   uniscr_dat;
    const int            uniscr_datSize = 101210;

    extern const char*   uniutc_dat;
    const int            uniutc_datSize = 152224;

    extern const char*   utcuni_dat;
    const int            utcuni_datSize = 68086;

    extern const char*   utc_csv;
    const int            utc_csvSize = 197304;

    // Points to the start of a list of resource names.
    extern const char* namedResourceList[];

    // Number of elements in the namedResourceList array.
    const int namedResourceListSize = 21;

    // If you provide the name of one of the binary resource variables above, this function will
    // return the corresponding data and its size (or a null pointer if the name isn't found).
    const char* getNamedResource (const char* resourceNameUTF8, int& dataSizeInBytes) throw();
}
