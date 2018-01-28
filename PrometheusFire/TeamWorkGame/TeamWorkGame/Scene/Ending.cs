/////////////////////////////
//最終更新日　2017/1/11
//更新者　柏
////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using TeamWorkGame.Device;
using TeamWorkGame.Def;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Scene
{
    enum EndLevel
    {
        None = -1,
        Clear,
        FireWorks,
        Text = 3,
    }

    class Ending : IScene
    {
        private InputState inputState;
        private bool isEnd;
        private int clearLevel;
        private Timer clearLevelTimer;
        ParticleControl particleControl;
        Timer flashTimer;
        float textAlph;

        public Ending(GameDevice gameDevice)
        {
            inputState = gameDevice.GetInputState();
            particleControl = new ParticleControl();   //Ending演出実装 by柏 2017.1.11

            Initialize(-1);
        }

        public void Initialize(int index) {
            isEnd = false;
            clearLevel = (int)EndLevel.None;        //Clear関連内容の表示段階管理
            clearLevelTimer = new Timer(1.0f);      //Clear関連内容の段階表示タイミング
            flashTimer = new Timer(0.2f);
            textAlph = 0.5f;
        }

        public bool IsEnd()
        {
            return isEnd;
        }

        public void ShutDown()
        {

        }

        public void Update(GameTime gametime) {
            //by柏 2017.1.11 Ending演出用
            ClearShow();
            if (clearLevel < (int)EndLevel.Text) { return; }
            Flash();

            //葉梨竜太　11/30
            if (inputState.IsKeyDown(Keys.Enter) || inputState.IsKeyDown(Keys.Z) || inputState.IsKeyDown(Keys.Space) || inputState.IsKeyDown(Buttons.A)) {
                isEnd = true;
            }
        }


        /// <summary>
        /// Ending演出 by柏　2017.1.11
        /// </summary>
        /// <param name="gameTime">ゲーム時間</param>
        /// <param name="renderer">描画用クラス</param>
        public void Draw(GameTime gameTime, Renderer renderer)
        {
            DrawClear(renderer, gameTime);
        }

        public NextScene Next() {
            NextScene nextScene = new NextScene(SceneType.Title, -1);
            return nextScene;
        }

        public NextScene GetNext() {
            return Next();
        }


        /// <summary>
        /// 文字点滅管理 by柏 2017.1.11
        /// </summary>
        private void Flash() {
            flashTimer.Update();
            if (flashTimer.IsTime()) {
                textAlph = (textAlph == 1.0f) ? 0.5f : 1.0f;
                flashTimer.Initialize();
            }
        }


        /// <summary>
        /// Ending演出段階管理 by柏 2017.1.11
        /// </summary>
        private void ClearShow() {
            if (clearLevel > 0) { particleControl.Update(); }
            clearLevelTimer.Update();
            if (clearLevelTimer.IsTime()) {
                clearLevel++;
                clearLevelTimer.Initialize();
            }
        }

        /// <summary>
        /// 演出段階表示 by柏 2017.1.11
        /// </summary>
        /// <param name="renderer">描画用クラス</param>
        /// <param name="gameTime">ゲーム時間</param>
        public void DrawClear(Renderer renderer, GameTime gameTime) {
            renderer.DrawTexture("backGround", Vector2.Zero);

            float size = (clearLevel < (int)EndLevel.Clear) ? (1 - clearLevelTimer.Rate()) : 1.0f;
            int Y = 100;
            int X = (int)(Parameter.ScreenWidth / 2 - Renderer.GetTexture("lastclear").Width / 2 * size);
            renderer.DrawTexture("lastclear", new Vector2(X, Y), size, 1);

            if (clearLevel < (int)EndLevel.FireWorks) { return; }
            particleControl.Draw(renderer);

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                if (clearLevel < (int)EndLevel.Text) { return; }
                renderer.DrawTexture("titlebackA", new Vector2(330, 500), textAlph);
            }
            else
            {
                if (clearLevel < (int)EndLevel.Text) { return; }
                renderer.DrawTexture("ToTitleText", new Vector2(330, 500), textAlph);
            }
        }

    }
}
