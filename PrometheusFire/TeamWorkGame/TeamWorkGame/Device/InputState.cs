///////////////////////////////////////////////////////////////
// キーボード and GamePad入力処理
// 作成時間：2016/9/22
// 作成者：氷見悠人
// 修正時間：2016/10/26
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TeamWorkGame.Device
{
    /// <summary>
    /// キーボード操作を処理するクラス
    /// </summary>
    class InputState
    {
        //フィールド
        private Vector2 velocity;   // 移動量の宣言
        private Vector2 rightVelocity;  //カメラ移動用

        //キー
        private KeyboardState currentKey;   //現在のキー
        private KeyboardState previousKey;  //１フレーム前のキー
        private GamePadState currentPad;
        private GamePadState previousPad;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InputState()
        {
            velocity = Vector2.Zero;
            rightVelocity = Vector2.Zero;
        }

        /// <summary>
        /// 移動量の取得
        /// </summary>
        /// <returns>移動量</returns>
        public Vector2 Velocity()
        {
            return velocity;
        }

        public Vector2 RightVelocity()
        {
            return rightVelocity;
        }

        /// <summary>
        /// 移動量の更新
        /// </summary>
        /// <param name="keyState">キーボードの状態</param>
        private void UpdateVelocity()
        {
            velocity = Vector2.Zero;    // 移動量をゼロで初期化

            // 十字キーの操作処理
            if (CheckDownKey(Keys.Right, Buttons.LeftThumbstickRight))
            {
                velocity.X = 1.0f;
            }
            if (CheckDownKey(Keys.Left, Buttons.LeftThumbstickLeft))
            {
                velocity.X = -1.0f;
            }

            if (CheckDownKey(Keys.Up, Buttons.LeftThumbstickUp))
            {
                velocity.Y = -1.0f;
            }
            if (CheckDownKey(Keys.Down, Buttons.LeftThumbstickDown))
            {
                velocity.Y = 1.0f;
            }

            // 正規化する（長さ１の単位ベクトルに）
            //if (velocity.Length() != 0.0f)
            //{
            //    velocity.Normalize();   // 正規化メソッド
            //}
        }
        
        private void UpdateRightVelocity()
        {
            rightVelocity = Vector2.Zero;    // 移動量をゼロで初期化

            // 十字キーの操作処理
            if (CheckDownKey(Keys.D, Buttons.RightThumbstickRight))
            {
                rightVelocity.X += 1.0f;
            }
            if (CheckDownKey(Keys.A, Buttons.RightThumbstickLeft))
            {
                rightVelocity.X += -1.0f;
            }

            if (CheckDownKey(Keys.W, Buttons.RightThumbstickUp))
            {
                rightVelocity.Y += -1.0f;
            }
            if (CheckDownKey(Keys.S, Buttons.RightThumbstickDown))
            {
                rightVelocity.Y += 1.0f;
            }

            //正規化する（長さ１の単位ベクトルに）
            if (rightVelocity.Length() != 0.0f)
            {
                rightVelocity.Normalize();   // 正規化メソッド
            }
        }

        /// <summary>
        /// キー情報の更新
        /// </summary>
        /// <param name="keyState"></param>
        private void UpdateKey(KeyboardState keyState)
        {
            //現在登録されているキーを１フレーム前のキーに
            previousKey = currentKey;
            //現在のキーを最新のキーに
            currentKey = keyState;
        }

        private void UpdatePad(GamePadState buttonState)
        {
            //現在登録されているキーを１フレーム前のキーに
            previousPad = currentPad;
            //現在のキーを最新のキーに
            currentPad = buttonState;
        }

        public bool CheckTriggerKey(Keys key, Buttons button)
        {
            //キーボードでチェックしたいキーが押されているか？
            if (IsKeyDown(key))
            {
                return true;
            }

            //キーパッドのキーがつながっていないか？
            if (!GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                //接続されなければチェック終了
                return false;
            }

            //キーパッドでチェックしたいボタンが押されているか？
            if (IsKeyDown(button))
            {
                return true;
            }
            return false;
        }

        public bool CheckDownKey(Keys key, Buttons button)
        {
            //キーボードでチェックしたいキーが押されているか？
            if (currentKey.IsKeyDown(key))
            {
                return true;
            }

            //キーパッドのキーがつながっていないか？
            if (!GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                //接続されなければチェック終了
                return false;
            }

            //キーパッドでチェックしたいボタンが押されているか？
            if (GamePad.GetState(PlayerIndex.One).IsButtonDown(button))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// キーが押されているか？
        /// </summary>
        /// <param name="key">調べたいキー</param>
        /// <returns>現在押されていて、１フレーム前に押されていなければ true</returns>
        public bool IsKeyDown(Keys key)
        {
            //現在チェックしたいキーが押されたか
            bool current = currentKey.IsKeyDown(key);
            //１フレーム前に押されていたか
            bool previous = previousKey.IsKeyDown(key);

            //現在押されていて、１フレーム前に押されていなければ true
            return current && !previous;
        }

        public bool IsKeyDown(Buttons button)
        {
            //現在チェックしたいキーが押されたか
            bool current = currentPad.IsButtonDown(button);
            //１フレーム前に押されていたか
            bool previous = previousPad.IsButtonDown(button);

            //現在押されていて、１フレーム前に押されていなければ true
            return current && !previous;
        }

        /// <summary>
        /// キー入力のトリガー判定
        /// </summary>
        /// <param name="key"></param>
        /// <returns>１フレーム前に押されていたらfalse</returns>
        public bool GetKeyTrigger(Keys key)
        {
            return IsKeyDown(key);
        }

        public bool GetKeyTrigger(Buttons button)
        {
            return IsKeyDown(button);
        }

        /// <summary>
        /// キー入力の状態判定
        /// </summary>
        /// <param name="key"></param>
        /// <returns>押されていたらtrue</returns>
        public bool GetKeyState(Keys key)
        {
            return currentKey.IsKeyDown(key);
        }

        public bool GetKeyState(Buttons button)
        {
            return currentPad.IsButtonDown(button);
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            // 現在のキーボードの状態を取得
            var keyState = Keyboard.GetState();
            UpdateKey(keyState);

            var padState = GamePad.GetState(PlayerIndex.One);
            UpdatePad(padState);

            // 移動量の更新
            UpdateVelocity();
            UpdateRightVelocity();
        }
    }
}
