/////////////////////////////////////////////////
// 鉄ブロックのクラス
// 作成時間：2016年10月12日
// By 長谷川修一
// 最終修正時間：2016年11月30日
// By 葉梨竜太
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TeamWorkGame.Device;
using TeamWorkGame.Def;
using TeamWorkGame.Utility;
using Microsoft.Xna.Framework.Graphics;

namespace TeamWorkGame.Actor
{
    public class Iron : GameObject
    {
        //private Timer timer;
        private bool isToDeath;
        private Animation animation;
        private AnimationPlayer animationPlayer;
        private bool IsAnimation = false;

        public Iron(Vector2 pos, Vector2 velo)
            : base("iron", pos, velo, false, "Iron")
        {
            //animationPlayer = new AnimationPlayer();
        }

        public override void Initialize()
        {
            base.Initialize();
            //timer = new Timer(0.1f);
            isToDeath = false;
            isShow = true;
            animation = new Animation(Renderer.GetTexture("ironAnime"), Parameter.IronAnimeTime / 4, false);//4はフレーム数
            animationPlayer.PlayAnimation(animation);
            SetTimer(Parameter.IronAnimeTime, 5f);
        }
        /// <summary>
        /// 死亡する時
        /// </summary>
        public void ToDeath()
        {
            if (!isToDeath)
            {
                isToDeath = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            AliveUpdate();
        }

        public override void AliveEvent(GameObject other)
        {
            if (other is Fire)
            {
                other.IsDead = true;
                isShow = false;         //不可視化
            }
            if (other is Player)
            {
                isShow = false;         //Playerの場合は不可視化だけ
            }
            spawnTimer.Initialize(); //Timerを初期化して可視化するのを防ぐ
        }

        /// <summary>
        /// 描画の再定義（透明値を追加）　By　氷見悠人　2016/10/20
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="renderer"></param>
        /// <param name="offset"></param>
        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            //アニメーションの追加 by 長谷川修一
            if (IsAnimation)
            {
                animationPlayer.Draw(gameTime, renderer, position * cameraScale + offset, SpriteEffects.None, cameraScale);
                IsAnimation = animationPlayer.Reset(isShow);
            }
            else
            {
                renderer.DrawTexture(name, position * cameraScale + offset, cameraScale, alpha);
            }
        }

        public override void EventHandle(GameObject other)
        {
            //火の数が5以上の時に消える
            if (other is Player && ((Player)other).FireNum >= Parameter.ironfire)
            {
                //ToDeath();
                AliveEvent(other);
                IsAnimation = true;
            }
            //葉梨竜太 11/30
            else if(other is Bomb)
            {
                BombEvent(other);
            }
            //AliveEvent(other);
        }
    }
}
