//////////////////////////////
//爆弾クラス
//作成者　葉梨竜太
//作成日時　11/30
//最後修正日　2016.12.14
//修正者 氷見悠人　爆弾のいろいろ修正
//////////////////////////////
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
    public class Bomb : GameObject
    {
        private Map map;
        protected Sound sound;       //2016.12.14、柏
        private float gForce;       //2016.12.21　佐瀬
        private Animation bombEffect;
        private AnimationPlayer animationPlayer;
        private bool IsAnimation = false;
        private Vector2 effectPosition; //エフェクト用の位置
        private Vector2 randOffset;
        private static Random rnd = new Random();
        private float flashTimer;

        public Bomb(Vector2 pos, Sound sound, Vector2 velo)
            :base("bomb",pos, velo, false,"Bomb")
        {
            this.sound = sound;
        }

        public override void Initialize()
        {
            base.Initialize();
            //scale = 1.0f;
            isShow = true;
            map = MapManager.GetNowMapData();
            gForce = Parameter.GForce;
            bombEffect = new Animation(Renderer.GetTexture("bombEffect"), Parameter.BombAnimeTime / 7, false);
            //animationPlayer = new AnimationPlayer();
            animationPlayer.PlayAnimation(bombEffect);
            effectPosition = Vector2.Zero;
            SetTimer(Parameter.BombColTime);
            flashTimer = 0.0f;
            randOffset = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            if(!isShow)
            {
                DeathUpdate();

                if (flashTimer <= 0.0f)
                {
                    randOffset = new Vector2(rnd.Next(-1, 1), rnd.Next(-1, 1));
                    randOffset *= 5f;
                    flashTimer = 0.05f;
                }
                else
                {
                    flashTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }


            if (IsAnimation)
            {
                return;
            }

            //重力を付ける
            velocity.Y += gForce;

            if (!IsTrigger)
            {
                //マップ上の物と障害物判定
                foreach (var m in map.MapThings.FindAll(x => !x.IsTrigger && x != this))
                {
                    ObstacleCheck(m);
                }
                Method.MapObstacleCheck(ref position, localColRect, ref velocity, ref isOnGround, map, new int[] { 1, 2 });
            }
            
            //地面にいると運動停止 //アニメーションが始まったら運動停止by長谷川
            if (isOnGround)
            {
                velocity = Vector2.Zero;
            }

            position += velocity;

            

            ////マップ上の物と衝突区域判定
            //foreach (var m in map.MapThings.FindAll(x => x.IsTrigger))
            //{
            //    CollisionCheck(m);
            //}
        }
        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            if(IsAnimation)
            {
                animationPlayer.Draw(gameTime, renderer, effectPosition * cameraScale + offset, SpriteEffects.None, cameraScale);
                //IsAnimation = animationPlayer.Reset(isShow);
                if (animationPlayer.IsEnd())
                {
                    isDead = true;
                }
            }
            else
            {
                renderer.DrawTexture(name, position * cameraScale + randOffset + offset, cameraScale, 1.0f);
            }
        }

        public override void EventHandle(GameObject other)
        {
            if (other is Fire)
            {
                isShow = false;
            }
            if (other is Player)
            {
                isShow = false;
            }
            if (other is Igniter)
            {
                isShow = false;
            }
        }

        //by長谷川 1/11
        public override void DeathUpdate()
        {
            //DeaathEventが発生したら
            //タイマー更新
            deathTimer.Update();
            if (deathTimer.IsTime())
            {
                if (isOnGround)
                {
                    deathTimer.Initialize();
                    deathTimer.Stop();
                    Explosion();
                }
            }
        }

        /// <summary>
        /// 爆発
        /// </summary>
        public void Explosion()
        {
            //エフェクト用のポジション取得 by長谷川
            effectPosition = new Vector2(position.X - 64, position.Y - 64);
            
            //アニメーションを開始
            IsAnimation = true;

            //当たり判定変更
            isTrigger = true;
            //当たり判定範囲拡大
            localColRect.Offset(-1, -1);
            localColRect.Width += 2;
            localColRect.Height += 2;

            //SE
            sound.PlaySE("bomb1");

            //爆発処理
            foreach (var m in map.MapThings)
            {
                if (ColRect.Intersects(m.ColRect))
                {
                    m.EventHandle(this);
                }
            }
        }
    }
}
