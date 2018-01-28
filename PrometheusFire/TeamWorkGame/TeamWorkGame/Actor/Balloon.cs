///作成by　柏
///最後修正by   柏
///最後修正日   2016.12.8
///最後修正項目   2016.12.7新しい仕様書に合わせて修正

using Microsoft.Xna.Framework;
using TeamWorkGame.Device;
using TeamWorkGame.Def;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Actor
{

    public class Balloon : GameObject
    {
        private const int flyLevel = 5; //2016.12.7、新しい仕様書に合わせて修正、定数になる
        private static readonly Point basketSize = new Point(28, 10);   //気球のかごのサイズ
        private Vector2 startPosition;  //気球の初期位置
        private bool playerIsOn;    //playerの乗る状態を保存する
        private Player player;  //player気球に乗る状態をとるように
        private bool stopFlag;  //気球はブロックに当たると止まる
        private Rectangle checkRect;    //ブロックとの当たり範囲
        //private Animation animation;
        //private AnimationPlayer animationPlayer;
        //private bool IsAnimation = false;

        public Balloon(Vector2 pos, Vector2 velo)
            : base("balloon", pos, velo, false, "balloon")
        {
            //animationPlayer = new AnimationPlayer();
            startPosition = pos;    //ステージデータに合わせて、初期位置を設置する
            checkRect = new Rectangle(0, 0, 64, 64);
        }

        /// <summary>
        /// 気球の状態を初期化する
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            //flyLevel = 0;     //2016.12.7、新しい仕様書に合わせて修正、定数になる
            playerIsOn = false; //乗る状態初期化（乗ってない）
            player = null;  //空っぽのplayerを生成する
            isTrigger = true;
        }

        /// <summary>
        /// 気球の状態を更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            AliveUpdate();

            //playerまだ取ってないとき、処理やらない
            if (player == null) { return; }
            BalloonMoveDown();

            //animationPlayer.PlayAnimation(animation);
        }

        /// <summary>
        /// 気球下方向の移動処理
        /// </summary>
        private void BalloonMoveDown()
        {
            //移動速度を3にする
            for (int i = 0; i < Parameter.BalloonMove; i++)
            {
                BalloonGoDownMove();
            }
        }


        /// <summary>
        /// 気球ピクセルずつの移動処理
        /// </summary>
        private void BalloonGoDownMove()
        {
            //playerが乗ってない場合、降りる
            if (playerIsOn) { return; }
            VelocityY = 1;  //下方向
            if (!CanMoveDown()) { VelocityY = 0; }

            BalloonMoveOne();
        }


        /// <summary>
        /// 画面に表示してる場合処理する
        /// </summary>
        /// <param name="other"></param>
        public override void AliveEvent(GameObject other)
        {
            if (other is Player)
            {
                BalloonMoveUp((Player)other);                
            }
        }

        /// <summary>
        /// 気球上方向の移動処理
        /// </summary>
        /// <param name="player"></param>
        private void BalloonMoveUp(Player player)
        {
            //乗ってない場合移動しない
            if (!playerIsOn) { return; }
            //移動速度を3にする
            for (int i = 0; i < Parameter.BalloonMove; i++)
            {
                //毎回移動する前にブロックと当たったかどうかチェックする
                stopFlag = Method.MapObstacleCheck(ref position, checkRect, ref velocity, ref isOnGround, MapManager.GetNowMapData(), new int[] { 1, 2 });

                //当たったら、プレーヤーの位置を設定し、気球はもう移動しない
                if (stopFlag) {
                    player.PositionY= position.Y - basketSize.Y;
                    return;
                }
                BalloonGoUpMove(player);
            }
        }

        /// <summary>
        /// player乗ってる時、気球の状況をチェックして移動
        /// </summary>
        /// <param name="player"></param>
        private void BalloonGoUpMove(Player player)
        {
            VelocityY = -1; //上方向
            if (!CanMoveUp()) { VelocityY = 0; }

            BalloonMoveOne();
            PlayerOnBalloon(player);    //playerは気球の移動に合わせて移動
        }


        /// <summary>
        /// playerIsOnのget,set
        /// </summary>
        public bool IsPlayerOn
        {
            get { return playerIsOn; }
            set { playerIsOn = value; }
        }

        /// <summary>
        /// あたり判定範囲設定
        /// </summary>
        /// <returns></returns>
        protected override Rectangle InitLocalColRect()
        {
            //気球のかご部分は判定部分になってる
            return new Rectangle(18, 54, basketSize.X, basketSize.Y);
        }

        /// <summary>
        /// ピクセルずつ移動
        /// </summary>
        private void BalloonMoveOne()
        {
            PositionY += VelocityY;
        }


        /// <summary>
        /// 気球が上方向移動できるかどうかをチェック
        /// </summary>
        /// <returns></returns>
        private bool CanMoveUp()
        {
            //初期位置戻ったら止まる
            if (startPosition.Y - PositionY >= Height * flyLevel)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 気球が下方向移動できるかどうかをチェック
        /// </summary>
        /// <returns></returns>
        private bool CanMoveDown()
        {
            //上限位置に着いたら止まる
            if (PositionY >= startPosition.Y)
            {
                return false;
            }
            return true;
        }



        /// <summary>
        /// playerは気球に乗る処理
        /// </summary>
        private void PlayerOnBalloon(Player player)
        {
            //playerのＹ座標と気球のy座標を等しくする
            player.PositionY = PositionY - basketSize.Y;
        }


        /// <summary>
        /// 描画の再定義（透明値を追加）　By　氷見悠人　2016/10/20
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="renderer"></param>
        /// <param name="offset"></param>
        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            renderer.DrawTexture(name, position * cameraScale + offset, cameraScale, alpha);
            //if (IsAnimation)
            //{
            //    animationPlayer.Draw(gameTime, renderer, position + offset, SpriteEffects.None);
            //    IsAnimation = animationPlayer.Reset(isShow);
            //}
        }

        /// <summary>
        /// 当たるobjectによっての行動
        /// </summary>
        /// <param name="other"></param>
        public override void EventHandle(GameObject other)
        {
            if (other is Player)
            {
                AliveEvent(other);

                //気球に乗る瞬間、playerと気球の状態を変換する
                if (playerIsOn) { return; }
                other.Position = position - new Vector2(0, basketSize.Y);  //playerを気球のかごに乗せる
                ((Player)other).IsOnBalloon = true;
                ((Player)other).IsOnGround = true;
                playerIsOn = true;

                if (player != null) { return; }
                player = (Player)other;     //playerをとる

                //flyLevel = ((Player)other).FireNum;        //2016.12.7、新しい仕様書に合わせて修正
                //IsAnimation = true;
            }
        }
    }

}
