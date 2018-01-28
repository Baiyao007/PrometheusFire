////////////////////////////////////////////////////////////
//マップのゴール
//作成時間：2016/10/1
//作成者：氷見悠人
//最終修正時間：2017/1/11
//ゴールの上の矢印を付けた
//By　葉梨竜太　
////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamWorkGame.Device;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Actor
{
    public enum GoalState
    {
        NONE = 0,
        APPEARING,
        SHOW,
    }

    public class Goal : GameObject
    {
        private Camera camera;
        private bool isOnFire;
        private Sound sound;    //by柏　2016.12.14　ＳＥ実装
        //葉梨竜太
        private Vector2 signpos;
        private Vector2 signvelo;
        private Vector2 startpos;
        private Vector2 endpos;

        //アニメーション
        private AnimationPlayer animePlayer;
        private Animation goalAnime;
        private bool isAnimation = false;
        public bool IsOnFire
        {
            get
            {
                return isOnFire;
            }
            set
            {
                isOnFire = value;
            }
        }

        private bool isComplete;
        public bool IsComplete
        {
            get
            {
                return isComplete;
            }
            set
            {
                isComplete = value;
            }
        }

        private GoalState state;
        public GoalState State
        {
            get
            {
                return state;
            }
        }

        public Goal(Vector2 pos, Sound sound) : base("goal", pos, Vector2.Zero, true, "Goal")
        {
            this.sound = sound;         //by柏　2016.12.14　ＳＥ実装
            state = GoalState.NONE;
            alpha = 0;
            animePlayer = new AnimationPlayer();
            goalAnime = new Animation(Renderer.GetTexture("goalAnime"), 0.6f, true);
        }

        public override void Initialize()
        {
            base.Initialize();
            state = GoalState.NONE;
            alpha = 0;
            isComplete = false;
            isOnFire = false;
            //葉梨竜太
            signpos = position - new Vector2(0, 64);
            startpos = signpos;
            endpos = startpos - new Vector2(0, 32);
            signvelo = new Vector2(0, 1);
        }

        protected override Rectangle InitLocalColRect()
        {
            return new Rectangle(14, 32, 36, 32);
        }

        public void SetCamera(Camera camera)
        {
            this.camera = camera;
        }

        public override void Update(GameTime gameTime)
        {
            //Appear状態に移行
            if (state == GoalState.NONE && isComplete)
            {
                sound.PlaySE("goalAppear");     //by柏　2016.12.14 ＳＥ実装
                state = GoalState.APPEARING;
                camera.SetLimitView(false);
            }
            //Show状態に移行
            else if(state == GoalState.APPEARING)
            {
                alpha += 0.01f;
                if (alpha >= 1.2f)
                {
                    state = GoalState.SHOW;
                    //FuncSwitch.AllAnimetionPause = false;
                    camera.SetLimitView(true);
                    alpha = 1.0f;

                }
            }
            //葉梨竜太
            SignMove();
        }
        
        //葉梨竜太
        /// <summary>
        /// 動く矢印
        /// </summary>
        public void SignMove()
        {
            signpos += signvelo;
            if(signpos.Y >= startpos.Y)
            {
                signvelo = new Vector2(0, -1);
            }
            if(signpos.Y <= endpos.Y)
            {
                signvelo = new Vector2(0, 1);
            }
        }

        public override void EventHandle(GameObject other)
        {
            //Show状態だけ、動く
            if (state == GoalState.SHOW)
            {
                other.Velocity = velocity;
                other.Position = new Vector2(ColRect.Left + ColRect.Width / 2 - other.Width / 2, ColRect.Top - other.ColRect.Height - other.LocalColRect.Top);
                other.IsOnGround = true;
                //other.Alpha = 0.0f;
                if (other is Player)
                {
                    isOnFire = true;
                }
            }
        }

        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            //None状態は描画しない
            if (state == GoalState.APPEARING || state == GoalState.SHOW)
            {
                base.Draw(gameTime, renderer, offset, cameraScale);
                //葉梨竜太
                renderer.DrawTexture("goalsign", signpos*cameraScale+offset,0.7f);
                if (IsOnFire)
                {
                    animePlayer.PlayAnimation(goalAnime);
                    animePlayer.Draw(gameTime, renderer, position * cameraScale + offset, SpriteEffects.None, cameraScale);
                }
            }
                 
        }
    }
}
