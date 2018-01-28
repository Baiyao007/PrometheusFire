/////////////////////////////////////////////////
// ステージのクラス
// 最終修正時間：2016年12月14日
// by 柏 ＳＥ実装
/////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TeamWorkGame.Device;
using TeamWorkGame.Actor;
using TeamWorkGame.Def;
using TeamWorkGame.Scene;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Scene
{
    class Stage : IScene
    {
        private InputState inputState;
        private Sound sound;
        private bool isEnd;
        private int mapIndex;
        public List<Vector2> herol;
        private StageSaver sever;
        private Animation animation;
        private Animation standAnime;
        private Animation runAnime;
        private AnimationPlayer animePlayer;
        private bool isBack;
        private Vector2 position;
        private bool isMove;
        private Vector2 startpos;
        private Vector2 endpos;
        private float time;
        private float totaltime;
        private SpriteEffects direction;

        public Stage(GameDevice gameDevice)
        {
            inputState = gameDevice.GetInputState();
            sound = gameDevice.GetSound();
            sever = gameDevice.GetStageSaver();
            herol = new List<Vector2>()
            {
                new Vector2(143-32,200-55),
                new Vector2(307-32,317-55),
                new Vector2(736-32,348-55),
                new Vector2(967-32,469-55),
                new Vector2(1052-32,369-55),
            };
        }
        public void Initialize()
        {
            isEnd = false;
            mapIndex = 0;
            sever.LoadStageData();
            standAnime = new Animation(Renderer.GetTexture("standAnime"), 0.1f, true);
            runAnime = new Animation(Renderer.GetTexture("playerAnime"), 0.1f, true);
            sound.PlayBGM("worldmap1");
            isBack = false;
            animation = standAnime;
            position = herol[mapIndex];
            isMove = false;
            time = 0;
            direction = SpriteEffects.FlipHorizontally;
        }

        public void Initialize(int index)
        {
            isEnd = false;
            mapIndex = index;
            sever.LoadStageData();
            standAnime = new Animation(Renderer.GetTexture("standAnime"), 0.1f, true);
            runAnime = new Animation(Renderer.GetTexture("playerAnime"), 0.1f, true);
            sound.PlayBGM("worldmap1");
            isBack = false;
            animation = standAnime;
            position = herol[mapIndex];
            isMove = false;
            time = 0;
            direction = SpriteEffects.FlipHorizontally;
        }

        //描画の開始と終了は全部Game1のDrawに移動した
        public void Draw(GameTime gameTime, Renderer renderer)
        {
            renderer.DrawTexture("worldmap", Vector2.Zero);

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                renderer.DrawTexture("stageSelect_UI", new Vector2(0, 720 - 64));
            }
            else
            {
                renderer.DrawTexture("stageSelect_UI2", new Vector2(0, 720 - 64));
            }

            animePlayer.PlayAnimation(animation);
            animePlayer.Draw(gameTime, renderer, position, direction, 1);

            for (int i = 0; i < herol.Count(); i++)
            {
                if (i > (sever.ClearStage + 1) / 6)
                {
                    renderer.DrawTexture("lock", herol[i]);
                }
            }
        }

        public void MapAnimation()
        {
            if (isMove)
            {
                animation = runAnime;

                Vector2 velocity = endpos - startpos;
                velocity.Normalize();
                totaltime = Parameter.MapSpeedTime;

                position = startpos + (time / totaltime) * (endpos - startpos);
                time++;

                if (time > totaltime)
                {
                    isMove = false;
                    time = 0;
                    animation = standAnime;
                }
            }
        }

        public void Update(GameTime gametime)
        {            
            if (inputState.IsKeyDown(Keys.Right) || inputState.IsKeyDown(Buttons.LeftThumbstickRight))
            {
                if (isMove == false)
                {
                    sound.PlaySE("cursor");    //by 柏　2016.12.14 ＳＥ実装
                    if (mapIndex >= (sever.ClearStage + 1) / 6)
                    {
                        return;
                    }
                    if (mapIndex >= 4)
                    {
                        return;
                    }
                    isMove = true;

                    mapIndex++;

                    startpos = herol[mapIndex - 1];
                    endpos = herol[mapIndex];
                    direction = SpriteEffects.FlipHorizontally;
                }
               
            }
            else if (inputState.IsKeyDown(Keys.Left) || inputState.IsKeyDown(Buttons.LeftThumbstickLeft))
            {
                if (isMove == false)
                {
                    sound.PlaySE("cursor");    //by 柏　2016.12.14 ＳＥ実装
                    if (mapIndex <= 0)
                    {
                        return;
                    }

                    isMove = true;
                    mapIndex--;

                    startpos = herol[mapIndex + 1];
                    endpos = herol[mapIndex];
                    direction = SpriteEffects.None;
                }
            }

            MapAnimation();

            //position = herol[mapIndex];

            if (isMove == false)
            {
                if (inputState.IsKeyDown(Keys.Z) || inputState.IsKeyDown(Keys.Space) || inputState.IsKeyDown(Keys.Enter) || inputState.IsKeyDown(Buttons.A))
                {
                    //sound.PlaySE("decision1");    //by 柏　2016.12.14 ＳＥ実装  (Next()のところにもう再生した　By　氷見悠人)
                    isEnd = true;
                }
                else if (inputState.IsKeyDown(Keys.X) || inputState.IsKeyDown(Buttons.B))
                {
                    //sound.PlaySE("cancel1");    //by 柏　2016.12.14 ＳＥ実装    (Next()のところにもう再生した　By　氷見悠人)
                    isBack = true;
                    isEnd = true;
                }
            }
        }
        
        public bool IsEnd()
        {
            return isEnd;
        }

        public NextScene Next()
        {
            NextScene nextScene;
            if (isBack == true)
            {
                nextScene = new NextScene(SceneType.Title);
                sound.PlaySE("cancel1");
            }
            else
            {
                nextScene = new NextScene(SceneType.SmallStage, mapIndex * 6);
                sound.PlaySE("decision1");
            }
            return nextScene;
        }

        public NextScene GetNext() {
            NextScene nextScene;
            if (isBack == true)
            {
                nextScene = new NextScene(SceneType.Title);
            }
            else
            {
                nextScene = new NextScene(SceneType.SmallStage, mapIndex * 6);
            }
            return nextScene;
        }

        public void ShutDown()
        {
        }
    }
}
