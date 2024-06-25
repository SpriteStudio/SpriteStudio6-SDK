#ifndef __TKSCENE__
#define __TKSCENE__

#include "sshObject.h"

namespace spritestudio6 {

/// シーングラフを構築するためのクラス
/// 色々オミットする シーンを既定できればOK
class tkScene : public tkObject {
   public:
    tkScene() {}
    virtual ~tkScene() {}

    virtual void draw() {};
    virtual void update(double delta) { SPRITESTUDIO6SDK_NOUSE_ARGUMENT(delta); };
    virtual void init() {};
};

}  // namespace spritestudio6

#endif
