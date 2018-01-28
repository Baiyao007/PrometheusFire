///作成日：2016.12.21
///作成者：柏
///作成内容： パーティクルクラス
///最後修正内容：コードの整理　＋　コメント追加　＋　微調整
///最後修正者：柏
///最後修正日：2016.12.22

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamWorkGame.Device;
using TeamWorkGame.Def;

namespace TeamWorkGame.Utility
{
    class Particle
    {
        private static Random rnd = new Random();
        private string name;        //花火リソースの名前
        private float size;         //花火の大きさ
        private float gravity;      //重力
        private int fireCount;      //花火の爆発回数管理
        private Timer fireContinueTimer;     //複数爆発のタイミング管理
        private Timer burntimer;        //子花火の爆発時間管理
        private Vector2 position;       //打ち上げと爆発の位置
        private Vector2 velocity;       //花火の速度
        private Color color;    //花火の色

        private List<Particle> fireworks;       //子花火管理リストに追加
        private List<Vector2> fireVelocitys;    //花火の飛び方向の保存用
        private bool isDead;        //終わったかどうか
        private bool burn;          //爆発したかどうか
        private bool visible;       //見えるかどうか

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="velocity">移動方向と速度</param>
        /// <param name="burn">初期値は爆発した</param>
        /// <param name="size">初期サイズは0.3倍の画像の大きさ</param>
        public Particle(Vector2 velocity, bool burn = true, float size = 0.3f)
        {
            name = "firework";
            this.size = size;
            gravity = 1.0f;
            fireCount = 0;
            fireContinueTimer = new Timer(0.25f);    //花火は0.25秒ずつ再爆発するに設定
            burntimer = new Timer(0.8f);        //子花火の爆発時間は0.8秒に設定

            //花火は窓口の一番下の任意位置で生成するのに設定
            position = new Vector2(GetFireStartPositionX(), Parameter.ScreenHeight);
            this.velocity = velocity;

            fireworks = new List<Particle>();
            fireVelocitys = new List<Vector2>();
            isDead = false;
            this.burn = burn;
            visible = true;

            InitializeFireVelocity();   //花火の飛び方向を登録
        }

        /// <summary>
        /// 生成時のＸ座標を計算する    by柏　2016.12.22
        /// </summary>
        /// <returns></returns>
        private int GetFireStartPositionX() {
            int x = rnd.Next(Parameter.ScreenWidth);
            int min = Parameter.ScreenWidth / 5 * 2;
            int max = Parameter.ScreenWidth / 5 * 3;

            while (x < max && x > min) {
                x = rnd.Next(Parameter.ScreenWidth);
            }
            return x;
        }


        /// <summary>
        /// 花火の飛び方向の登録
        /// </summary>
        private void InitializeFireVelocity()
        {
            float x = 0, y = 0;
            int fireMax = Parameter.FireworksCount;     //一回の爆発生成してくる花火の個数のリネーム
            for (int i = 0; i < fireMax; i++)
            {
                x = (float)Math.Cos(Math.PI * 2 / fireMax * i);     //横方向速度(円を花火の個数で分割して計算)
                y = (float)Math.Sin(Math.PI * 2 / fireMax * i);     //縦方向速度(円を花火の個数で分割して計算)
                fireVelocitys.Add(new Vector2(x, y));       //登録
            }
        }

        /// <summary>
        /// 花火の追加
        /// </summary>
        private void AddFireworks()
        {
            List<Particle> fireList = new List<Particle>();
            for (int i = 0; i < fireVelocitys.Count; i++)
            {
                Particle newP = new Particle(5 * fireVelocitys[i]);
                newP.color = color;
                fireList.Add(newP);
            }
            fireList.ForEach(f => {
                f.position = position;   //爆発座標を設定
                f.SetFireCount();       //爆発回数の上限で設定（子花火の再爆発を防ぐため）
            });
            fireworks.AddRange(fireList);   //子花火管理リストに追加
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            Move();     //花火の移動処理
            ReBurn();   //花火の再爆発処理
            if (CheckBurnAble()) { Burn(); }
            BurnedFireUpdate();         //爆発した花火の更新
        }

