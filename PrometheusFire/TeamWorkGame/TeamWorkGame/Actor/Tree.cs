/////////////////////////////////////////////////
// 木のクラス
// 作成時間：2016年10月12日
// By 長谷川修一
// 最終修正時間：2016年12月21日
// By 長谷川修一
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
    public class Tree : GameObject
    {
        private Timer timer;
        private bool isToDeath;
        //private float scale;
        private Animation animation;
        private AnimationPlayer animationPlayer;
        private bool IsAnimation = false;
        public Tree(Vector2 pos)
            : base("tree", pos, Vector2.Zero, false, "Tree")
        {
            //animationPlayer = new AnimationPlayer();
        }

        public void ToDeath()
        {
            if (!isToDeath)
            {
                isToDeath = true;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            timer = new Timer(Parameter.TreeAnimeTime - Parameter.TreeColTime);
            isToDeath = false;
            isShow = true;
            animation = new Animation(Renderer.GetTexture("treeAnime"), Parameter.TreeAnimeTime / 10, false);
            animationPlayer.PlayAnimation(animation);
            SetTimer(Parameter.TreeColTime);//当たり判定がなくなるまでの時間
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
            renderer.DrawTexture(name, position * cameraScale + offset, cameraScale, alpha);
            if (IsAnimation)
            {
                animationPlayer.Draw(gameTime, renderer, position * cameraScale + offset, SpriteEffects.None, cameraScale);
                IsAnimation = animationPlayer.Reset(isShow);
            }
        }


        public override void EventHandle(GameObject other)
        {

            if (other is Player && ((Player)other).FireNum >= Parameter.treefire)
            {
                DeathEvent(other);
                IsAnimation = true;

            }
        }

        public override void DeathUpdate()
        {
            if (isShow == false)
            {
                deathTimer.Update();
                if (deathTimer.IsTime())
                {
                    alpha = 0.0f;
                    IsTrigger = true;
                    timer.Update();
                    if (timer.IsTime())
                    {
                        IsDead = true;
                    }
                }
            }
        }
    }
}
