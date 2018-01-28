using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TeamWorkGame.Device;
using TeamWorkGame.Actor;
using TeamWorkGame.Def;
using TeamWorkGame.Scene;
using TeamWorkGame.Utility;


namespace TeamWorkGame.Scene
{
    class Credit:IScene
    {
        private InputState inputState;
        private Sound sound;
        private bool isEnd;
        private Vector2 creditpos;
        private Vector2 textpos;
        private Timer time;
        private float alpha;
        private bool alphaflag;

        public Credit(GameDevice gameDevice)
        {
            inputState = gameDevice.GetInputState();
            sound = gameDevice.GetSound();
        }

        public void Initialize(int index = -1)
        {
            isEnd = false;
            creditpos = new Vector2(0,Parameter.ScreenHeight);
            textpos = new Vector2(Parameter.ScreenWidth / 2 - Parameter.TextWidth / 2, 0 - Parameter.TextHeight);
            time = new Timer(1f);
            alpha = 1.0f;
            alphaflag = true;
        }

        public void Update(GameTime gameTime)
        {
            if (creditpos.Y >= 0) 
            creditpos.Y-=10;

            if (textpos.Y <= Parameter.TextHeight) 
            textpos.Y += 10;

            time.Update();
            
            if (time.IsTime())
            {
                alphaflag = !alphaflag;
                alpha = alphaflag ? 1.0f : 0.5f;
                time.Initialize();
            }

            if (inputState.CheckDownKey(Keys.X, Buttons.B))
            {
                isEnd = true;
            }
        }

        public void Draw(GameTime gameTime, Renderer renderer)
        {
            renderer.DrawTexture("backGround", Vector2.Zero);
            renderer.DrawTexture("credit", creditpos);
            renderer.DrawTexture("text", textpos, new Rectangle(0, (int)Text.CREDIT * Parameter.TextHeight, Parameter.TextWidth, Parameter.TextHeight));
            if (!GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                renderer.DrawTexture("Zback", new Vector2(Parameter.ScreenWidth - 500, Parameter.ScreenHeight - 70), alpha);
            }
            else
            {
                renderer.DrawTexture("titleback", new Vector2(Parameter.ScreenWidth - 500, Parameter.ScreenHeight - 70), alpha);
            }
        }

        public void ShutDown()
        {
        }

        public bool IsEnd()
        {
            return isEnd;
        }

        public NextScene Next()
        {
            NextScene nextScene = new NextScene(SceneType.Title, -1);
            
            return nextScene;
        }

        public NextScene GetNext() {
            return Next();
        }

    }
}
