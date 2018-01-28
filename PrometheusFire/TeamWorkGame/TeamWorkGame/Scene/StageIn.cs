using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TeamWorkGame.Device;
using TeamWorkGame.Def;
using TeamWorkGame.Utility;


namespace TeamWorkGame.Scene
{
    class StageIn : IScene
    {
        private string word;
        private bool isEnd;
        private int index;
        private Timer timer;
        private int width;
        private int height;
        private Vector2 wordPosition;
        private Vector2 charaPosition;
        private float scale;

        //アニメーション関係
        private AnimationPlayer animePlayer;
        private Animation standAnime;

        public StageIn(float time)
        {
            timer = new Timer(time);
        }

        public void Initialize(int index = -1)
        {
            isEnd = false;
            timer.Initialize();
            this.index = index;
            if (index >= 0)
            {
                int bigIndex = index / StageDef.SmallIndexMax + 1;
                int smallIndex = index % StageDef.SmallIndexMax + 1;

                word = bigIndex + " - " + smallIndex;
            }
            width = word.Length * 32;
            height = 64;
            scale = 1.5f;
            wordPosition = new Vector2((Parameter.ScreenWidth - width) / 2, (Parameter.ScreenHeight - height) / 2 - 64);
            charaPosition = new Vector2((Parameter.ScreenWidth - 64 * scale) / 2, wordPosition.Y + 128);

            animePlayer = new AnimationPlayer();
            standAnime = new Animation(Renderer.GetTexture("standAnime"), 0.1f, true);
            animePlayer.PlayAnimation(standAnime);
        }

        public bool IsEnd()
        {
            return isEnd;
        }

        public NextScene Next()
        {
            NextScene nextScene;
            nextScene = new NextScene(SceneType.PlayScene, index);
            return nextScene;
        }

        public NextScene GetNext() {
            return Next();
        }

        public void ShutDown()
        {

        }

        public void Update(GameTime gameTime)
        {
            timer.Update();
            if (timer.IsTime())
            {
                isEnd = true;
            }
        }

        public void Draw(GameTime gameTime, Renderer renderer)
        {
            //renderer.DrawTexture("fadein", Vector2.Zero);
            renderer.DrawNumber("number", wordPosition, word, 5);
            animePlayer.Draw(gameTime, renderer, charaPosition, SpriteEffects.None, scale);
            //renderer.DrawTexture("hero", charaPosition, scale, 1.0f);
        }
    }
}
