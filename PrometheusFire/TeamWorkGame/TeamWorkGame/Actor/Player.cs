/////////////////////////////////////////////////
// プレーヤーのクラス
// 作成時間：2016年9月24日
// By 氷見悠人
// 最終修正時間：2017年1月11日
// 火の飛ぶ方向を上下左右に　By 葉梨竜太
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;  //Vector2
using Microsoft.Xna.Framework.Graphics; //spriteBatch
using Microsoft.Xna.Framework.Input;
using TeamWorkGame.Device;    //入力状態クラス
using TeamWorkGame.Def;
using TeamWorkGame.Utility;


namespace TeamWorkGame.Actor
{
    class Player : GameObject
    {
        // フィールド
        private GameDevice gameDevice;
        private InputState inputState;          //入力管理
        private Sound sound;
        private Map map;
        private float gForce;
        private Direction diretion;             //向いている方向
        private List<Fire> firesList;               //投げ出した火
        private List<WaterLine> watersList;         //滝のリスト

        private int firstFireNum;                    //火の総数
        private int fireNum;                        //持っている火の数
        private Animation standAnime;           //待機アニメ、正面
        private Animation sidewaysAnime;            //待機アニメ、横
        private Animation runAnime;             //走るアニメ
        private Animation throwAnime;           //投げるアニメ
        private Animation deathAnime;           //死ぬアニメ
        private Animation lowRunAnime;     //走るアニメ、弱い
        private Animation lowSidewaysAnime;    //待機アニメ、横、弱い
        private Animation lowStandAnime; //正面アニメ、弱い
        private Animation lowDeathAnime; //死ぬアニメ、弱い

        private Timer jumpEffectTimer;
        private Timer fallEffectTimer;
        private Vector2 jumpEffectPos;
        private Vector2 fallEffectPos;
        private bool previousIsOnGround;
        private bool isClear;

        private AnimationPlayer animePlayer;    //アニメ再生器
        private SpriteEffects flip = SpriteEffects.FlipHorizontally;
        private PlayerMotion playerMotion;

        private Vector2 aim; //エイムの向き
        private Vector2 aimpos;
        private Vector2 armsUpPos; //ばんざいする位置

        private bool isOnBalloon;   //気球に乗ってるかどうか

        private bool isView;                        //カメラ操作中か？

        private List<FireDust> fireDustList;
        private Timer makeFireDustTimer;

        private FireEnergy fireEnergy;

        private Camera camera;

        private bool isthrow;
        public bool IsView
        {
            get
            {
                return isView;
            }
            set
            {
                isView = value;
            }
        }

        public int FireNum
        {
            get
            {
                return fireNum;
            }
            set
            {
                fireNum = value;
            }
        }

        //new Size(50,52)の中身を(64,64)に修正
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="input"></param>
        /// <param name="position">位置</param>
        /// <param name="velocity">移動量</param>
        /// <param name="fires">投げ出した火のList、書き出す</param>
        public Player(GameDevice gameDevice, Vector2 position, Vector2 velocity, ref List<Fire> firesList, ref List<WaterLine> watersList, bool isView)
            : base("hero", position, velocity, true, "Player")
        {
            this.gameDevice = gameDevice;
            inputState = gameDevice.GetInputState();
            sound = gameDevice.GetSound();
            this.firesList = firesList;
            this.watersList = watersList;
            this.isView = isView;
            playerMotion = PlayerMotion.STAND;
            
        }

        public void SetCamera(Camera ca)
        {
            camera = ca;
        }

        //当たり判定変更by長谷川修一
        protected override Rectangle InitLocalColRect()
        {
            return new Rectangle(8, 22, 49, 42);
        }

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            map = MapManager.GetNowMapData();
            gForce = Parameter.GForce;

            diretion = Direction.RIGHT;
            firstFireNum = Parameter.FirstFireNum;
            fireNum = firstFireNum;
            runAnime = new Animation(Renderer.GetTexture("playerAnime"), 0.1f, true);
            standAnime = new Animation(Renderer.GetTexture("standAnime"), 0.1f, true);
            sidewaysAnime = new Animation(Renderer.GetTexture("sideAnime"), 0.1f, true);
            throwAnime = new Animation(Renderer.GetTexture("throwAnime"), 0.1f, false);
            deathAnime = new Animation(Renderer.GetTexture("deathAnime"), 0.1f, false);
            lowRunAnime = new Animation(Renderer.GetTexture("lowRunAnime"), 0.1f, true);
            lowSidewaysAnime = new Animation(Renderer.GetTexture("lowSideAnime"), 0.1f, true);
            lowStandAnime = new Animation(Renderer.GetTexture("lowStandAnime"), 0.1f, true);
            lowDeathAnime = new Animation(Renderer.GetTexture("lowDeathAnime"), 0.1f, false);

