// 
//火の粉Effect　By　氷見悠人
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamWorkGame.Device;    //入力状態クラス
using TeamWorkGame.Def;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Actor
{
    public class FireDust : GameObject
    {
        private Animation anime;             //アニメ
        private AnimationPlayer animePlayer;    //アニメ再生器
        private float disappearTime;        //消え始める時間
        private float lifeTime; 

        public FireDust(Vector2 pos, Vector2 velo, int fireNum) : base("FireDust", pos, velo, true, "FireDust")
        {
            anime = new Animation(Renderer.GetTexture("FireDust"), 0.1f, true);
            animePlayer.PlayAnimation(anime);
            disappearTime = 0.2f;
            SetLifeTime(fireNum);
        }

        public override void EventHandle(GameObject other)
        {
            
        }

        /// <summary>
        /// 存在時間を設定
        /// </summary>
        /// <param name="fireNum"></param>
        public void SetLifeTime(int fireNum)
        {
            lifeTime = (float)fireNum / (float)Parameter.FireMaxNum;
        }

        public override void Update(GameTime gameTime)
        {
            if(lifeTime < 0)
            {
                isDead = true;
            }
            else
            {
                alpha = lifeTime > disappearTime ? (1.0f) : (lifeTime / disappearTime);
                lifeTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                position.Y += 0.3f;
            }
        }

        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            animePlayer.Draw(gameTime, renderer, position * cameraScale + offset, SpriteEffects.None, cameraScale, alpha);
        }
    }
}
