////////////////////////
//作　成：2016.10.27
//作成者：柏
///////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TeamWorkGame.Device
{
    class StageLoader
    {
        private int[,] mapData;

        public StageLoader() { }

        public int[,] MapLoad(int stageNumber)
        {
            try
            {
                StreamReader streamReader = new StreamReader("Content/CSV/stage" + (stageNumber + 1) + ".csv", Encoding.GetEncoding("Shift_JIS"));

                List<string> lines = new List<string>();
                string[] splitLine = null;
                while (streamReader.Peek() >= 0)
                {
                    lines.Add(streamReader.ReadLine());
                }
                splitLine = lines[0].Split(',');
                int yMax = int.Parse(splitLine[0]);
                int xMax = int.Parse(splitLine[1]);
                mapData = new int[yMax, xMax];

                for (int j = 1; j < yMax; j++)
                {
                    splitLine = lines[j].Split(',');
                    for (int i = 0; i < xMax; i++)
                    {
                        mapData[j, i] = int.Parse(splitLine[i]);
                    }
                }
                streamReader.Close();
            }
            catch (FileNotFoundException ffe)
            {
                mapData = new int[0, 0];
            }
            return mapData;
        }

    }
}
