///////////////////////////////////////////////////////////////////////
// マップデータクラス
// 作成者：氷見悠人
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TeamWorkGame.Def;
using TeamWorkGame.Actor;

namespace TeamWorkGame.Utility
{
    /// <summary>
    /// 方向の列挙型
    /// </summary>
    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        
    }

    enum TileCollision
    {
        Passable = 0,
        Impassable = 1,
        Platform = 2,
    }

    /// <summary>
    /// BlockのIndex構造体
    /// </summary>
    public struct BlockIndex
    {
        public int X { get; set; }
        public int Y { get; set; }

        public BlockIndex(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetIndex(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Map処理用クラス
    /// </summary>
    public class Map
    {
        //プロパティ
        public int[,] Data { get; }             //地図のデータ
        public int BlockSize { get; }           //地図BlockのSize
        public int HorizontalBlockNum { get; }  //水平方向のBlock数
        public int VerticalBlockNum { get; }    //垂直方向のBlock数
        public int MapWidth { get; }            //地図の横長さ
        public int MapHeight { get; }           //地図の高さ
        public List<Actor.GameObject> MapThings { get; }       //地図にある物達のデータ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="data">地図のデータ</param>
        /// <param name="blockSize">地図BlockのSize</param>
        public Map(int[,] data, int blockSize, List<Actor.GameObject> things)
        {
            Data = data;
            BlockSize = blockSize;
            MapThings = things;
            HorizontalBlockNum = data.GetLength(1);
            VerticalBlockNum = data.GetLength(0);
            MapWidth = blockSize * HorizontalBlockNum;
            MapHeight = blockSize * VerticalBlockNum;
        }

        public void Update(GameTime gameTime)
        {
            //By　張
            //foreach(var x in MapThings)
            //{
            //    if(x.PositionY > MapHeight)
            //    {
            //        x.IsDead = true;
            //    }
            //}
            //マップ上に死んだ物を消す
            MapThings.RemoveAll(x => x.IsDead);
        }

        public Goal GetGoal()
        {
            Goal goal = (Goal)MapThings.Find(x => x.Tag == "Goal");
            return goal;
        }
        /// <summary>
        /// 指定位置からBlockのIndexを調べる
        /// </summary>
        /// <param name="pos">調べたい位置</param>
        /// <returns>その位置のBlockIndex</returns>
        public BlockIndex GetBlockIndex(Vector2 pos)
        {
            float x = pos.X / BlockSize;
            float y = pos.Y / BlockSize;
            BlockIndex blockIndex = new BlockIndex(0, 0);

            // 範囲のチェック
            if (x < 0 || x >= HorizontalBlockNum || y < 0 || y >= VerticalBlockNum)
            {
                // 範囲外
                blockIndex.SetIndex(-1, -1);
            }
            else
            {
                blockIndex.SetIndex((int)x, (int)y);
            }

            return blockIndex;
        }

        /// <summary>
        /// 指定BlockIndexから、位置を調べる
        /// </summary>
        /// <param name="blockIndex">調べたいBlockIndex</param>
        /// <returns>その位置</returns>
        public Vector2 GetBlockPosition(BlockIndex blockIndex)
        {
            Vector2 pos = new Vector2(blockIndex.X * BlockSize, blockIndex.Y * BlockSize);
            return pos;
        }

        public Rectangle GetBlockRect(int x, int y)
        {
            Vector2 pos = GetBlockPosition(new BlockIndex(x, y));
            return new Rectangle((int)pos.X, (int)pos.Y, BlockSize, BlockSize);
        }

        /// <summary>
        /// 指定位置のMapの内容を調べる
        /// </summary>
        /// <param name="pos">位置</param>
        /// <returns>内容</returns>
        public int GetBlockData(Vector2 pos)
        {
            BlockIndex blockIndex = GetBlockIndex(pos);

            //調べたい位置は地図外とすると、-1（内容なし）を返す
            if (blockIndex.X == -1 || blockIndex.Y == -1)
            {
                return -1;
            }
            else
            {
                return Data[blockIndex.Y, blockIndex.X];
            }
        }

        /// <summary>
        /// その位置は指定の地形であるかどうかを調べる
        /// </summary>
        /// <param name="pos">位置</param>
        /// <param name="blockIndex">その位置のBlockIndexを返す</param>
        /// <param name="data">指定の地形種類の配列</param>
        /// <returns></returns>
        public bool IsInBlock(Vector2 pos, ref Vector2 blockPos, int[] data)
        {
            BlockIndex blockIndex = GetBlockIndex(pos);
            bool flag = false;

            if (blockIndex.X == -1 || blockIndex.Y == -1)
            {
                return false;
            }

            foreach (var d in data)
            {
                if(d == Data[blockIndex.Y, blockIndex.X])
                {
                    flag = true;
                    blockPos = GetBlockPosition(blockIndex);
                    break;
                }
            }

            return flag;
        }
    }
}
