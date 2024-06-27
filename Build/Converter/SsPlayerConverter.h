//
//  SsPlayerConverter.h
//

#ifndef _SsPlayerConverter_h
#define _SsPlayerConverter_h

enum SsPlayerConverterResultCode {
    SSPC_SUCCESS = 0,                  // 成功
    SSPC_INTERNAL_ERROR,               // 内部エラーが発生しました（※バグなどの不具合が無い限り通常発生しません）
    SSPC_NOT_SPECIFY_INPUT_FILE,       // 入力ファイルの指定がありません
    SSPC_NOT_EXIST_INPUT_FILE,         // 指定された入力ファイルが存在しません
    SSPC_ILLEGAL_ARGUMENT,             // 引数が不正です
    SSPC_NOT_SUPPORT_OUTPUT_ENCODING,  // 対応していない出力ファイルのエンコーディング形式が指定されました
    SSPC_IMPOSSIBLE_PLURAL_SSF_FILES,  // ssfファイルが複数指定されています。一つのみ指定可能です
    SSPC_SSAX_PARSE_FAILED,            // ssaxファイルの解析に失敗しました
    SSPC_SSPJ_PARSE_FAILED,            // sspjファイルの解析に失敗しました

    SSPC_INCOMPATIBLE_VERSION_ERROR,  // 対応しないバージョン
    SSPC_PARTS_ERROR,                 //
    SSPC_NOT_EXIST_OUTPUT_DIR,        // 出力ディレクトリがありません
    SSPC_NOT_EXIST_CELLMAP,           // セルマップが存在しません。
    SSPC_NOT_CELL_IN_CELLMAP,         // セルマップにセルが存在しません。
    SSPC_NOT_ANIMATION,               // アニメーションが存在しません
    SSPC_NOT_OUTPUT_FILE,             // 出力ファイルが開けません。
    SSPC_SSPKG_ERROR,                 // SSPKG エラー
    SSPC_ZENKAKU_ERROR,               // 全角エラー
    SSPC_UNSUPPORT_FORMAT,            // 未対応のフォーマット
    SSPC_UNSUPPORT_EFFECT_COMMAND,    // 未対応のエフェクトコマンド

    END_OF_SSPC
};

#endif
