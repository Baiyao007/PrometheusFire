#region 概要
//-----------------------------------------------------------------------------
// アニメーション再生構造体
// 作成者：氷見悠人
// 最終修正時間 11/16 by長谷川修一
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Device
{
    public struct AnimationPlayer
    {
        //再生するアニメーション
        public Animation Animation
        {
            get { return animation; }
        }
        Animation animation;

        //フレイム番号
        public int FrameIndex
        {
            get { return frameIndex; }
        }
        int frameIndex;

        /// <summary>
        /// The amount of time in seconds that the current frame has been shown for.
        /// </summary>
        private float time;

        private bool isPaused;  //一時停止　By　氷見悠人
        public bool IsPaused
        {
            get
            {
                return isPaused;
            }
            set
            {
                isPaused = value;
            }
        }
        //描画位置の原点
        public Vector2 Origin
        {
            get { return Vector2.Zero; } //new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); 
        }

        /// <summary>
        /// アニメションを再生
        /// </summary>
        /// <param name="animation">指定アニメション</param>
        public void PlayAnimation(Animation animation)
        {
            //同じも指定したら、何もしない
            if (Animation == animation)
                return;
            isPaused = false;
            //そのアニメションの最初から再生
            this.animation = animation;
            this.frameIndex = 0;
            this.time = 0.0f;
        }

        /// <summary>
        /// アニメーションの描画
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="renderer"></param>
        /// <param name="position">描画位置</param>
        /// <param name="spriteEffects">向き</param>
        public void Draw(GameTime gameTime, Renderer renderer, Vector2 position, SpriteEffects spriteEffects, float cameraScale = 1.0f, float alpha = 1.0f)
        {
            if (Animation == null)
                throw new NotSupportedException("アニメーション指定していません！");

            if (!isPaused && !FuncSwitch.AllAnimetionPause)
            {
                // 経過時間計算
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (time > Animation.FrameTime)
                {
                    time -= Animation.FrameTime;

                    //Loopの処理
                    if (Animation.IsLooping)
                    {
                        frameIndex = (frameIndex + 1) % Animation.FrameCount;
                    }
                    else
                    {
                        frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
                    }
                }
            }

            //今描画する画像範囲を計算
            Rectangle range = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);

            // Draw the current frame.
            //spriteBatch.Draw(Animation.Texture, position, source, Color.White, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
            //今のフレイムを描画
            renderer.DrawTexture(Animation.Texture, position, range, spriteEffects, alpha, 0.0f, cameraScale);
        }

        /// <summary>
        /// アニメーションの描画(回転用)
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="renderer"></param>
        /// <param name="position">描画位置</param>
        /// <param name="spriteEffects">向き</param>
        /// <param name="origin">中心座標</param>
        /// <param name="rotation">回転角度</param>
        /// <param name="cameraScale"></param>
        public void Draw(GameTime gameTime, Renderer renderer, Vector2 position, SpriteEffects spriteEffects, Vector2 origin, float rotation, float cameraScale, float alpha)
        {
            if (Animation == null)
                throw new NotSupportedException("アニメーション指定していません！");

            if (!isPaused && !FuncSwitch.AllAnimetionPause)
            {
                // 経過時間計算
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (time > Animation.FrameTime)
                {
                    time -= Animation.FrameTime;

                    //Loopの処理
                    if (Animation.IsLooping)
                    {
                        frameIndex = (frameIndex + 1) % Animation.FrameCount;
                    }
                    else
                    {
                        frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);
                    }
                }
            }

            //今描画する画像範囲を計算
            Rectangle range = new Rectangle(FrameIndex * Animation.Texture.Height, 0, Animation.Texture.Height, Animation.Texture.Height);

            // Draw the current frame.
            //spriteBatch.Draw(Animation.Texture, position, source, Color.White, 0.0f, Origin, 1.0f, spriteEffects, 0.0f);
            //今のフレイムを描画
            renderer.DrawTexture(Animation.Texture, position, range, spriteEffects, origin, alpha, rotation, cameraScale);
        }

        public bool Reset(bool isShow)
        {
            if (isShow)
            {
                frameIndex = 0;
                return false;
            }
            return true;
        }

        public bool IsEnd()
        {
            return frameIndex >= animation.FrameCount - 1;
        }

        /// <summary>
        /// ループしないアニメーションのリセット
        /// </summary>
        /// <param name="animation">リセットしたいアニメーション</param>
        public void ResetAnimation(Animation animation)
        {
            if (frameIndex == Animation.FrameCount - 1)
            {
                frameIndex = 0;
            }
        }

        /// <summary>
        /// 現在のフレームを返す
        /// </summary>
        /// <returns>現在のフレーム</returns>
        public int FrameNow()
        {
            return frameIndex;
        }
    }
}
