#region 概要
//-----------------------------------------------------------------------------
// Stageに関する定義
// 作成者：氷見悠人
//-----------------------------------------------------------------------------
#endregion


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamWorkGame.Def
{
    public static class StageDef
    {
        public const string StagePath = "Content/Stage/";
        public const int BigIndexMax = 5;
        public const int SmallIndexMax = 6;

        public static readonly List<GimmickType> ImpassableTiles = new List<GimmickType>()
        {
            GimmickType.GROUND1, GimmickType.GROUND2,
        };

        public static readonly List<GimmickType> PlatformTiles = new List<GimmickType>()
        {

        };

        public static readonly List<GimmickType> PassableTiles = new List<GimmickType>()
        {

        };
    }
}