        /// <summary>
        /// 移動処理
        /// </summary>
        public void Move()
        {
            position += velocity;

            //爆発しなかった場合打ち上げの減速処理
            if (!burn)
            {
                float speedDownRate = Parameter.FireworksSpeedDownRate;     //減速率のリネーム
                velocity *= speedDownRate;
            }
        }

        /// <summary>
        /// 爆発した花火の更新処理
        /// </summary>
        public void BurnedFireUpdate()
        {
            if (!burn) { return; }      //爆発しなかったら更新しない

            //子花火の更新
            foreach (var f in fireworks)
            {
                if (!f.visible) { continue; }
                f.Update();
            }
            burntimer.Update();     //爆発時間の更新はじめ
            position.Y += gravity;      //Ｙ座標に重力加算

            //２フレームずつサイズと重力自分自身の加算
            if (burntimer.CurrentTime % 2 == 0)
            {
                size *= 1.12f;      //サイズ大きくなる
                gravity *= 1.1f;    //重力大きくなる
            }
            if (burntimer.IsTime()) { visible = false; }    //爆発時間終わると見えないようになる
            fireworks.RemoveAll(f => !f.visible);       //見えない子花火を削除
            isDead = (fireworks.Count()) == 0 ? true : false;       //子花火全部爆発終わると親花火は死ぬ
        }

        //再爆発処理
        public void ReBurn()
        {
            if (!burn) { return; }      //爆発状態じゃないと処理しない
            if (fireCount >= Parameter.FireworksReBurnCount) { return; }  //再爆発回数は上限だったら処理しない

            fireContinueTimer.Update();
            if (fireContinueTimer.IsTime())
            {
                fireContinueTimer.Initialize();
                fireCount++;    //爆発回数　＋１
                burn = false;   //爆発できるようになる
            }
        }

        /// <summary>
        /// 爆発できるかどうかチェック
        /// </summary>
        /// <returns></returns>
        private bool CheckBurnAble()
        {
            if (burn) { return false; }     //親花火もう爆発した状態だったら、爆発できない
            if (velocity.Y >= -1.5f)
            {
                velocity.Y = 0;     //親花火速度０で初期化
                burn = true;        //親花火爆発したに設定
                visible = false;    //親花火見えないに設定
                return true;
            }
            return false;
        }

        /// <summary>
        /// 爆発処理
        /// </summary>
        private void Burn()
        {
            AddFireworks();     //子花火の追加
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer">描画用クラス</param>
        public void Draw(Renderer renderer)
        {
            DrawFires(renderer);    //子花火の描画処理

            if (burn)
            {
                if (!visible) { return; }
                //花火は爆発時間に合わせてどんどん透明になる
                renderer.DrawTexture(name, position, color, size, burntimer.Rate());
            }
            else {
                renderer.DrawTexture(name, position, color, size, 0.8f);
            }

        }

        /// <summary>
        /// 子花火の描画処理
        /// </summary>
        /// <param name="renderer">描画用クラス</param>
        public void DrawFires(Renderer renderer)
        {
            if (fireworks.Count() == 0) { return; }
            fireworks.ForEach(f => f.Draw(renderer));
        }

        /// <summary>
        /// 再爆破回数は上限に設定
        /// </summary>
        public void SetFireCount()
        {
            fireCount = Parameter.FireworksReBurnCount;
        }

        /// <summary>
        /// 死亡状態のゲット
        /// </summary>
        public bool IsDead
        {
            get { return isDead; }
        }

        /// <summary>
        /// 色の変更
        /// </summary>
        public Color SetColor
        {
            set { color = value; }
        }
    }
}
