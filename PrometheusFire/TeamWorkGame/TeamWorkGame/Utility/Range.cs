//////////////////////////////////////////////////////////
// アニメーション用の描画範囲管理
// 作成者：氷見悠人
//////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamWorkGame.Utility
{
    /// <summary>
    /// 範囲クラス
    /// </summary>
    class Range
    {
        private int first;  //最初
        private int end;    //終端

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="first"></param>
        /// <param name="end"></param>
        public Range(int first, int end)
        {
            this.first = first;
            this.end = end;
        }

        /// <summary>
        /// Get最初の番号
        /// </summary>
        /// <returns>最初の番号</returns>
        public int First()
        {
            return first;
        }

        /// <summary>
        /// Get最後の番号
        /// </summary>
        /// <returns>最後の番号</returns>
        public int End()
        {
            return end;
        }

        /// <summary>
        /// 範囲内か？
        /// </summary>
        /// <param name="num">調べたい番号</param>
        /// <returns>範囲内だったらtrue</returns>
        public bool IsWithin(int num)
        {
            //最初の番号より小さい
            if (num < first || num > end)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// （設定した開始・終端が）範囲外か？
        /// </summary>
        /// <returns></returns>
        public bool IsOutOfRange()
        {
            return first >= end;
        }

        /// <summary>
        /// 指定された番号が範囲外か？
        /// </summary>
        /// <param name="num">調べたい番号</param>
        /// <returns>範囲外ならtrue</returns>
        public bool IsOutOfRange(int num)
        {
            return !IsWithin(num);
        }
    }
}
