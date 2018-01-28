////////////////////////////////////////////////////////////////////////////////
// 水滝のクラス
// 作成者：氷見悠人
//////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TeamWorkGame.Device;
using TeamWorkGame.Def;
using TeamWorkGame.Scene;
using TeamWorkGame.Utility;

namespace TeamWorkGame.Actor
{
    public class WaterLine
    {
        private List<Water> waters; //沢山の水
        private Size waterSize;
        private Vector2 position;   //左上の座標
        private Timer timer;
        private Map mapdata;
        private bool isCollisioned;
        private bool isDead;
        private Timer startTimer;        

        public bool IsDead
        {
            get
            {
                return isDead;
            }
        }

        public List<Water> Waters
        {
            get
            {
                return waters;
            }
        }

        /// <summary>
        /// 氷から生成する水滝
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="anime"></param>
        public WaterLine(Vector2 pos, Animation anime)
        {
            position = pos;
            waters = new List<Water>();
            mapdata = MapManager.GetNowMapData();
            timer = new Timer(Parameter.WaterFlowSpeed);
            startTimer = new Timer((anime.FrameCount-1) * anime.FrameTime);
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            waters.Clear();
            isDead = false;
            //waters.Add(new Water(position, Vector2.Zero));
            waterSize = new Size(64, 64);
            timer.CurrentTime = 0;
            startTimer.Initialize();
        }

        public void Update(GameTime gameTime)
        {
            startTimer.Update();
            if (startTimer.IsTime())
            {
                timer.Update();
                if (timer.IsTime())
                {
                    if (isCollisioned)
                    {
                        RemoveWater();
                    }
                    else
                    {
                        MakeWater();
                        isCollisioned = CheckNextMapColision() || CheckNextThingsColision();
                    }
                    timer.Initialize();
                }
            }
        }

        public void Draw(GameTime gameTime, Renderer renderer, Vector2 offset, float cameraScale) 
        {
            foreach(var w in waters)
            {
                w.Draw(gameTime, renderer, offset, cameraScale);
            }
        }

        private void RemoveWater()
        {
            if (waters.Count > 0)
            {
                waters.RemoveAt(0);
            }
            else
            {
                isDead = true;
            }
        }

        private void MakeWater()
        {
            Water water = new Water(position + (new Vector2(0, 64)) * waters.Count, Vector2.Zero, 1);
            waters.Add(water);
        }

        private bool CheckNextThingsColision()
        {
            bool flag = false;
            Vector2 pos = position + (new Vector2(0, 64)) * waters.Count;

            foreach (var x in mapdata.MapThings.FindAll(y => !y.IsTrigger))
            {
                if (Method.CollisionCheck(pos, waterSize.Width, waterSize.Height, x.Position, x.ColRect.Width, x.ColRect.Height))
                {
                    flag = true;
                }
            }

            return flag;
        }

        private bool CheckNextMapColision()
        {
            bool flag = false;
            Vector2 pos = position + (new Vector2(0, 64)) * waters.Count;
            int nextPosType = mapdata.GetBlockData(pos);

            if(StageDef.ImpassableTiles.Contains((GimmickType)nextPosType) || StageDef.PlatformTiles.Contains((GimmickType)nextPosType))
            {
                flag = true;
            }
            else if(pos.Y > mapdata.MapHeight)
            {
                flag = true;
            }
            return flag;
        }
    }
}
