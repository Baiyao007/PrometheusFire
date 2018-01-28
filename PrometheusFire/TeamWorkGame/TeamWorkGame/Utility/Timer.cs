/////////////////////////////////////////////////////////////////////////
// Timerクラス
// 作成者：氷見悠人
// 最後修正日：2016.12.22
// 最後修正人：柏
// 最後修正内容：経過時間比率の追加
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamWorkGame.Utility
{
    public class Timer
    {
        //フィールド
        private float currentTime;  //現在時間
        private float limitTime;    //制限時間
        private bool isStop;

        public float CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
            }
        }

        public float LimitTime
        {
            get
            {
                return limitTime;
            }
        }

        public Timer()
        {
            limitTime = 60;
            Initialize();
        }

        public Timer(float seconds)
        {
            limitTime = seconds * 60;
            Initialize();
        }

        public void Initialize()
        {
            currentTime = limitTime;
            isStop = false;
        }

        /// <summary>
        /// Timer状態の更新
        /// </summary>
        public void Update()
        {
            if (!isStop)
            {
                currentTime--;
                if (currentTime < 0.0f)
                {
                    currentTime = 0.0f;
                }
            }
        }

        /// <summary>
        /// Get今の時刻
        /// </summary>
        /// <returns>今の時間</returns>
        public float Now()
        {
            return currentTime;
        }

        /// <summary>
        /// 規定時間になったか？
        /// </summary>
        /// <returns>true or false</returns>
        public bool IsTime()
        {
            return currentTime <= 0;
        }

        public void Stop()
        {
            isStop = true;
        }

        public void Restart()
        {
            isStop = false;
        }
        /// <summary>
        /// 規定時間の変更
        /// </summary>
        /// <param name="limitTime"></param>
        public void Change(float seconds)
        {
            this.limitTime = seconds * 60;
            Initialize();
        }

        /// <summary>
        /// 経過時間の比率 2016.12.22　by柏
        /// </summary>
        /// <returns></returns>
        public float Rate()
        {
            return currentTime / limitTime;
        }

    }
}
