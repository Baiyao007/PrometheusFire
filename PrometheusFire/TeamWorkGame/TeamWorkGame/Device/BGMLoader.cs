using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamWorkGame.Device
{
    /// <summary>
    /// BGM読み込みクラス
    /// </summary>
    class BGMLoader : Loader
    {
        private Sound sound;

        public BGMLoader(Sound sound, string[,] resources) :
            base(resources)
        {
            this.sound = sound;
            Initialize();
        }

        /// <summary>
        /// 更新
        /// </summary>
        public override void Update()
        {
            endFlag = true;
            if(counter < maxNum)
            {
                sound.LoadBGM(resources[counter, 0], resources[counter, 1]);
                counter++;
                endFlag = false;
            }
        }
    }
}
