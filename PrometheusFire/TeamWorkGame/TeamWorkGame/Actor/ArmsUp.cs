using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TeamWorkGame.Device;
using TeamWorkGame.Def;
using Microsoft.Xna.Framework.Graphics;

namespace TeamWorkGame.Actor
{
    //ClearSelect画面でアニメーションするためだけのクラス
    //by長谷川
    class ArmsUp : GameObject
    {
        private AnimationPlayer animePlayer;
        private Animation armsUpAnime;

        public ArmsUp(Vector2 pos, Vector2 velo) : base("hero", pos, velo, false, "hero")
        {
            //animePlayer = new AnimationPlayer();
            armsUpAnime = new Animation(Renderer.GetTexture("armsUpAnime"), 1.0f, true);
            animePlayer.PlayAnimation(armsUpAnime);
        }

        public override void EventHandle(GameObject other)
        {

        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            animePlayer.Draw(gameTime, renderer, position, SpriteEffects.None, 2.0f);
        }
    }
}
