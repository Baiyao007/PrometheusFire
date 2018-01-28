//最終修正日
//by 長谷川修一 11/2

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TeamWorkGame.Device;
using TeamWorkGame.Actor;
using Microsoft.Xna.Framework.Graphics;

namespace TeamWorkGame.Scene
{
    class FireMeter
    {

        public FireMeter()
        {

        }

        public void Initialize()
        {

        }

        public void Update()
        {

        }

        //数値変更
        //by長谷川修一
        public void Draw(Renderer renderer, GameObject other)
        {
            //renderer.DrawTexture("FireMeter", new Vector2(0, 7 * 64), new Rectangle((((Player)other).FireNum - 1) * 64, 0, 64, 192));
            renderer.DrawTexture("FireMeter", new Vector2(224 + 32, 0 + 16), new Rectangle((((Player)other).FireNum - 1) * 64, 0, 64, 192), SpriteEffects.None, 1.0f, MathHelper.ToRadians(90));
            //renderer.DrawNumber("number", new Vector2(0 + 16, 10 * 64), ((Player)other).FireNum);
            renderer.DrawNumber("number", new Vector2(26,20), ((Player)other).FireNum);
        }
    }
}
