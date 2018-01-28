/////////////////////////////////////////////////////////////////////////////
// カメラ処理
// 作成時間：2016/9/25
// 作成者：氷見悠人
/////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;  //Vector2
using TeamWorkGame.Def;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Device
{
    public class Camera
    {
        //フィールド 
        //public int ViewWidth { get; }
        //public int ViewHeight { get; }

        private Vector2 position;
        private Vector2 aimPosition;
        private Map map;
        private bool IsLimitView;
        private float scale;

        public float Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }
        public Vector2 OffSet
        {
            get
            {
                return -position;
            }
        }

        public Vector2 AimPosition
        {
            get
            {
                return aimPosition;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="aimPos">カメラ注視位置（中心位置）</param>
        public Camera()
        {
            scale = 1.0f;
            //ViewWidth = (int)Math.Round(Parameter.ScreenWidth * scale);
            //ViewHeight = (int)Math.Round(Parameter.ScreenHeight * scale);
            map = MapManager.GetNowMapData();
            IsLimitView = true;
            SetData(new Vector2(Parameter.ScreenWidth / 2, Parameter.ScreenHeight / 2));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="aimPos">カメラ注視位置（中心位置）</param>
        public Camera(Vector2 aimPos, bool isLimitView = false)
        {
            scale = 1.0f;
            //ViewWidth = (int)Math.Round(Parameter.ScreenWidth * scale);
            //ViewHeight = (int)Math.Round(Parameter.ScreenHeight * scale);
            map = MapManager.GetNowMapData();
            IsLimitView = isLimitView;
            SetData(aimPos);
        }

        public Camera(Vector2 aimPos, float scale, bool isLimitView = false)
        {
            this.scale = scale;
            //ViewWidth = (int)Math.Round(Parameter.ScreenWidth * scale);
            //ViewHeight = (int)Math.Round(Parameter.ScreenHeight * scale);
            map = MapManager.GetNowMapData();
            IsLimitView = isLimitView;
            SetData(aimPos * scale);
        }

        

        /// <summary>
        /// カメラの各位置を設定
        /// </summary>
        /// <param name="aimPos">注視する位置</param>
        private void SetData(Vector2 aimPos)
        {
            //bool isOut = false;
            if (IsLimitView)
            {
                if (map.MapWidth >= Parameter.ScreenWidth)
                {
                    // Cameraの位置を制限する
                    if (aimPos.X < Parameter.ScreenWidth / 2)
                    {
                        aimPos.X = Parameter.ScreenWidth / 2;
                        //isOut = true;
                    }
                    if (aimPos.X > map.MapWidth * scale - Parameter.ScreenWidth / 2)
                    {
                        aimPos.X = map.MapWidth * scale - Parameter.ScreenWidth / 2;
                        //isOut = true;
                    }
                }
                else
                {
                    aimPos.X = map.MapWidth * scale / 2;
                    //isOut = true;
                }

                if (aimPos.Y < Parameter.ScreenHeight / 2)
                {
                    aimPos.Y = Parameter.ScreenHeight / 2;
                    //isOut = true;
                }
                if (aimPos.Y > map.MapHeight * scale - Parameter.ScreenHeight / 2)
                {
                    aimPos.Y = map.MapHeight * scale - Parameter.ScreenHeight / 2;
                    //isOut = true;
                }
            }

            aimPosition = aimPos;
            //centerPosition = aimPosition;
            position = aimPosition - new Vector2(Parameter.ScreenWidth / 2, Parameter.ScreenHeight / 2);

            //return isOut;
        }

        /// <summary>
        /// カメラの注視位置を設定
        /// </summary>
        /// <param name="aimPos"></param>
        public void SetAimPosition(Vector2 aimPos)
        {
            SetData(aimPos * scale);
        }

        /// <summary>
        /// カメラを移動する
        /// </summary>
        /// <param name="aimPos"></param>
        //public void MoveAimPosition(Vector2 aimPos)
        //{
        //    Vector2 distance = aimPos * scale - aimPosition;
        //    //float speed = 0;
        //    Vector2 aim;

        //    Vector2 velocity = distance * 0.9f;

        //    //カメラの位置を整数化
        //    velocity.X = (float)Math.Floor((velocity.X));
        //    velocity.Y = (float)Math.Floor((velocity.Y));

        //    aim = aimPos * scale - velocity;

        //    SetData(aim);
        //}

        /// <summary>
        /// カメラを移動する
        /// </summary>
        /// <param name="aimPos">次の注視位置</param>
        public void MoveAimPosition(Vector2 aimPos)
        {
            Vector2 distance = aimPos * scale - aimPosition;
            //float speed = 0;
            Vector2 aim;

            Vector2 velocity = distance * 0.1f;

            //カメラの位置を整数化
            if (velocity.X > 0)
                velocity.X = (float)Math.Ceiling((velocity.X));
            else if (velocity.X < 0)
                velocity.X = (float)Math.Floor((velocity.X));

            if (velocity.Y > 0)
                velocity.Y = (float)Math.Ceiling((velocity.Y));
            else if (velocity.Y < 0)
                velocity.X = (float)Math.Floor((velocity.X));

            aim = aimPosition + velocity;

            SetData(aim);
        }

        public void CameraShake()
        {

        }

        /// <summary>
        /// カメラ注視位置と注視物の距離により、カメラの移動速度を計算し、速度を返す
        /// </summary>
        /// <param name="distance">カメラ注視位置と注視物位置の距離</param>
        /// <returns>カメラの移動速度</returns>
        private float GetMoveSpeed(float distance)
        {
            float speed;
            speed = 0.1f * distance;
            return speed;
        }

        public void UpdateMap()
        {
            map = MapManager.GetNowMapData();
        }

        public void SetLimitView(bool flag)
        {
            IsLimitView = flag;
        }
    }
}
