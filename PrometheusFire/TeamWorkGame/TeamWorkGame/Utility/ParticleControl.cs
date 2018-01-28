///作成日：2016.12.21
///作成者：柏
///作成内容： パーティクル表現用クラス
///最後修正内容：コードの整理　＋　コメント追加
///最後修正者：柏
///最後修正日：2016.12.22

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TeamWorkGame.Device;
using TeamWorkGame.Def;

namespace TeamWorkGame.Utility
{
    class ParticleControl
    {
        private Timer addTimer;     //花火の生成タイミング管理
        private List<Particle> fireworks;   //親花火の管理リスト
        private List<Color> colors;     //花火の色管理
        private static Random rnd = new Random();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ParticleControl()
        {
            addTimer = new Timer(0.2f);     //0.5秒ずつ花火一個打ち上げ
            fireworks = new List<Particle>();
            colors = new List<Color>() {
                Color.Yellow, Color.Red, Color.White, Color.Orange, Color.Pink
                //Color.Blue, Color.BlueViolet,
            };
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            addTimer.Update();
            fireworks.ForEach(f => f.Update());
            fireworks.RemoveAll(f => f.IsDead);
            Fire();
        }


        /// <summary>
        /// パーティクル生成時の初速度
        /// </summary>
        /// <returns></returns>
        private Vector2 CreatSpeed()
        {
            int speedMin = Parameter.FireworksStartSpeedMin;
            int speedMax = Parameter.FireworksStartSpeedMax;
            int X = 0;      //横方向の速度は０
            int Y = rnd.Next(-speedMax, -speedMin);
            return new Vector2(X, Y);
        }

        private Color CreatColor()
        {
            if (colors.Count == 0) { return Color.White; }
            return colors[rnd.Next(colors.Count)];
        }

        /// <summary>
        /// 時間だったら花火を打ち上げ
        /// </summary>
        private void Fire()
        {
            if (addTimer.IsTime())
            {
                //ランダム初速度、再爆発可能、等倍の状態で花火一個生成
                Particle newP = new Particle(CreatSpeed(), false, 1f);
                newP.SetColor = CreatColor();
                fireworks.Add(newP);
                addTimer.Initialize();
            }
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer"></param>
        public void Draw(Renderer renderer)
        {
            fireworks.ForEach(f => f.Draw(renderer));
        }
    }
}
