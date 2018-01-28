////////////////////////////////////////////////////////////////////////////////////////////////////////////
// 松明クラス
// 作成者：氷見悠人
/////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TeamWorkGame.Device;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Actor
{
    public class Light : GameObject
    {
        private bool isOn;

        public bool IsOn
        {
            get
            {
                return isOn;
            }
            set
            {
                IsOn = value;
            }
        }

        public Light(Vector2 pos, bool isOn = false) : base("light_off", pos, Vector2.Zero, true, "Light")
        {
            this.isOn = isOn;
        }
       
        public override void Initialize()
        {

            base.Initialize();
        }

        protected override Rectangle InitLocalColRect()
        {
            return new Rectangle(25, 34, 14, 30);
        }

        public override void Update(GameTime gameTime)
        {
            isOn = false;
        }

        public override void EventHandle(GameObject other)
        {
            if (other is Fire || other is Player)
            {
                if (!isOn)
                {
                    other.Velocity = velocity;
                    other.Position = new Vector2(ColRect.Left + ColRect.Width / 2 - other.Width / 2, ColRect.Top - other.ColRect.Height - other.LocalColRect.Top);
                    other.IsOnGround = true;
                    isOn = true;
                }
            }
        }
    }
}
