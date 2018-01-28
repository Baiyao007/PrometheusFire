//////////////////////////////////////////////////////////////////////////////
// 氷クラス
// 作成者：氷見悠人
// 最終修正時間：2016/11/30
// By 葉梨竜太
////////////////////////////////////////////////////////////////////////////

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
    public class Ice : GameObject
    {
        private bool isToDeath;
        private List<WaterLine> waters;
        private Animation animation;
        private AnimationPlayer animationPlayer;
        private bool IsAnimation = false;

        public Ice(Vector2 pos, Vector2 velo) : base("ice", pos, velo, false, "Ice")
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();
            isToDeath = false;
            isShow = true;      //初期値はtrue by佐瀬拓海
            animation = new Animation(Renderer.GetTexture("iceAnime"), Parameter.IceAnimeTime / 2, false);
            //animationPlayer = new AnimationPlayer(animation);
            animationPlayer.PlayAnimation(animation);
            SetTimer(Parameter.IceAnimeTime, Parameter.IceSpawnTime);
        }

        public void SetWaters(List<WaterLine> waters)
        {
            this.waters = waters;
        }

        /// <summary>
        /// 死亡開始
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
                //other.IsDead = true;
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
            renderer.DrawTexture(name, position * cameraScale + offset, cameraScale, alpha);
            //アニメーションの追加 by長谷川修一
            if(IsAnimation)
            {
                animationPlayer.Draw(gameTime, renderer, position * cameraScale + offset, SpriteEffects.None, cameraScale);
                IsAnimation = animationPlayer.Reset(isShow);
            }
        }

        public override void EventHandle(GameObject other)
        {
            if (other is Player && ((Player)other).FireNum >= Parameter.icefire)
            {
                AliveEvent(other);
                IsAnimation = true;
                WaterLine waterLine = new WaterLine(position, animation);
                if (waters != null)
                    waters.Add(waterLine);
            }
            else if(other.Tag == "Fire")    //火が氷と接触すると消える
            {
                other.IsGoDie = true;
            }
            //葉梨竜太　11/30
            else if(other is Bomb)
            {
                BombEvent(other);
            }
        }
    }
}
