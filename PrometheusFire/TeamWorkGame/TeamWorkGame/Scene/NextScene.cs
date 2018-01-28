#region 概要
//-----------------------------------------------------------------------------
// 次のシーンの構造体
// 作成者：氷見悠人
//-----------------------------------------------------------------------------
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamWorkGame.Scene
{
    struct NextScene
    {
        public SceneType sceneType;
        public int stageIndex;
        
        /// <summary>
        /// NextSceneを構築
        /// </summary>
        /// <param name="scene">Sceneの名</param>
        /// <param name="index">Stage番号、必要ない場合-1にする</param>
        public NextScene(SceneType scene, int index = -1)
        {
            sceneType = scene;
            stageIndex = index;
        }
    }
}
