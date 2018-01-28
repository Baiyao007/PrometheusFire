///////////////////////////////
//炭のクラス
//作成時間：１０月１３日
//By　佐瀬　拓海
//最終更新日：１月１１日
//炭取得時のエフェクト追加
//By　葉梨竜太
///////////////////////////////
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using TeamWorkGame.Device;
using TeamWorkGame.Def;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Actor
{
    public class Coal : GameObject
    {
        private bool isToDeath;

        //追加　葉梨竜太
        private float gForce;
        private Map map;
        //追加　佐瀬拓海
        private bool isMove;
        private bool prevIsMove;
        private Vector2 coalPos;

        //葉梨竜太
        private Animation animation;
        private AnimationPlayer animePlayer;
        private bool isAnimation = false;
        private Vector2 effpos;

        //長谷川修一
        private float angle; //回転角度
        public Coal(Vector2 pos, Vector2 velo) :
            base("coal", pos, velo, true, "coal")
        {
            //葉梨竜太
            //animePlayer = new AnimationPlayer();
            
        }
        public override void Initialize()
        {
            base.Initialize();
            isToDeath = false;
            //追加　佐瀬拓海
            isMove = false;
            prevIsMove = false;
            coalPos = Parameter.CoalPosition;
            gForce = Parameter.GForce;
            map = MapManager.GetNowMapData();
            //葉梨竜太
            animation = new Animation(Renderer.GetTexture("coaleffect"), 0.1f, false);
            animePlayer.PlayAnimation(animation);
            angle = 0.0f;
        }
        public void ToDeath()
        {
            if (!isToDeath)
            {
                isToDeath = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (isMove == false)//Playerとまだ触れてないとき
            {
                //重力とあたり判定の追加
                //葉梨竜太
                velocity.Y += gForce;

                //マップ上の物と障害物判定
                foreach (var m in map.MapThings.FindAll(x => !x.IsTrigger))
                {
                    ObstacleCheck(m);
                }

                Method.MapObstacleCheck(ref position, localColRect, ref velocity, ref isOnGround, map, new int[] { 1, 2 });

                //地面にいると運動停止
                if (isOnGround)
                {
                    velocity = Vector2.Zero;
                }

                position += velocity;

                effpos = position;

                //マップ上の物と衝突区域判定
                //foreach (var m in map.MapThings.FindAll(x => x.IsTrigger))
                //{
                //    CollisionCheck(m);
                //}
            }
            else//Playerと触れた後
            {
                Move();
                angle += MathHelper.ToRadians(1); //フレーム当たりx度回転
            }
        }

        public void Move()//画面右上のCoal表示まで移動させる
        {
            if (isMove)
            {
                Vector2 distance = position - coalPos;
                velocity = distance;
                velocity.Normalize();
                velocity *= -20;

                position += velocity;

                if (Math.Abs(distance.X) < 10 || Math.Abs(distance.Y) < 10)
                {
                    isDead = true;
                }
            }
        }

        public override void EventHandle(GameObject other)
        {
            if (!isMove)
            {
                if (other is Player)
                {
                    if (((Player)other).FireNum < Parameter.FireMaxNum)//Fireの数を回復
                    {
                        //葉梨竜太
                        ((Player)other).FireNum = ((Player)other).FireNum + 1;

                    }
                    isAnimation = true;
                    isMove = true;
                }
            }
        }
        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)//Playerと触れる前と触れる後の描画処理
        {
            if (prevIsMove == false && isMove == true)
            {
                position += offset;                
            }
            
            if (!isMove)
            {
                base.Draw(gameTime, renderer, offset, cameraScale);
                
            }
            if (isMove)
            {                               
                renderer.DrawTexture(name, position);
            }
            //葉梨竜太
            if (isAnimation)
            {
                Vector2 origin = new Vector2(32, 32);//炭エフェクト、中心座標の変更by長谷川
                //回転するように変更by長谷川
                animePlayer.Draw(gameTime, renderer, (effpos + new Vector2(32, 32)) * cameraScale + offset, SpriteEffects.None, origin, angle, 3.0f, 0.7f);
                if(animePlayer.FrameNow() >=animation.FrameCount-1)
                {
                    isAnimation = false;
                }
            }
            prevIsMove = isMove;
        }
    }
}
