using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamWorkGame.Device
{
    /// <summary>
    /// SE読み込みクラス
    /// </summary>
    class SELoader : Loader
    {
        private Sound sound;

        public SELoader(Sound sound, string[,] resources):
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
                sound.LoadSE(resources[counter, 0], resources[counter, 1]);
                sound.CreateSEInstance(resources[counter, 0]);
                counter++;
                endFlag = false;
            }
        }
    }
}
