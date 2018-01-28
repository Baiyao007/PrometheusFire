#region 概要
//-----------------------------------------------------------------------------
// アニメションのクラス
// 作成者：氷見悠人
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework.Graphics;

namespace TeamWorkGame.Device
{
    public class Animation
    {
        //フレイムを横に並べる画像
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        //1フレイムの時間
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        //Loopするか？
        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        //フレイム総数
        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        //フレイムの横幅
        public int FrameWidth
        {
            // Assume square frames.
            get { return Texture.Height; }
        }

        //フレイムの横幅
        public int FrameHeight
        {
            get { return Texture.Height; }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="texture">フレイムを並べる画像</param>
        /// <param name="frameTime">1フレイムの時間</param>
        /// <param name="isLooping">Loopするか？</param>
        public Animation(Texture2D texture, float frameTime, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.isLooping = isLooping;
        }
    }
}
