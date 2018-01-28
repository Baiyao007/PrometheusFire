////////////////////////
//作　成：2016.11.02
//作成者：柏
///////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using TeamWorkGame.Def;

namespace TeamWorkGame.Device
{
    class StageSaver
    {
        private int playTime;
        private int currentStage;
        private int clearState;
        private int clearStage;
        private int charcoal;
        private int[][] stageData;

        public StageSaver()
        {
            Initialize();
        }


        /// <summary>
        /// Data構造:
        /// playerTime(秒),clearState(0:false,1:true),charcoal
        /// </summary>
        public void Initialize()
        {
            playTime = 0;
            currentStage = 0;
            clearState = 0;
            clearStage = -1;
            charcoal = 0;
            stageData = new int[StageDef.BigIndexMax * StageDef.SmallIndexMax + 1][];

            for (int i = 0; i < StageDef.BigIndexMax * StageDef.SmallIndexMax + 1; i++)
            {
                stageData[i] = new int[3];
                for (int j = 0; j < 3; j++)
                {
                    stageData[i][j] = 0;
                }
            }
        }

        public void LoadStageData()
        {
            FileStream fileStream = new FileStream("Content/Stage/StageData.csv", FileMode.OpenOrCreate);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.GetEncoding("Shift_JIS"));

            List<string> lines = new List<string>();
            string[] splitLine = new string[3]; //要素数

            for (int i = 0; i < StageDef.BigIndexMax * StageDef.SmallIndexMax + 1; i++)
            {
                lines.Add(streamReader.ReadLine());
            }

            splitLine = lines[0].Split(',');

            if (clearStage <= int.Parse(splitLine[0]))
            {
                clearStage = int.Parse(splitLine[0]);
            }

            for (int i = 1; i < StageDef.BigIndexMax * StageDef.SmallIndexMax + 1; i++)
            {
                splitLine = lines[i].Split(',');

                for (int j = 0; j < splitLine.Count(); j++) {
                    int check;
                    if (!int.TryParse(splitLine[j],out check))
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            stageData[i][x] = 0;
                        }
                        continue;
                    }
                    stageData[i][j] = int.Parse(splitLine[j]);
                    

                }
                
            }


            streamReader.Close();
            fileStream.Close();
        }

        public void SaveStageData()
        {
            LoadStageData();
            FileStream fileStream = new FileStream("Content/Stage/StageData.csv", FileMode.OpenOrCreate);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding("Shift_JIS"));

            stageData[0][0] = clearStage;

            stageData[currentStage + 1][0] = playTime;
            stageData[currentStage + 1][1] = 1;
            stageData[currentStage + 1][2] = charcoal;


            for (int i = 0; i < StageDef.BigIndexMax * StageDef.SmallIndexMax + 1; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    streamWriter.Write(stageData[i][j] + ",");
                }
                streamWriter.WriteLine();
            }

            streamWriter.Close();
            fileStream.Close();
        }


        public int[] TimeCalculat(int time)
        {
            int second = time % 60;
            int minute = time / 60;
            int[] times = new int[2] { minute, second };
            return times;
        }


        public int PlayTime
        {
            get { return playTime; }
            set { playTime = value; }
        }

        public int ClearStage {
            get { return clearStage; }
            set { clearStage = value; }
        }

        public int CurrentStage {
            get { return currentStage; }
            set { currentStage = value; }
        }

        public int Charcoal {
            get { return charcoal; }
            set { charcoal = value; }
        }

    }
}
