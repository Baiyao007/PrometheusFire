/////////////////////////////////////////////////
// 藁のクラス
// 作成時間：2016年10月12日
// By 長谷川修一
// 最終修正時間：2017年01月25日
// By 氷見悠人　Bug修正
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
    public class Straw : GameObject
    {
        private Timer timer;
        private Timer burnTimer;
        private bool isToDeath;
        private Animation animation;
        private AnimationPlayer animationPlayer;
        private Map map;

        public Straw(Vector2 pos)
            : base("straw", pos, Vector2.Zero, false, "Straw")
        {
            
        }


        public override void Initialize()
        {
            base.Initialize();
            timer = new Timer(Parameter.StrawAnimeTime - Parameter.StrawColTime);
            burnTimer = new Timer(Parameter.StrawBurn);
            isToDeath = false;
            isShow = true;
            map = MapManager.GetNowMapData();
            animation = new Animation(Renderer.GetTexture("strawAnime"), Parameter.StrawAnimeTime / 3, false);
            //animationPlayer = new AnimationPlayer();
            animationPlayer.PlayAnimation(animation);
            SetTimer(Parameter.StrawColTime);//当たり判定が消え始めるまでの時間
        }

        public bool IsToDeath
        {
            get
            {
                return isToDeath;
            }
        }

        public override void Update(GameTime gameTime)
        {
            DeathUpdate();
        }

        /// <summary>
        /// 描画の再定義（Scale追加）　by　氷見悠人　2016/10/20
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="renderer"></param>
        /// <param name="offset"></param>
        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            if (isShow)
            {
                renderer.DrawTexture(name, position * cameraScale + offset, cameraScale, alpha);
            }
            else
            {
                animationPlayer.Draw(gameTime, renderer, position * cameraScale + offset, SpriteEffects.None, cameraScale);
                //IsAnimation = animationPlayer.Reset(isShow);
            }
        }

        public override void EventHandle(GameObject other)
        {
            //葉梨竜太 11/30
            if (other is Bomb)
            {
                BombEvent(other);
            }
            else
            {
                DeathEvent(other);
            }
        }

        //spawnTimerで復活 by長谷川修一
        public override void DeathUpdate()
        {
            if (isToDeath)
            {
                burnTimer.Update();
                timer.Update();

                if (burnTimer.IsTime())
                {
                    burnTimer.Initialize();
                    burnTimer.Stop();
                    Rectangle col = new Rectangle(ColRect.X - 1, ColRect.Y - 1, ColRect.Width + 2, ColRect.Height + 2);
                    CheckOhterStraw(col);
                }

                if (timer.IsTime())
                {
                    IsDead = true;
                }

                return;
            }

            if (isShow == false)
            {
                deathTimer.Update();
                if (deathTimer.IsTime())
                { 
                    isTrigger = true;
                    isToDeath = true;
                }
            }
        }

        public void CheckOhterStraw(Rectangle col)
        {
            foreach (var m in map.MapThings)
            {
                if (m is Straw)
                {
                    if (!((Straw)m).IsToDeath)
                    {
                        if (col.Intersects(m.ColRect))
                        {
                            m.EventHandle(this);
                        }
                    }
                }
            }
        }

        public override void DeathEvent(GameObject other)
        {
            if ((other is Fire) || (other is Player && ((Player)other).FireNum >= Parameter.strawfire)) 
            {
                isShow = false;
            }
            if (other is Straw)
            {
                if (((Straw)other).IsToDeath)
                {
                    isShow = false;
                }
            }
        }
    }
}
