/////////////////////////////////////////////////
// 看板クラス
// 作成時間：2016年11月30日
// By 佐瀬拓海
// 最終修正時間：2016年11月30日
// By 佐瀬拓海
/////////////////////////////////////////////////
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamWorkGame.Device;
using TeamWorkGame.Def;

namespace TeamWorkGame.Actor
{
    class Sign : GameObject
    {
        private int tutorial;
        public Sign(Vector2 pos,int tutorial)
            : base("sign", pos, Vector2.Zero, true, "Sign")
        {
            this.tutorial = tutorial;
        }
        public override void Initialize()
        {
            base.Initialize();

        }
        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            switch (tutorial)
            {
                case (int)GimmickType.JSIGN:
                    renderer.DrawTexture("jump", new Vector2(position.X - image.Width / 2, position.Y - image.Height) * cameraScale + offset, cameraScale, alpha);
                    break;
                case (int)GimmickType.MSIGN:
                    renderer.DrawTexture("move", new Vector2(position.X - image.Width / 2, position.Y - image.Height) * cameraScale + offset, cameraScale, alpha);
                    break;
                case (int)GimmickType.RSIGN:
                    renderer.DrawTexture("slow", new Vector2(position.X - image.Width / 2, position.Y - image.Height) * cameraScale + offset, cameraScale, alpha);
                    break;
                case (int)GimmickType.LSIGN:
                    renderer.DrawTexture("change", new Vector2(position.X - image.Width / 2, position.Y - image.Height) * cameraScale + offset, cameraScale, alpha);
                    break;
                case (int)GimmickType.CSIGN:
                    renderer.DrawTexture("camerascroll", new Vector2(position.X - image.Width / 2, position.Y - image.Height) * cameraScale + offset, cameraScale, alpha);
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            
        }
        public override void EventHandle(GameObject other)
        {

        }
    }
}
