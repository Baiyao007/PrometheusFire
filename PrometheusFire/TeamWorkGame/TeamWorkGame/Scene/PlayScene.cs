/////////////////////////////////////////////////
//プレイシーン
//作成時間：2016/9/26
//作成者：氷見悠人
// 最終修正時間：2017年1月26日
// 修正内容:stageNumの表示と合わせて調整
// by柏
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
    class PlayScene : IScene
    {
        private GameDevice gameDevice;
        private InputState input;
        private Sound sound;
        private bool isEnd;     //終了、Fade開始
        private bool isClear;
        private bool isPause;   //一時停止状態　By　氷見悠人
        private bool isView;        //カメラ操作中　By　氷見悠人

        //葉梨竜太
        private bool isOver;
        private int mapIndex;
        private Player player;
        private Camera camera;
        private Map map;
        private List<Fire> fires;
        private List<WaterLine> waterLines;
        private List<GameObject> coals;     //マップに存在した炭の数　By佐瀬拓海
        private List<GameObject> nowCoals;  //現在の炭の数 By佐瀬拓海
        private ClearSelect clearSelect;    //clear後の選択画面
        private FireMeter fireMeter;
        private Goal goal;

        private Timer StartTimer;

        //柏
        private StageSaver stageSaver;
        private int playTime;
        private ParticleControl particleControl;    //clear演出用
        private int clearLevel;         //clear関連内容は段階的表示用
        private Timer clearLevelTimer;    //clear関連内容は段階的表示用

        public PlayScene(GameDevice gameDevice, int mapIndex = 0)
        {
            this.gameDevice = gameDevice;
            sound = this.gameDevice.GetSound();
            input = this.gameDevice.GetInputState();
            this.mapIndex = mapIndex;
            //isEnd = false;
        }

        public void Initialize(int stageIndex)
        {
            StartTimer = new Timer(1f);
            mapIndex = stageIndex;
            isEnd = false;
            isClear = false;
            //葉梨竜太
            isOver = false;
            isPause = false;    //一時停止状態　By　氷見悠人
            //isStarting = true;
            isView = false;

            //全局Animation一時停止のスイッチ　By　氷見悠人
            FuncSwitch.AllAnimetionPause = false;

            MapManager.SetNowMap(mapIndex, sound);  // by柏　SE実装 2016.12.14
            map = MapManager.GetNowMapData();
            fires = new List<Fire>();
            waterLines = new List<WaterLine>();
            foreach (var ice in map.MapThings.FindAll(x => x is Ice))
            {
                ((Ice)ice).SetWaters(waterLines);
            }
            coals = new List<GameObject>();
            coals = map.MapThings.FindAll(x => x is Coal);
            nowCoals = new List<GameObject>();

            //氷見悠人
            player = new Player(gameDevice, MapManager.PlayerStartPosition(), Vector2.Zero, ref fires, ref waterLines, isView);

            clearSelect = new ClearSelect(gameDevice.GetInputState(), player, sound);   //by 柏　2016.12.14 ＳＥ実装

            camera = new Camera(player.Position + new Vector2(32, 32), Parameter.CameraScale, true);
            player.SetCamera(camera);
            fireMeter = new FireMeter();

            //Goalを取得とCamera設置  By　氷見悠人
            goal = map.GetGoal();
            goal.SetCamera(camera);

            //柏
            stageSaver = gameDevice.GetStageSaver();
            playTime = 0;
            particleControl = new ParticleControl();   //Clear演出実装 2016.12.22
            clearLevel = -1;                         //Clear関連内容の表示段階管理
            clearLevelTimer = new Timer(1.0f);      //Clear関連内容の段階表示タイミング

            //PlayBGM
            sound.PlayBGM("forest1");
        }

        //Goal出現用の状態変換　By　氷見悠人
        public void ChangeGoalStage(GameTime gameTime)
        {
            if (goal.State == GoalState.NONE && nowCoals.Count == 0)
            {
                camera.SetLimitView(false);
                goal.IsComplete = true;
            }
        }

        private void CameraControl()
        {
            Vector2 pos = camera.AimPosition;
            //カメラの移動 矢印キーに変更

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                pos += input.RightVelocity() * 15;
            }
            else
            {
                pos += input.Velocity() * 15;
            }
            camera.SetAimPosition(pos);
        }

        /// <summary>
        /// Clear演出 by柏 2016.12.22
        /// </summary>
        private void ClearShow()
        {
            if (!isClear) { return; }
            if (clearLevel > 0) { particleControl.Update(); }
            if (clearLevel > Parameter.ClearSelectLevel)
            {
                clearSelect.IsClear = true;     //選択できるようになる
            }
            clearLevelTimer.Update();
            if (clearLevelTimer.IsTime())
            {
                clearLevel++;
                clearLevelTimer.Initialize();
            }
        }

        /// <summary>
        /// ゴール後処理  by柏  2016.12.22
        /// </summary>
        private void Goal()
        {
            if (map.GetGoal() == null) { return; }
            if (!map.GetGoal().IsOnFire) { return; }
            isClear = true;
            player.IsClear = true;
            sound.PlaySE("GameClear");  //by柏 SE実装 2016.12.14
            clearLevel = 0;     //by柏 Clear段階表示はじめ 2016.12.22

            stageSaver.ClearStage = mapIndex;
            stageSaver.PlayTime = playTime / 60;
            stageSaver.CurrentStage = mapIndex;
            stageSaver.Charcoal = coals.Count - nowCoals.Count;
            stageSaver.SaveStageData();
        }

        public void Update(GameTime gameTime)
        {
            if (isClear && mapIndex == StageDef.BigIndexMax * StageDef.SmallIndexMax - 1)
            {
                clearSelect.GetSelect = 0;
                isEnd = true;
                return;
            }

            StartTimer.Update();
            if (StartTimer.IsTime())
            {
                StartTimer.Stop();
            }

            ClearShow();

            //死んでいないと更新する
            if (!isClear && !isOver && !isPause)
            {
                //カメラ操作中、別の操作は不可
                if (input.CheckDownKey(Parameter.CameraKey, Parameter.CameraButton))
                {
                    isView = true;
                    player.IsView = isView;
                }
                else
                {
                    isView = false;
                    player.IsView = isView;
                }

                if (isView)
                {
                    CameraControl();
                }

                //Goal出現の時に、全画面の更新を一時停止、Goalの演出だけをする By 氷見悠人
                if (goal.State == GoalState.APPEARING)
                {
                    FuncSwitch.AllAnimetionPause = true;

                    goal.Update(gameTime);
                    camera.MoveAimPosition(goal.Position + new Vector2(goal.Width / 2, goal.Height / 2));
                }
                else
                {
                    FuncSwitch.AllAnimetionPause = false;
                    playTime++;

                    for (int i = 0; i < map.MapThings.Count; i++)
                    {
                        map.MapThings[i].Update(gameTime);
                    }

                    //プレイヤーの更新
                    player.Update(gameTime);

                    //火の更新
                    for (int i = fires.Count - 1; i >= 0; i--)
                    {
                        fires[i].Update(gameTime);
                    }

                    //死んだ火を消す
                    fires.RemoveAll(x => x.IsDead); // && x.IsOnGround 

                    //水の更新
                    foreach (var w in waterLines)
                    {
                        w.Update(gameTime);
                    }

                    waterLines.RemoveAll(x => x.IsDead);

                    //マップの更新
                    map.Update(gameTime);

                    //カメラの注視位置を更新
                    if (!isView)
                    {
                        camera.MoveAimPosition(player.Position + new Vector2(player.Width / 2, player.Height / 2));
                    }
                    //Console.WriteLine(camera.OffSet);

                    //マップ上にある炭の数を取得
                    nowCoals = map.MapThings.FindAll(x => x is Coal);

                    ChangeGoalStage(gameTime);

                    Goal();     //ゴール後処理  by柏  2016.12.22

                    //葉梨竜太
                    if (player.IsDead)
                    {
                        isOver = true;
                        //isClear = true;
                        //clearSelect.IsClear = true;
                    }
                }
            }
            else
            {
                FuncSwitch.AllAnimetionPause = false;
                sound.StopBGM();
            }

            //　ClearWindow2が出るように変更(KeyもQに変更)　By佐瀬拓海
            if (!player.IsDead && !isClear)
            {
                if (gameDevice.GetInputState().CheckTriggerKey(Keys.Q, Parameter.MenuButton))
                {
                    if (isOver == false)
                    {
                        sound.PlaySE("pauseMenu");  //by柏　2016.12.14 ＳＥ実装

                        //全体Animationを一時停止
                        isPause = true;
                        clearSelect.IsPause = isPause;
                        clearSelect.GetSelect = 1;
                        isOver = true;
                    }
                    else if (isOver == true)
                    {
                        sound.PlaySE("pauseMenu");  //by柏　2016.12.14 ＳＥ実装

                        //全体Animationを一時停止解除
                        isPause = false;
                        clearSelect.IsPause = isPause;
                        isOver = false;
                    }
                }
            }

            clearSelect.Update();
            isEnd = clearSelect.IsEnd;  //clear窓口からend状態をとる
            if (isOver)                 //SceneのisOverで判断する
            {
                if (player.IsDead)
                {
                    //player.Death();
                }
                if (isPause)
                {
                    clearSelect.IsClear = true;
                    return;
                }
                else
                {
                    if (clearSelect.IsClear) { return; }
                }
                if (clearSelect.IsClear) { return; }
                clearSelect.Initialize();
                clearSelect.IsClear = true;
            }
            else if (!isOver && !isClear && !isPause)   //isOver = falseでゲーム画面に戻れる By佐瀬拓海
            {
                clearSelect.Initialize();
                clearSelect.IsClear = false;
                sound.PlayBGM("forest1");
            }
        }

        //描画の開始と終了は全部Game1のDrawに移動した
        public void Draw(GameTime gameTime, Renderer renderer)
        {
            if (mapIndex / 6 == 4)
            {
                renderer.DrawTexture("templeground", Vector2.Zero);
            }
            else if (mapIndex / 6 == 3)
            {
                renderer.DrawTexture("desertground", Vector2.Zero);
            }
            else if (mapIndex / 6 == 2)
            {
                renderer.DrawTexture("mountainground", Vector2.Zero);
            }
            else if (mapIndex / 6 == 1)
            {
                renderer.DrawTexture("forestground", Vector2.Zero);
            }
            else
            {
                renderer.DrawTexture("backGround", Vector2.Zero);
            }

            for (int i = 0; i < map.Data.GetLength(0); i++)
            {
                for (int j = 0; j < map.Data.GetLength(1); j++)
                {
                    //動かないマップチップはここで描画 by 長谷川修一
                    if (MapManager.GetNowMapArr()[i, j] == 1)
                    {
                        if (mapIndex / 6 == 3)
                        {
                            renderer.DrawTexture("ground4", map.GetBlockPosition(new BlockIndex(j, i)) * camera.Scale + camera.OffSet, camera.Scale, 1.0f);
                        }
                        else
                        {
                            renderer.DrawTexture("ground2", map.GetBlockPosition(new BlockIndex(j, i)) * camera.Scale + camera.OffSet, camera.Scale, 1.0f);
                        }
                    }
                    if (MapManager.GetNowMapArr()[i, j] == 2)
                    {
                        if (mapIndex / 6 == 3)
                        {
                            renderer.DrawTexture("ground3", map.GetBlockPosition(new BlockIndex(j, i)) * camera.Scale + camera.OffSet, camera.Scale, 1.0f);
                        }
                        else
                        {
                            renderer.DrawTexture("ground1", map.GetBlockPosition(new BlockIndex(j, i)) * camera.Scale + camera.OffSet, camera.Scale, 1.0f);
                        }
                    }
                }
            }

            waterLines.ForEach(x => x.Draw(gameTime, renderer, camera.OffSet, camera.Scale));

            foreach (var x in map.MapThings)//By　佐瀬 拓海
            {
                x.Draw(gameTime, renderer, camera.OffSet, camera.Scale);
            }

            player.Draw(gameTime, renderer, camera.OffSet, camera.Scale);

            fires.ForEach(x => x.Draw(gameTime, renderer, camera.OffSet, camera.Scale));


            //2016.12.22 by柏 Clearの段階的表示
            DrawClear(renderer, gameTime);

            fireMeter.Draw(renderer, player);

            //炭の取得数を描画 By佐瀬拓海
            //2017.1.26 by柏 表示大きさ、位置調整
            renderer.DrawTexture("coal", Parameter.CoalPosition, 0.7f, 1.0f);
            renderer.DrawNumber3("number", new Vector2(1145, 76), (coals.Count - nowCoals.Count).ToString(), 2, 0.5f);
            renderer.DrawNumber3("number", new Vector2(1184, 76), "/", 1, 0.5f);
            renderer.DrawNumber3("number", new Vector2(1220, 76), coals.Count.ToString(), 2, 0.5f);

            //2016.12.22 by柏 playTimeの表示(非表示中)
            //ShowPlayTime(renderer);

            //2017.1.26 by柏 stageNumberの表示
            ShowStageNum(renderer);

            if (!StartTimer.IsTime())
            {
                float scale = 1.5f;
                renderer.DrawTexture("text", new Vector2((Parameter.ScreenWidth - Parameter.TextWidth * scale) / 2, (Parameter.ScreenHeight - Parameter.TextHeight * scale) / 2), new Rectangle(0, 0, Parameter.TextWidth, Parameter.TextHeight), scale, 1);
            }
        }

        /// <summary>
        /// Clear段階表示 by柏
        /// </summary>
        /// <param name="renderer">描画用クラス</param>
        /// <param name="gameTime">ゲーム時間</param>
        public void DrawClear(Renderer renderer, GameTime gameTime)
        {
            if (clearLevel < 0)
            {
                clearSelect.Draw(gameTime, renderer, camera.Scale);
                return;
            }
            float size = (clearLevel < Parameter.ClearFireworksLevel) ? (1 - clearLevelTimer.Rate()) : 1.0f;
            int Y = 100;
            int X = (int)(Parameter.ScreenWidth / 2 - Renderer.GetTexture("text").Width / 2 * size * 2);
            renderer.DrawTexture("text", new Vector2(X, Y), new Rectangle(0, 7 * Parameter.TextHeight, Parameter.TextWidth, Parameter.TextHeight), size * 2, 1);

            particleControl.Draw(renderer);
            if (clearLevel > Parameter.ClearSelectLevel) { clearSelect.Draw(gameTime, renderer, camera.Scale); }
        }

        /// <summary>
        /// プレータイムの表示 by柏
        /// </summary>
        /// <param name="renderer"></param>
        public void ShowPlayTime(Renderer renderer)
        {
            int[] playtime = stageSaver.TimeCalculat(playTime / 60);
            renderer.DrawNumber("number", new Vector2(1152, 128), playtime[0]);
            renderer.DrawNumber("number", new Vector2(1182, 128), "/", 1);
            renderer.DrawNumber("number", new Vector2(1216, 128), playtime[1]);
        }


        /// <summary>
        /// StageNumの表示 by柏   2017.1.26
        /// </summary>
        /// <param name="renderer"></param>
        public void ShowStageNum(Renderer renderer)
        {
            int bigStage = mapIndex / StageDef.SmallIndexMax + 1;
            int smallStage = mapIndex % StageDef.SmallIndexMax + 1;
            renderer.DrawNumber3("number", new Vector2(1152, 16), bigStage.ToString(), 1, 0.7f);
            renderer.DrawNumber3("number", new Vector2(1185, 16), "-", 1, 0.7f);
            renderer.DrawNumber3("number", new Vector2(1216, 16), smallStage.ToString(), 1, 0.7f);
        }


        public Rectangle GetRect(int num)
        {
            Rectangle rect = new Rectangle(num % 4 * 64, num / 4 * 64, 64, 64);
            return rect;
        }


        public bool IsEnd()
        {
            return isEnd;
        }

        public void ShutDown()
        {
            sound.StopBGM();
        }

        NextScene IScene.Next()
        {
            //clear画面の選択肢によって、処理する
            NextScene nextScene;
            if (clearSelect.GetSelect == 0)
            {      //Next
                if (mapIndex == StageDef.BigIndexMax * StageDef.SmallIndexMax - 1)
                {
                    mapIndex = 0;
                    nextScene = new NextScene(SceneType.Ending, -1);
                }
                else
                {
                    mapIndex++;
                    nextScene = new NextScene(SceneType.StageIn, mapIndex);
                }

                //nextScene = new NextScene(SceneType.PlayScene, mapIndex);

            }
            else if (clearSelect.GetSelect == 1)
            {     //RePlay

                nextScene = new NextScene(SceneType.PlayScene, mapIndex);
            }
            else
            {     //World
                nextScene = new NextScene(SceneType.Stage, mapIndex / 6);
            }

            sound.PlaySE("decision1");

            return nextScene;
        }

        public NextScene GetNext()
        {
            NextScene nextScene;
            if (clearSelect.GetSelect == 0)
            {
                if (mapIndex == StageDef.BigIndexMax * StageDef.SmallIndexMax - 1)
                {
                    nextScene = new NextScene(SceneType.Ending, -1);
                }
                else
                {
                    nextScene = new NextScene(SceneType.StageIn, mapIndex);
                }
            }
            else if (clearSelect.GetSelect == 1)
            {     //RePlay
                nextScene = new NextScene(SceneType.PlayScene, mapIndex);
            }
            else
            {     //World
                nextScene = new NextScene(SceneType.Stage, -1);
            }
            return nextScene;
        }
    }
}
