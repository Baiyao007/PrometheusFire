////////////////////////////////////////////////////////////////
// 水クラス
// 作成者：氷見悠人
// 最終更新日 11月30日
// By 葉梨竜太
///////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TeamWorkGame.Device;
using TeamWorkGame.Def;
using TeamWorkGame.Scene;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Actor
{
    public class Water : GameObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="velo"></param>
        /// <param name="type">0：水平　1：垂直</param>
        public Water(Vector2 pos, Vector2 velo, int type = 0) : base("horizontalWater", pos, velo, true, "Water")
        {
            if(type == 1)
            {
                name = "verticalWater";
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void EventHandle(GameObject other)
        {
            if(other is Fire || other is Player)
            {
                other.IsGoDie = true;
            }
            else if(other.Tag == "Sand")
            {
                isDead = true;
            }
            //葉梨竜太　11/30
            else if(other is Bomb)
            {
                BombEvent(other);
            }
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            if (isDead == false)//IsDeadがFalseの間だけ描画
            {
                renderer.DrawTexture(name, position * cameraScale + offset, cameraScale, alpha);
            }
        }
    }
}