            isClear = false;
            isOnBalloon = false;
            playerMotion = PlayerMotion.STAND;
            animePlayer.PlayAnimation(standAnime);

            jumpEffectTimer = new Timer(0.15f);
            jumpEffectTimer.CurrentTime = 0;
            fallEffectTimer = new Timer(0.15f);
            fallEffectTimer.CurrentTime = 0;
            aim = new Vector2(Parameter.FireSpeed, 0);
            armsUpPos = new Vector2(560, 210);
            //FireDust
            fireDustList = new List<FireDust>();
            makeFireDustTimer = new Timer(0.1f);

            //FireEnergy
            fireEnergy = new FireEnergy(position, 50);

            isthrow = true;
        }

        /// <summary>
        /// 火の粉を生成
        /// </summary>
        private void MakeFireDust()
        {
            if (fireNum <= 0)
            {
                return;
            }
            if (velocity.LengthSquared() > 1)
            {
                if (makeFireDustTimer.IsTime())
                {
                    fireDustList.Add(new FireDust(position, Vector2.Zero, fireNum));
                    makeFireDustTimer.Initialize();
                }
            }
            else
            {
                makeFireDustTimer.Initialize();
            }
        }

        /// <summary>
        /// 火の粉の更新処理
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateFireDust(GameTime gameTime)
        {
            makeFireDustTimer.Update();
            fireDustList.RemoveAll(x => x.IsDead);
            MakeFireDust();
            foreach (var f in fireDustList)
            {
                f.Update(gameTime);
            }
        }

        /// <summary>
        /// 火の粉の描画処理
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="renderer"></param>
        /// <param name="offset"></param>
        /// <param name="cameraScale"></param>
        private void DrawFireDust(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            foreach (var f in fireDustList)
            {
                f.Draw(gameTime, renderer, offset, cameraScale);
            }
        }

        /// <summary>
        /// 立つ状態の判断（弱い火）
        /// </summary>
        private void LowStand()
        {
            if (diretion == Direction.UP || diretion == Direction.DOWN) //葉梨竜太　Down追加
            {
                animePlayer.PlayAnimation(lowStandAnime);
                playerMotion = PlayerMotion.LOWSTAND;
            }
        }

        /// <summary>
        /// 立つ状態の判断
        /// </summary>
        private void Stand()
        {
            if (diretion == Direction.UP || diretion == Direction.DOWN)//葉梨竜太　Down追加// && !IsThrowing()) 
            {
                animePlayer.PlayAnimation(standAnime);
                playerMotion = PlayerMotion.STAND;
            }
        }
        /// <summary>
        /// 横向きの状態の判断（弱い火）
        /// </summary>
        private void LowStandSideWays()
        {
            if ((diretion == Direction.LEFT || diretion == Direction.RIGHT))
            {
                //仮実装by長谷川
                animePlayer.PlayAnimation(lowSidewaysAnime);
                playerMotion = PlayerMotion.SIDEWAYS;
            }
        }

        /// <summary>
        /// 横向きの状態の判断
        /// </summary>
        private void StandSideWays()
        {
            if ((diretion == Direction.LEFT || diretion == Direction.RIGHT))// && !IsThrowing())
            {
                animePlayer.PlayAnimation(sidewaysAnime);
                playerMotion = PlayerMotion.SIDEWAYS;
            }
        }

        /// <summary>
        /// 走る状態の判断（弱い火）
        /// </summary>
        private void LowRun()
        {
            if ((diretion == Direction.LEFT || diretion == Direction.RIGHT))// && !IsThrowing())
            {
                animePlayer.PlayAnimation(lowRunAnime);
                playerMotion = PlayerMotion.LOWRUN;
            }
        }

