/////////////////////////////////////////////////
// ステージ選択のクラス
// 作成時間：2016年10月13日
// By 葉梨竜太
//最終修正時間：2016年12月14日
// by 柏　ＳＥ実装
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
    class SmallStage : IScene
    {
        private InputState inputState;
        private Sound sound;
        private bool isEnd;
        private int stageIndex;
        private int mapIndex;
        private List<Vector2> mapl;
        private int mapnum;
        private bool isBack;
        private StageSaver sever;
        private Vector2 flame;
        private Animation standAnime;
        private AnimationPlayer animePlayer;

        public SmallStage(GameDevice gameDevice)
        {
            inputState = gameDevice.GetInputState();
            sever = gameDevice.GetStageSaver();
            mapIndex = 0;
            mapnum = 0;
            sound = gameDevice.GetSound();
        }

        public void Initialize(int index)
        {
            mapIndex = 0;
            mapnum = 0;
            isEnd = false;
            isBack = false;
            stageIndex = index;
            sever.LoadStageData();
            standAnime = new Animation(Renderer.GetTexture("standAnime"), 0.1f, true);
            mapl = new List<Vector2>()
            {
                new Vector2(636,41),
                new Vector2(636,253),
                new Vector2(636,465),
                new Vector2(636,737),
                new Vector2(636,949),
                new Vector2(636,1161),
            };
        }
        public void Draw(GameTime gameTime, Renderer renderer)
        {
            if (stageIndex/6 == 4)
            {
                renderer.DrawTexture("templeground", Vector2.Zero);
            }
            else if (stageIndex / 6 == 3)
            {
                renderer.DrawTexture("desertground", Vector2.Zero);
            }
            else if(stageIndex/6 == 2)
            {
                renderer.DrawTexture("mountainground", Vector2.Zero);
            }
            else if (stageIndex / 6 == 1)
            {
                renderer.DrawTexture("forestground", Vector2.Zero);
            }
            else
            {
                renderer.DrawTexture("backGround", Vector2.Zero);
            }

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                renderer.DrawTexture("stageSelect_UI", new Vector2(0, 720 - 64));
            }
            else
            {
                renderer.DrawTexture("stageSelect_UI2", new Vector2(0, 720 - 64));
            }

            renderer.DrawTexture("frame", flame = new Vector2(mapl[mapIndex].X - 10, mapl[mapIndex].Y - 10));

            for(int i = 0; i < mapl.Count; i++)
            {
                renderer.DrawTexture("smallmap", mapl[i]);
                renderer.DrawNumber("number", mapl[i]+new Vector2(516/2,163/2)-new Vector2(32/2,64/2), i+1);
            }

            //renderer.DrawTexture("smallmap1", mapl[0]);
            //renderer.DrawTexture("smallmap2", mapl[1]);
            //renderer.DrawTexture("smallmap3", mapl[2]);
            //renderer.DrawTexture("smallmap4", mapl[3]);
            //renderer.DrawTexture("smallmap5", mapl[4]);
            //renderer.DrawTexture("smallmap6", mapl[5]);

            if(mapIndex >=3)
            renderer.DrawTexture("uparrow", new Vector2(550, 50));
            else
            renderer.DrawTexture("downarrow", new Vector2(550, 550));

            for (int i = 0; i < mapl.Count(); i++)
            {
                if (i > sever.ClearStage - stageIndex+1)
                {
                    renderer.DrawTexture("lock", mapl[i]);
                }
            }

            animePlayer.PlayAnimation(standAnime);
            animePlayer.Draw(gameTime, renderer, new Vector2(200,300), SpriteEffects.None,3);
        }

        public void Update(GameTime gametime)
        {
            if (inputState.IsKeyDown(Keys.Right) || inputState.IsKeyDown(Buttons.LeftThumbstickRight))
            {

            }
            else if (inputState.IsKeyDown(Keys.Left)||inputState.IsKeyDown(Buttons.LeftThumbstickLeft))
            {

            }
            if (inputState.IsKeyDown(Keys.Up)||inputState.IsKeyDown(Buttons.LeftThumbstickUp))
            {
                sound.PlaySE("cursor");    //by 柏　2016.12.14 ＳＥ実装
                mapnum--;
                if(mapnum == -1)
                {
                    mapnum = 5;
                    if (mapnum >= sever.ClearStage - stageIndex+1)
                    {
                        mapnum = 0;
                    }
                }
                
                if (mapnum == 2) 
                {
                    for (int i = 0; i < mapl.Count; i++)
                    {
                        mapl[i] = new Vector2(mapl[i].X, mapl[i].Y + 696);
                    }
                }

                if (mapnum == 5)
                {
                    for (int i = 0; i < mapl.Count; i++)
                    {
                        mapl[i] = new Vector2(mapl[i].X, mapl[i].Y - 696);
                    }
                }
            }
            else if (inputState.IsKeyDown(Keys.Down)||inputState.IsKeyDown(Buttons.LeftThumbstickDown))
            {
                sound.PlaySE("cursor");    //by 柏　2016.12.14 ＳＥ実装
                if (mapnum  >= sever.ClearStage-stageIndex+1)
                {
                    return;
                }
                mapnum++; 
                if(mapnum == 6)
                {
                    mapnum = 0;
                }   
                        
                if(mapnum == 3)
                {
                    for (int i = 0; i < mapl.Count ; i++) 
                    {
                        mapl[i] = new Vector2(mapl[i].X, mapl[i].Y -696);
                    }
                }

                if (mapnum == 0)
                {
                    for (int i = 0; i < mapl.Count; i++)
                    {
                        mapl[i] = new Vector2(mapl[i].X, mapl[i].Y + 696);
                    }
                }
            }
            
            mapIndex = mapnum;

            if (inputState.IsKeyDown(Keys.Z) || inputState.IsKeyDown(Keys.Space) || inputState.IsKeyDown(Keys.Enter) || inputState.IsKeyDown(Buttons.A))
            {
                //sound.PlaySE("decision1");    //by 柏　2016.12.14 ＳＥ実装  (Next()のところにもう再生した　By　氷見悠人)
                if (stageIndex + mapIndex <= sever.ClearStage + 1)
                {
                    isEnd = true;
                }
            }
            else if (inputState.IsKeyDown(Keys.X) || inputState.IsKeyDown(Buttons.B))
            {
                //sound.PlaySE("cancel1");    //by 柏　2016.12.14 ＳＥ実装    (Next()のところにもう再生した　By　氷見悠人)
                isBack = true;
                isEnd = true;
            }
        }

        public bool IsEnd()
        {
            return isEnd;
        }

        public NextScene Next()
        {
            NextScene nextScene;
            if (isBack) 
            {
                nextScene = new NextScene(SceneType.Stage,stageIndex/6);
                sound.PlaySE("cancel1");
            }
            else
            {
                //nextScene = new NextScene(SceneType.PlayScene, mapIndex + stageIndex);
                nextScene = new NextScene(SceneType.StageIn, mapIndex + stageIndex);
                sound.PlaySE("decision1");
                sound.StopBGM();
            }
            return nextScene;
        }

        public NextScene GetNext()
        {
            NextScene nextScene;
            if (isBack)
            {
                nextScene = new NextScene(SceneType.Stage, stageIndex);
            }
            else
            {
                nextScene = new NextScene(SceneType.StageIn, mapIndex + stageIndex);
            }
            return nextScene;
        }

        public void ShutDown()
        {

        }
    }
}
