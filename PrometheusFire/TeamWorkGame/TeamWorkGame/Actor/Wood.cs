/////////////////////////////
//木材クラス
//最終修正時間:2017年01月25日
//by 氷見悠人　Bug修正
/////////////////////////////
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
    public class Wood : GameObject
    {
        private Animation animation;
        private AnimationPlayer animationPlayer;
        private bool IsAnimation = false;

        public Wood(Vector2 pos)
            : base("wood", pos, Vector2.Zero, false, "Wood")
        {
            //animationPlayer = new AnimationPlayer();
        }

        public override void Initialize()
        {
            base.Initialize();
            isShow = true;
            animation = new Animation(Renderer.GetTexture("woodAnime"), Parameter.WoodAnimeTime / 3, false);
            animationPlayer.PlayAnimation(animation);
            SetTimer(Parameter.WoodAnimeTime, Parameter.WoodSpawnTime);
        }

        public override void Update(GameTime gameTime)
        {
            AliveUpdate();
            
        }

        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
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
            //葉梨竜太 11/30
            if (other is Bomb)
            {
                BombEvent(other);
            }
            else
            {
                AliveEvent(other);
                
            }
        }

        public override void AliveEvent(GameObject other)
        {
            if (other is Fire)
            {
                //other.IsDead = true;
                isShow = false;
                IsAnimation = true;
            }
            if (other is Player && ((Player)other).FireNum >= Parameter.woodfire)
            {
                isShow = false;
                IsAnimation = true;
            }
            spawnTimer.Initialize();
        }
    }
}