        /// <summary>
        /// 走る状態の判断
        /// </summary>
        private void Run()
        {
            if (diretion == Direction.LEFT || diretion == Direction.RIGHT)// && !IsThrowing())
            {
                animePlayer.PlayAnimation(runAnime);
                playerMotion = PlayerMotion.RUN;
            }
        }

        private void LowDeath()
        {
            sound.PlaySE("dead");
            animePlayer.PlayAnimation(lowDeathAnime);
            playerMotion = PlayerMotion.DEATH;
        }

        public void Death()
        {
            sound.PlaySE("dead");   //by柏　2016.12.14 ＳＥ実装
            animePlayer.PlayAnimation(deathAnime);
            playerMotion = PlayerMotion.DEATH;
        }

        /// <summary>
        /// 火を投げる
        /// </summary>
        private void ThrowFire()
        {
            //投げ出した火の位置と速度を計算（初期位置は自身とぶつからないように）
            //Speedを固定にした
            //上下左右にした


            isthrow = true;

            if (diretion == Direction.UP || inputState.CheckDownKey(Keys.Up, Buttons.LeftThumbstickUp))
            {
                aim = new Vector2(0, -Parameter.FireSpeed);
                aimpos = new Vector2(position.X, position.Y - 48);
            }
            else if (diretion == Direction.DOWN || inputState.CheckDownKey(Keys.Down, Buttons.LeftThumbstickDown))
            {
                aim = new Vector2(0, Parameter.FireSpeed);
                aimpos = new Vector2(position.X, position.Y + 60);
                isthrow = isOnGround ? false : true;
            }
            else if (diretion == Direction.LEFT || inputState.CheckDownKey(Keys.Left, Buttons.LeftThumbstickLeft))
            {
                aim = new Vector2(-Parameter.FireSpeed, 0);
                aimpos = new Vector2(position.X-50, position.Y);
            }
            else if (diretion == Direction.RIGHT || inputState.CheckDownKey(Keys.Right, Buttons.LeftThumbstickRight))
            {
                aim = new Vector2(Parameter.FireSpeed, 0);
                aimpos = new Vector2(position.X+50, position.Y);
            }

            if (fireNum > 0)
            {
                if (inputState.CheckTriggerKey(Parameter.ThrowKey, Parameter.ThrowButton))
                {
                    if (isthrow)
                    {
                        
                        Vector2 fireVelo = aim;
                        Vector2 firePos = aimpos;

                        Fire fire = new Fire(firePos, fireVelo, watersList);
                        //葉梨竜太
                        //単位ベクトル化
                        fireVelo.Normalize();
                        fire.Velocity = fireVelo * Parameter.FireSpeed;
                        //葉梨竜太
                        fire.SetStartPos();

                        firesList.Insert(0, fire);
                        fireNum--;
                    }


                    //投げる状態に入る
                    animePlayer.PlayAnimation(throwAnime);
                    playerMotion = PlayerMotion.THROW;
                    sound.PlaySE("fire1");
                }
                aim.Normalize();
                aimpos.X = aimpos.X + (Parameter.FireFly * 64 * aim.X);
                aimpos.Y = aimpos.Y + (Parameter.FireFly * 64 * aim.Y);
                AimCheck();
            }
        }

        /// <summary>
        /// エイムがマップ内か？
        /// </summary>
        public void AimCheck()
        {
            Vector2 aimpos2 = aimpos + camera.OffSet;

            if (aimpos2.Y > Parameter.ScreenHeight)
            {
                aimpos2.Y = Parameter.ScreenHeight - 64;
            }
            if (aimpos2.X + 64 > Parameter.ScreenWidth)
            {
                aimpos2.X = Parameter.ScreenWidth - 64;
            }
            if (aimpos2.Y < 0)
            {
                aimpos2.Y = 0;
            }
            if (aimpos2.X < 0)
            {
                aimpos2.X = 0;
            }

            aimpos = aimpos2 - camera.OffSet;
        }

        /// <summary>
        /// マップ上いある火と位置交換（最後に投げ出した火は最初に交換）
        /// </summary>
        private void Teleport()
        {
            if (firesList.Count > 0)
            {
                if (inputState.CheckTriggerKey(Parameter.TeleportKey, Parameter.TeleportButton))
                {
                    Vector2 tempPos = firesList[0].Position;
                    Vector2 tempVelo = firesList[0].Velocity;

                    firesList[0].Position = position;
                    firesList[0].Velocity = velocity;

                    position = tempPos;
                    velocity = tempVelo;

                    Fire tempfire = firesList[0];
                    firesList.RemoveAt(0);
                    firesList.Add(tempfire);
                    //葉梨竜太
                    tempfire.Velocity = new Vector2(0, Parameter.FireFall);
                }
            }
        }

