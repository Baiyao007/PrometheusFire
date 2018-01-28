/////////////////////////////////////////////////
// MapManager
// マップ作成管理クラス
// 作成時間：2016/9/30
// 作成者：氷見悠人
// 最終修正時間：2016年12月14日
// By　柏 ＳＥ実装
/////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TeamWorkGame.Utility;
using TeamWorkGame.Actor;
using System.Diagnostics;               //Assert
using TeamWorkGame.Device;

namespace TeamWorkGame.Def
{
    /// <summary>
    /// Map管理
    /// </summary>
    public static class MapManager
    {
        private static int nowMapIndex = -1;
        private static Map map = null;
        private static int blockSize = 64;
        private static List<Action> CreateStage;
        private static int[,] mapdata;

        public static int StageCount
        {
            get
            {
                return CreateStage.Count;
            }
        }

        public static void Init()
        {
            CreateStage = new List<Action>();
        }

        //public static void SetNowMap(int i)
        //{
        //    Debug.Assert(
        //        i >= 0 && i < CreateStage.Count,
        //        "MapManager->SetNowMapパラメタ範囲外！");
        //    if (i < 0)
        //    {
        //        i = 0;
        //    }
        //    else if (i >= CreateStage.Count)
        //    {
        //        i = CreateStage.Count;
        //    }

        //    //if (nowMapIndex != i)
        //    //{
        //    //nowMapIndex = i;
        //    CreateStage[i]();
        //    //}
        //}

        /// <summary>
        /// 今のマップを設置する
        /// by柏　SE実装 2016.12.14
        /// </summary>
        /// <param name="index">設置したいStage番号</param>
        public static void SetNowMap(int index, Sound sound)
        {
            List<GameObject> MapThings = new List<GameObject>();
            if (nowMapIndex != index)
            {
                int bigIndex = index / StageDef.SmallIndexMax + 1;
                int smallIndex = index % StageDef.SmallIndexMax + 1;
                mapdata = Method.GetMapdataFromFile(bigIndex, smallIndex);
                nowMapIndex = index;
            }
            
            map = new Map(mapdata, blockSize, MapThings);
            Method.CreateGimmicks(mapdata, MapThings, sound);   //by柏　2016.12.14　sound引数追加
        }

        public static Vector2 PlayerStartPosition() {
            return Method.PlayerStartPosition(mapdata);
        }


        public static int[,] GetNowMapArr()
        {
            return map.Data;
        }

        public static Map GetNowMapData()
        {
            return map;
        }
    }
}
