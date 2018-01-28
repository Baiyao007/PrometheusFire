////////////////////////////////////////////////////////////
// サイズの構造体
// 作成者：氷見悠人
//////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamWorkGame
{
    /// <summary>
    /// サイズ構造体
    /// </summary>
    public struct Size
    {
        public int Width;   //横幅
        public int Height;  //縦幅

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Size(int w, int h)
        {
            Width = w;
            Height = h;
        }
    }
}