        /// <summary>
        /// プレイヤーの移動系の処理
        /// </summary>
        private void MoveMotion()
        {
            Stand();
            if (velocity.X != 0)
            {
                Run();
            }
            else
            {
                StandSideWays();
            }
        }

        /// <summary>
        /// プレイヤーの移動系の処理（弱い）
        /// </summary>
        private void LowMoveMotion()
        {
            LowStand();
            if (velocity.X != 0)
            {
                LowRun();
            }
            else
            {
                LowStandSideWays();
            }
        }

        /// <summary>
        /// 衝突区域判定
        /// </summary>
        /// <param name="other">対象</param>
        /// <returns></returns>
        public override bool CollisionCheck(GameObject other)
        {
            bool flag = false;

            if (other.IsTrigger)
            {
                flag = base.CollisionCheck(other);
                if (flag)
                {
                    other.EventHandle(this);
                }
            }

            return flag;
        }

        /// <summary>
        /// 障害物判定
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool ObstacleCheck(GameObject other)
        {
            bool flag = false;
            if (!other.IsTrigger)
            {
                flag = base.ObstacleCheck(other);
                if (flag)
                {
                    other.EventHandle(this);
                }
                else
                {
                    if (other is Balloon)
                    {
                        ((Balloon)other).IsPlayerOn = false;
                    }
                }
            }

            return flag;
        }

