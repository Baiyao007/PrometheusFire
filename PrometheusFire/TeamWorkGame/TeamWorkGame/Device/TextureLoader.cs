using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamWorkGame.Device
{
    class TextureLoader : Loader
    {
        private Renderer renderer;

        public TextureLoader(Renderer renderer, string[,] resources) :
            base(resources)
        {
            this.renderer = renderer;
            Initialize();
        }

        public override void Update()
        {
            endFlag = true;

            if(counter < maxNum)
            {
                renderer.LoadTexture(resources[counter, 0], resources[counter, 1]);
                counter++;
                endFlag = false;
            }
        }
    }
}
