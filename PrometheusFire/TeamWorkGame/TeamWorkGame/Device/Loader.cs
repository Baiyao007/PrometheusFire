using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamWorkGame.Device
{
    /// <summary>
    /// 読み込み抽象クラス
    /// </summary>
   　abstract class Loader
    {
        protected string[,] resources;  //リソースアセット名群
        protected int counter;          //カウンタ
        protected int maxNum;           //リソース最大数
        protected bool endFlag;         //終了フラグ

        public Loader(string[,] resources)
        {
            this.resources = resources;
        }

        public void Initialize()
        {
            counter = 0;
            endFlag = false;
            maxNum = 0;
            if (resources != null)
            {
                maxNum = resources.GetLength(0);
            }
        }

        public int Count()
        {
            return maxNum;
        }

        public int CurrentCount()
        {
            return counter;
        }

        public bool IsEnd()
        {
            return endFlag;
        }

        public abstract void Update();
    }
}
