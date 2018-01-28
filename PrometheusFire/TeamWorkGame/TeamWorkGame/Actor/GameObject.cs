////////////////////////////////////////////////////////////////////
//マップ上にある物の親クラス
//作成時間：2016/9/23
//作成者：氷見悠人
//最終修正時間：2016/10/30
//修正者 葉梨竜太　BombEventの実装
///////////////////////////////////////////////////////////////////

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
    /// <summary>
    /// マップ上にある物の親クラス
    /// </summary>
    public abstract class GameObject
    {
        //フィールド
        protected string name;          //アセット名
        protected Texture2D image;      //画像
        protected Rectangle localColRect;    //当たり判定区域
        protected Vector2 position;     //位置
        protected int previousBottom;
        //protected Vector2 origin;       //描画の原点

        protected Vector2 velocity;     //移動量（tempを整数化）
        protected bool isGoDie;         //死亡中
        protected bool isDead;          //生きているか？
        protected bool isOnGround;      //地上にいるか？
        protected string tag;           //タグ
        protected bool isTrigger;       //衝突判定の種類 true: 衝突区域  false: 障害物
        protected bool isShow;          //存在判定 true:見える false:見えない
        protected Timer deathTimer;       //消えるまでのTimer
        protected Timer spawnTimer;       //再度見えるようになるまでのTimer
        protected float alpha;

        public int Width
        {
            get
            {
                return image.Width;
            }
        }

        public int Height
        {
            get
            {
                return image.Height;
            }
        }

        public Rectangle LocalColRect
        {
            get
            {
                return localColRect;
            }
        }

        public Rectangle ColRect
        {
            get
            {
                return new Rectangle(localColRect.X + (int)position.X, localColRect.Y + (int)position.Y, localColRect.Width, localColRect.Height);
            }
        }

        public int PreviousBottom
        {
            get
            {
                return previousBottom;
            }
        }

        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public float PositionX
        {
            get
            {
                return position.X;
            }
            set
            {
                position.X = value;
            }
        }

        public float PositionY
        {
            get
            {
                return position.Y;
            }
            set
            {
                position.Y = value;
            }
        }

        public Vector2 Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;
            }
        }

        public float VelocityX
        {
            get
            {
                return velocity.X;
            }
            set
            {
                velocity.X = value;
            }
        }

        public float VelocityY
        {
            get
            {
                return velocity.Y;
            }
            set
            {
                velocity.Y = value;
            }
        }

        public bool IsGoDie
        {
            get
            {
                return IsGoDie;
            }
            set
            {
                isGoDie = value;
            }
        }

        public bool IsDead
        {
            get
            {
                return isDead;
            }
            set
            {
                isDead = value;
            }
        }

        public bool IsOnGround
        {
            get
            {
                return isOnGround;
            }
            set
            {
                isOnGround = value;
            }
        }


        public bool IsTrigger
        {
            get
            {
                return isTrigger;
            }
            set
            {
                isTrigger = value;
            }
        }

        public string Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }

        /// <summary>
        /// コンストラクタ（衝突判定区域は自身サイズと同じ）
        /// </summary>
        /// <param name="name">アセット名</param>
        /// <param name="imageSize">画像サイズ</param>
        /// <param name="pos">位置</param>
        /// <param name="velo">移動量</param>
        /// <param name="isTrigger">区域ですか？</param>
        /// <param name="tag">タグ</param>
        public GameObject(string name, Vector2 pos, Vector2 velo, bool isTrigger, string tag = "")
        {
            this.name = name;
            image = Renderer.GetTexture(name);
            //this.imageSize = imageSize;
            position = pos;
            velocity = velo;
            localColRect = InitLocalColRect();

            this.tag = tag;
            this.isTrigger = isTrigger;
            deathTimer = new Timer(0.5f);
            spawnTimer = new Timer(0.5f);

            Initialize();
        }

        protected virtual Rectangle InitLocalColRect()
        {
            return new Rectangle(0, 0, image.Width, image.Height);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public virtual void Initialize()
        {
            isGoDie = false;
            isDead = false;
            previousBottom = ColRect.Bottom;
            isOnGround = false;
            isShow = true;      //初期はTrue（見える
            alpha = 1.0f;
            //Timerの初期化
            deathTimer.Initialize();
            spawnTimer.Initialize();
        }

        /// <summary>
        /// 抽象更新メソッド
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="offset">カメラ偏移</param>
        public virtual void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            renderer.DrawTexture(name, position * cameraScale + offset, cameraScale, alpha);
        }

        /// <summary>
        /// 四角同士の衝突区域判定
        /// </summary>
        /// <param name="other">判定する対象</param>
        /// <returns></returns>
        public virtual bool CollisionCheck(GameObject other)
        {
            bool flag = false;
            flag = ColRect.Intersects(other.ColRect);
            return flag;
        }

        /// <summary>
        /// 障害物判定
        /// </summary>
        /// <param name="other">対象</param>
        /// <returns></returns>
        public virtual bool ObstacleCheck(GameObject other)
        {
            bool flag = Method.ObstacleCheck(this, other);

            return flag;
        }

        /// <summary>
        /// 消える時間と復活までの時間を設定
        /// </summary>
        /// <param name="deathTime">消える時間</param>
        /// <param name="spawnTime">復活する時間</param>
        public void SetTimer(float deathTime, float spawnTime)
        {
            deathTimer.Change(deathTime);
            spawnTimer.Change(spawnTime);
        }

        /// <summary>
        /// 消えるまでの時間を設定
        /// </summary>
        /// <param name="deathTime">消えるまでの時間</param>
        public void SetTimer(float deathTime)
        {
            deathTimer.Change(deathTime);
        }

        /// <summary>
        /// 存在判定（そのクラスのUpdateに追加する）
        /// </summary>
        public virtual void AliveUpdate()
        {
            if (isShow == false)//見えないとき
            {
                deathTimer.Update();
                if (alpha >= 0.0f)       //Alphaが０になるまで減らす
                {
                    alpha -= 0.06f;
                }
                if (deathTimer.IsTime())     //時間になったら当たり判定を消す
                {
                    isTrigger = true;
                }
                spawnTimer.Update();
                if (spawnTimer.IsTime())
                {
                    isShow = true;      //可視化
                    deathTimer.Initialize(); //Timerの初期化
                }
            }
            else//見えるとき
            {
                isTrigger = false; //当たり判定を作る
                if (alpha <= 1.0f) // alphaを増やす
                {
                    alpha += 0.1f;
                }
            }
        }

        //by長谷川修一
        /// <summary>
        /// 消したいだけのとき
        /// </summary>
        public virtual void DeathUpdate()
        {
            if (isShow == false)
            {
                deathTimer.Update();
                if (deathTimer.IsTime())
                {
                    isTrigger = true;
                }
            }
        }
        /// <summary>
        /// 存在判定（EventHandleに追加する）
        /// </summary>
        /// <param name="other">対象</param>
        public virtual void AliveEvent(GameObject other)
        {
            if (other is Fire || other is Player)
            {
                //if(other is Fire)
                //other.IsDead = true;

                isShow = false;         //不可視化

            }
            spawnTimer.Initialize(); //Timerを初期化して可視化するのを防ぐ
        }

        /// <summary>
        /// 存在判定（EventHandleに追加する）
        /// </summary>
        /// <param name="other">対象</param>
        public virtual void DeathEvent(GameObject other)
        {
            if (other is Fire || other is Player)
            {
                isShow = false;
            }
        }

        //葉梨竜太　11/30
        /// <summary>
        /// 爆弾限定の判定(EventHandleに追加する)
        /// </summary>
        /// <param name="other">対象</param>
        public virtual void BombEvent(GameObject other)
        {
            if(other is Bomb)
            {
                isDead = true;
                isShow = false;
            }
        }

        /// <summary>
        /// 透明値プロパティ
        /// </summary>
        /// <returns></returns>
        public float Alpha
        {
            get
            {
                return alpha;
            }
            set
            {
                alpha = value;
            }
        }

        
        /// <summary>
        /// 衝突事件の処理
        /// </summary>
        /// <param name="other">衝突対象/param>
        public abstract void EventHandle(GameObject other);   

    }
}
