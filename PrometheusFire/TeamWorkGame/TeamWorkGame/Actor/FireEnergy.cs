//
//  回る火     By　張
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TeamWorkGame.Device;

namespace TeamWorkGame.Actor
{
    class FireEnergy : GameObject
    {
        private int fireNum;
        private List<Vector2> firePosList;
        private float mainAngle;    //回転時の角度
        private float deltaAngle;   //火の間の間隔角度
        private float radius;       //回転の半径
        private float scale;

        public FireEnergy(Vector2 centerPos, float rad) : base("fire", centerPos, Vector2.Zero, true, "FireEnergy")
        {
            fireNum = 0;
            mainAngle = 0;
            radius = rad;
            firePosList = new List<Vector2>();
            scale = 0.5f;
            position += new Vector2(-32, -32);
        }

        /// <summary>
        /// 角度から、位置を求める
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private Vector2 GetFirePositionFromAngle(float angle)
        {
            return radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) + position + (new Vector2(-32, -32)) * scale;
        }

        /// <summary>
        /// 火の位置Listを更新
        /// </summary>
        private void UpdateFirePosList()
        {
            if (firePosList.Count < fireNum)
            {
                while (firePosList.Count < fireNum)
                {
                    firePosList.Add(GetFirePositionFromAngle(mainAngle + deltaAngle * firePosList.Count));
                }
            }
            else if (firePosList.Count > fireNum)
            {
                while (firePosList.Count > fireNum)
                {
                    firePosList.RemoveAt(firePosList.Count-1);
                }
            }

            if(firePosList.Count > 0)
            {
                for(int i = 0; i < firePosList.Count; i++)
                {
                    firePosList[i] = GetFirePositionFromAngle(mainAngle + deltaAngle * i);
                }
            }
        }
        public override void EventHandle(GameObject other)
        {

        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            mainAngle += MathHelper.ToRadians(1f);
            if(mainAngle > MathHelper.ToRadians(360f))
            {
                mainAngle -= MathHelper.ToRadians(360f);
            }
            UpdateFirePosList();
        }

        /// <summary>
        /// 火の数を設定
        /// </summary>
        /// <param name="num"></param>
        public void SetFireNum(int num)
        {
            fireNum = num;
            deltaAngle = MathHelper.ToRadians((360.0f / fireNum));
            
        }

        /// <summary>
        /// 火が回る中心位置を設定
        /// </summary>
        /// <param name="pos"></param>
        public void SetCenterPosition(Vector2 pos)
        {
            position = pos + new Vector2(32, 32);
        }

        public override void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale)
        {
            foreach (var fp in firePosList)
            {
                renderer.DrawTexture("fire", fp + offset, scale, 1.0f);
            }
        }

        //public void Vanish()
        //{
        //    firePosList.Clear();
        //}
    }
}
