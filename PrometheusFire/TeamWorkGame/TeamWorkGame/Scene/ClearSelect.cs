/////////////////////////////////////////////
//　クリア画面
//  作成者：柏杳
//
//  最終更新日 2016.12.14
//  by 柏　 ＳＥ実装
/////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TeamWorkGame.Device;
using TeamWorkGame.Actor;
using TeamWorkGame.Def;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Scene
{
    class ClearSelect
    {
        private InputState inputState;  //入力チェック
        private int select;     //選択肢
        private List<Vector2> selected; //選択肢登録
        private bool isClear;   //clear状態
        private bool isPause;
        private bool isEnd;     //選択完了状態
        private Player player;
        private ArmsUp armsUp;
        //Animation関連
        private Animation standAnime;
        private AnimationPlayer animePlayer;
        private SpriteEffects flip = SpriteEffects.None;

        private Vector2 worldTextPosition;
        private Vector2 retryTextPosition;
        private Vector2 nextTextPosition;

        private Sound sound;   //by 柏　2016.12.14 ＳＥ実装

        private Timer flashTimer;
        private float retryTextalpha;
        private float worldTextalpha;
        private float nextTextalpha;

        public ClearSelect(InputState inputState, Player player, Sound sound)
        {
            this.sound = sound;   //by 柏　2016.12.14 ＳＥ実装
            this.inputState = inputState;
            this.player = player;
            armsUp = new ArmsUp(new Vector2(560, 210), Vector2.Zero);
            Initialize();
        }

        public void Initialize()
        {
            if (player.IsDead)
            {
                selected = new List<Vector2>() {
                Vector2.Zero,
                new Vector2(470,440),
                new Vector2(470,540),
                };
                select = 1;
            }
            else {
                selected = new List<Vector2>() {
                new Vector2(470,340),
                new Vector2(470,440),
                new Vector2(470,540),
                };
                select = 0;
            }

            worldTextPosition = new Vector2(550, 550);
            retryTextPosition = new Vector2(550, 450);
            nextTextPosition = new Vector2(550, 350);

            flashTimer = new Timer(0.2f);
            retryTextalpha = 1.0f;
            worldTextalpha = 1.0f;
            nextTextalpha = 1.0f;

            standAnime = new Animation(Renderer.GetTexture("standAnime"), 0.1f, true);//StandAnimeの実体生成

            isPause = false;
            isClear = false;
            isEnd = false;
        }

        //点滅する処理
        private void Flash()
        {
            flashTimer.Update();
            if (flashTimer.IsTime())
            {
                switch (select)
                {
                    case 0:
                        worldTextalpha = 1.0f;
                        retryTextalpha = 1.0f;
                        if (nextTextalpha == 1.0f) { nextTextalpha = 0.5f; flashTimer.Initialize(); }
                        else { nextTextalpha = 1.0f; flashTimer.Initialize(); }
                        break;
                    case 1:
                        worldTextalpha = 1.0f;
                        nextTextalpha = 1.0f;
                        if (retryTextalpha == 1.0f) { retryTextalpha = 0.5f; flashTimer.Initialize(); }
                            else { retryTextalpha = 1.0f; flashTimer.Initialize(); }
                            break;
                    case 2:
                        retryTextalpha = 1.0f;
                        nextTextalpha = 1.0f;
                        if (worldTextalpha == 1.0f) { worldTextalpha = 0.5f; flashTimer.Initialize(); }
                        else { worldTextalpha = 1.0f; flashTimer.Initialize(); }
                        break;
                }
            }
        }

        public void Update()
        {
            Flash();
            if (isClear)    //clear状態だけ選択有効
            {
                if (inputState.CheckTriggerKey(Keys.Up, Buttons.LeftThumbstickUp))
                {
                    sound.PlaySE("cursor");    //by 柏　2016.12.14 ＳＥ実装
                    if (player.IsDead || (!player.IsDead && isPause))
                    {
                        if (select == 1) { return; }
                    }
                    else {
                        if (select == 0) { return; }
                    }
                    select--;
                }
                else if (inputState.CheckTriggerKey(Keys.Down, Buttons.LeftThumbstickDown))
                {
                    sound.PlaySE("cursor");    //by 柏　2016.12.14 ＳＥ実装
                    if (select == 2) { return; }
                    select++;
                }
                
                //選択確定
                //keyの変更　By葉梨竜太 11/30
                else if (inputState.CheckTriggerKey(Parameter.MenuKey, Parameter.JumpButton) || inputState.IsKeyDown(Keys.Z) || inputState.IsKeyDown(Keys.Space))
                {
                    isEnd = true;
                }
            }
        }


        public bool IsClear
        {
            get { return isClear; }
            set { isClear = value; }
        }

        public bool IsPause {
            get { return isPause; }
            set { isPause = value; }
        }

        public int GetSelect
        {
            get { return select; }
            set { select = value; }
        }

        public bool IsEnd
        {
            get { return isEnd; }
        }

        public void Draw(GameTime gameTime, Renderer renderer, float cameraScale)
        { //Animationをさせるため引数を変更
            if (isClear)
            {
                if (player.IsDead)
                {
                    renderer.DrawTexture("text", new Vector2(1280 / 2 - 152 / 2, 250), new Rectangle(0, (int)Text.MISS * Parameter.TextHeight, Parameter.TextWidth, Parameter.TextHeight));
                    renderer.DrawTexture("text", retryTextPosition, new Rectangle(0, (int)Text.RETRY * Parameter.TextHeight, Parameter.TextWidth, Parameter.TextHeight), retryTextalpha);
                    renderer.DrawTexture("text", worldTextPosition, new Rectangle(0, (int)Text.WORLD * Parameter.TextHeight, Parameter.TextWidth, Parameter.TextHeight), worldTextalpha);
                    //renderer.DrawTexture("ClearWindow2", new Vector2(350, 250));
                }
                else
                {
                    if (isPause)
                    {
                        renderer.DrawTexture("Pause", new Vector2(1280 / 2 - 472 / 2, 100));
                        renderer.DrawTexture("text", retryTextPosition, new Rectangle(0, (int)Text.RETRY * Parameter.TextHeight, Parameter.TextWidth, Parameter.TextHeight), retryTextalpha);
                        renderer.DrawTexture("text", worldTextPosition, new Rectangle(0, (int)Text.WORLD * Parameter.TextHeight, Parameter.TextWidth, Parameter.TextHeight), worldTextalpha);
                    }
                    else {
                        //renderer.DrawTexture("ClearWindow", new Vector2(350, 250));
                        renderer.DrawTexture("text", nextTextPosition, new Rectangle(0, (int)Text.NEXT * Parameter.TextHeight, Parameter.TextWidth, Parameter.TextHeight), nextTextalpha);
                        renderer.DrawTexture("text", retryTextPosition, new Rectangle(0, (int)Text.RETRY * Parameter.TextHeight, Parameter.TextWidth, Parameter.TextHeight), retryTextalpha);
                        renderer.DrawTexture("text", worldTextPosition, new Rectangle(0, (int)Text.WORLD * Parameter.TextHeight, Parameter.TextWidth, Parameter.TextHeight), worldTextalpha);
                        armsUp.Draw(gameTime, renderer, Vector2.Zero, cameraScale);
                    }
                    
                }
                //Selecterをキャラ画像に変更
                animePlayer.PlayAnimation(standAnime);
                animePlayer.Draw(gameTime, renderer, selected[select], flip, cameraScale);
            }
        }
    }
}
