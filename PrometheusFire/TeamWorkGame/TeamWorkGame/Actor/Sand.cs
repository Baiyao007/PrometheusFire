///////////////////////////////
//砂のクラス
//作成時間：１１月０２日
//By　佐瀬　拓海
//最終更新日：1月26日
//By　氷見悠人　砂が落下中と当たり判定しない
///////////////////////////////
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TeamWorkGame.Device;
using TeamWorkGame.Def;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Actor
{
    public class Sand : GameObject
    {
        private float gForce;
        private Map map;

        public Sand(Vector2 pos, Vector2 velo) :
            base("sand", pos, velo, false, "Sand")
        {

        }

        public override void Initialize()
        {
            base.Initialize();
            gForce = Parameter.GForce;
            map = MapManager.GetNowMapData();
        }

        protected override Rectangle InitLocalColRect()
        {
            return base.InitLocalColRect();
        }

        public override void Update(GameTime gameTime)
        {
            velocity.Y += gForce;

            if(velocity.Y < 1 && velocity.Y > 0)
            {
                velocity.Y = 1;
            }

            foreach (var m in map.MapThings.FindAll(x => !x.IsTrigger && x != this))
            {
                ObstacleCheck(m);
            }

            Method.MapObstacleCheck(ref position, localColRect, ref velocity, ref isOnGround, map, new int[] { 1, 2 });

            //地面にいると運動停止
            if (isOnGround)
            {
                velocity = Vector2.Zero;
                isTrigger = false;
            }
            else
            {
                isTrigger = true;
            }

            position += velocity;

            //マップ上の物と衝突区域判定
            foreach (var m in map.MapThings.FindAll(x => x.Tag == "Water"))
            {
                if (CollisionCheck(m))
                {
                    m.EventHandle(this);
                }
            }

        }
        public override void EventHandle(GameObject other)
        {
            //葉梨竜太　11/30
            if(other is Bomb)
            {
                BombEvent(other);
            }
        }
    }
}