        /// <summary>
        /// 地図の下に落ちたか？
        /// </summary>
        private void CheckIsOut()
        {
            if (position.Y > map.MapHeight + 64)
            {
                isDead = true;
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (isGoDie)
            {
                if (playerMotion != PlayerMotion.DEATH)
                {
                    if (FireNum == 0)
                    {
                        LowDeath();
                    }
                    else
                    {
                        Death();
                    }
                }

                if (animePlayer.FrameIndex >= deathAnime.FrameCount - 1)
                {
                    isDead = true;
                }
                return;
            }

            //カメラ操作中は主人公に対する操作不可
            if (isView)
            {
                inputState = new InputState();
            }
            else
            {
                inputState = gameDevice.GetInputState();
            }

            fallEffectTimer.Update();
            jumpEffectTimer.Update();

            if (isOnGround)
            {
                if (inputState.CheckTriggerKey(Parameter.JumpKey, Parameter.JumpButton))
                {
                    velocity.Y = -Parameter.PlayerJumpPower;
                    //isOnGround = false;
                    jumpEffectTimer.Initialize();
                    jumpEffectPos = position;
                }

                //isOnGroundをReset
                isOnGround = false;
            }

            //横方向のスピード計算
            velocity.X = inputState.Velocity().X * Parameter.MaxPlayerHorizontalSpeed;

            //方向判断
            if (velocity.X > 0 || inputState.Velocity().X > 0)
            {
                diretion = Direction.RIGHT;
            }
            else if (velocity.X < 0 || inputState.Velocity().X < 0)
            {
                diretion = Direction.LEFT;
            }
            else if (inputState.Velocity().Y < 0)
            {
                diretion = Direction.UP;
            }
            else if (inputState.Velocity().Y > 0)//葉梨竜太
            {
                diretion = Direction.DOWN;
            }

            //縦スピード計算
            velocity.Y += gForce;

            if (velocity.Y > 0 && velocity.Y < 1)
            {
                velocity.Y = 1;
            }

            if (velocity.Y > Parameter.MaxPlayerVerticalSpeed)
            {
                velocity.Y = 10;
            }


            //マップ上の物と障害物判定
            foreach (var m in map.MapThings.FindAll(x => !x.IsTrigger))
            {
                ObstacleCheck(m);
            }
            Method.MapObstacleCheck(ref position, localColRect, ref velocity, ref isOnGround, map, new int[] { 1, 2 });


            //Console.WriteLine(velocity);

            //位置計算
            position += velocity;

            //位置を整数にする　By氷見悠人
            position.X = (float)Math.Round(position.X);
            position.Y = (float)Math.Round(position.Y);

            previousBottom = ColRect.Bottom;

            //マップ上の物と衝突区域判定
            foreach (var m in map.MapThings.FindAll(x => x.IsTrigger))
            {
                CollisionCheck(m);
            }

            //火と衝突判定
            foreach (var f in firesList)
            {
                CollisionCheck(f);
            }

            //滝との衝突判定
            foreach (var wl in watersList)
            {
                foreach (var w in wl.Waters)
                    CollisionCheck(w);
            }

            CheckIsOut();
            if (!previousIsOnGround && isOnGround)
            {
                fallEffectTimer.Initialize();
                fallEffectPos = position;
            }
            previousIsOnGround = isOnGround;
            //Console.WriteLine("isOnGround: " + isOnGround);

            if (!IsThrowing())
            {
                if (FireNum != 0)
                {
                    //移動モーション関係はメソッド化by長谷川 12/15
                    MoveMotion();
                }
                else
                {
                    LowMoveMotion();
                }
            }
            //火と位置交換処理
            Teleport();

            ThrowFire();

            //火の粉の更新処理
            UpdateFireDust(gameTime);

            if (playerMotion == PlayerMotion.THROW)
            {
                ResetAnimation(throwAnime);
            }

            //地図外か？
            CheckIsOut();

            if (!previousIsOnGround && isOnGround)
            {
                fallEffectTimer.Initialize();
                fallEffectPos = position;
            }
            previousIsOnGround = isOnGround;
            //Console.WriteLine("isOnGround: " + isOnGround);

            fireEnergy.SetFireNum(fireNum);
            fireEnergy.SetCenterPosition(position);
            fireEnergy.Update(gameTime);
        }

        public bool GetState()
        {
            return isDead;
        }

        public bool IsOnBalloon
        {
            get { return isOnBalloon; }
            set { isOnBalloon = value; }
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer"></param>
        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            if (!isClear)
            {
                DrawFireDust(gameTime, renderer, offset, cameraScale);
                //必要ないっぽいのでコメントアウトby長谷川

                if (!IsDeath() && fireNum > 0)//葉梨竜太
                {
                    renderer.DrawTexture("aiming", aimpos * cameraScale + offset);
                    fireEnergy.Draw(gameTime, renderer, offset, cameraScale);
                }

                if (diretion == Direction.RIGHT)
                    flip = SpriteEffects.FlipHorizontally;
                else if (diretion == Direction.LEFT)
                    flip = SpriteEffects.None;
                animePlayer.Draw(gameTime, renderer, position * cameraScale + offset, flip, cameraScale);

                //JumpのEffect描画
                if (!fallEffectTimer.IsTime())
                {
                    renderer.DrawTexture("JumpEffect", fallEffectPos * cameraScale + offset, new Rectangle(0, 0, 64, 64), cameraScale);
                }
                if (!jumpEffectTimer.IsTime())
                {
                    Rectangle rect;
                    if (velocity.X >= 0)
                    {
                        rect = new Rectangle(64 * 2, 0, 64, 64);
                    }
                    else
                    {
                        rect = new Rectangle(64, 0, 64, 64);
                    }
                    renderer.DrawTexture("JumpEffect", jumpEffectPos * cameraScale + offset, rect, cameraScale);
                }
            }
        }

        public override void EventHandle(GameObject other)
        {
            //
        }

        ///// <summary>
        ///// 投げている状態ならtrue
        ///// </summary>
        ///// <returns></returns>
        public bool IsThrowing()
        {
            return playerMotion == PlayerMotion.THROW;
        }

        /// <summary>
        /// 死んだ状態ならtrue
        /// </summary>
        /// <returns></returns>
        public bool IsDeath()
        {
            return playerMotion == PlayerMotion.DEATH;
        }

        /// <summary>
        /// ループしないアニメーションのリセット
        /// </summary>
        /// <param name="animation">リセットしたいアニメーション</param>
        public void ResetAnimation(Animation animation)
        {
            if (animePlayer.FrameNow() == animePlayer.Animation.FrameCount - 1)
            {
                if (FireNum != 0)
                {
                    MoveMotion();
                }
                else
                {
                    LowMoveMotion();
                }
                animePlayer.ResetAnimation(animation);
            }
        }

        public bool IsClear
        {
            get { return isClear; }
            set { isClear = value; }
        }

    }
}
