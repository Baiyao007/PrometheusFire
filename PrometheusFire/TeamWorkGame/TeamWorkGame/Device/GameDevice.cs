///////////////////////////////////////////////////////////////////////////
//ゲーム中使われている機能の統合
//作成時間：2016/9/22
//作成者：氷見悠人
//最終更新日:2016/12/08
//編集者:佐瀬拓海
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TeamWorkGame.Def;

namespace TeamWorkGame.Device
{
    class GameDevice
    {
        private Renderer renderer;  //描画
        private InputState input;   //入力
        private Sound sound;        //サウンド
        private static Random rand = new Random(); //乱数
        private StageSaver stageSaver;  //stageごとのclear状態を保存する用

        public GameDevice(ContentManager content, GraphicsDevice graphics)
        {
            renderer = new Renderer(content, graphics);
            input = new InputState();
            sound = new Sound(content);
            stageSaver = new StageSaver();
        }

        public void Update(GameTime gameTime)
        {
            //デバイスで絶対に更新が必要なもの
            input.Update();
        }

        public void LoadContent()
        {
            //ゲーム開始時に必要な最小限のリソースを読み込む
            renderer.LoadTexture("number", "./Texture/");
            renderer.LoadTexture("logo", "./Texture/");
            renderer.LoadTexture("teamlogo", "./Texture/");     //チームロゴの読み込み
        }


        /// <summary>
        /// 描画オブジェクトの取得
        /// </summary>
        /// <returns></returns>
        public Renderer GetRenderer()
        {
            return renderer;
        }

        /// <summary>
        /// 入力オブジェクトの取得
        /// </summary>
        /// <returns></returns>
        public InputState GetInputState()
        {
            return input;
        }

        /// <summary>
        /// サウンドオブジェクトの取得
        /// </summary>
        /// <returns></returns>
        public Sound GetSound()
        {
            return sound;
        }

        /// <summary>
        /// 乱数オブジェクトの取得
        /// </summary>
        /// <returns></returns>
        public Random GetRandom()
        {
            return rand;
        }

        /// <summary>
        /// stageSeverの取得
        /// </summary>
        /// <returns></returns>
        public StageSaver GetStageSaver() {
            return stageSaver;
        }

    }

}
