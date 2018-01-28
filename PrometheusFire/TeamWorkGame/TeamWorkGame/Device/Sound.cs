using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;  //コンテンツ利用
using Microsoft.Xna.Framework.Audio;    //WAVデータ
using Microsoft.Xna.Framework.Media;    //MP3データ
using System.Diagnostics;               //Assert

namespace TeamWorkGame.Device
{
    public class Sound
    {
        private ContentManager contentManager;
        private Dictionary<string, Song> bgms;                          //MP3管理用
        private Dictionary<string, SoundEffect> soundEffects;           //WAV管理用
        private Dictionary<string, SoundEffectInstance> seInstances;    //WAVインスタンス管理用（WAVの高度な利用）
        private List<SoundEffectInstance> sePlayList;                   //WAVインスタンスの再生リスト

        //現在再生中のアセット名
        private string currentBGM;  
        //private string currentSE;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="content">Game1のコンテンツ管理者</param>
        public Sound(ContentManager content)
        {
            //Game1のコンテンツ管理者と繋ぐ
            contentManager = content;
            //BGMは繰り返し再生
            MediaPlayer.IsRepeating = true;

            //各Dictionaryの実体生成
            bgms = new Dictionary<string, Song>();
            soundEffects = new Dictionary<string, SoundEffect>();
            seInstances = new Dictionary<string, SoundEffectInstance>();

            //再生Listの実体生成
            sePlayList = new List<SoundEffectInstance>();

            //何も再生していないのでnull初期化
            currentBGM = null;
            //currentSE = null;
        }

        /// <summary>
        /// Assert用メッセージ
        /// </summary>
        /// <param name="name">アセット名</param>
        /// <returns></returns>
        private string ErrorMessage(string name)
        {
            return "再生する音データのアセット名（" + name + "）がありません\n" 
                +
                  "アセット名の確認、Dictionaryに登録されているか確認してください\n";
        }

        #region BGM関連処理
        /// <summary>
        /// BGM(MP3)の読み込み
        /// </summary>
        /// <param name="name">アセット名</param>
        /// <param name="filepath">ファイルへのパス</param>
        public void LoadBGM(string name, string filepath = "./")
        {
            //すでに登録されているか？
            if(bgms.ContainsKey(name))
            {
                return;
            }
            //MP3の読み込みと、Dictionaryへの登録
            bgms.Add(name, contentManager.Load<Song>(filepath + name));
        }

        /// <summary>
        /// BGM再生中か？
        /// </summary>
        /// <returns>再生中だったらtrue</returns>
        public bool IsPlayingBGM()
        {
            return (MediaPlayer.State == MediaState.Playing);
        }

        /// <summary>
        /// BGMが停止しているか？
        /// </summary>
        /// <returns></returns>
        public bool IsStoppedBGM()
        {
            return (MediaPlayer.State == MediaState.Stopped);
        }

        /// <summary>
        /// BGM一時停止中か？
        /// </summary>
        /// <returns>一時停止中だったらtrue</returns>
        public bool IsPausedBGM()
        {
            return (MediaPlayer.State == MediaState.Paused);
        }

        /// <summary>
        /// BGMを停止
        /// </summary>
        public void StopBGM()
        {
            MediaPlayer.Stop();
            currentBGM = null;
        }

        /// <summary>
        /// BGMループフラグの変更
        /// </summary>
        /// <param name="loopFlag"></param>
        public void ChangeBGMLoopFlag(bool loopFlag)
        {
            MediaPlayer.IsRepeating = loopFlag;
        }

        /// <summary>
        /// BGM再生
        /// </summary>
        /// <param name="name"></param>
        public void PlayBGM(string name)
        {
            Debug.Assert(bgms.ContainsKey(name), ErrorMessage(name));

            //同じ曲か？
            if(currentBGM == name)
            {
                //同じ曲だったら何もしない
                return;
            }

            //BGMは再生中か？
            if(IsPlayingBGM())
            {
                //再生中の場合、停止処理をする
                StopBGM();
            }

            //ボリューム設定
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.IsRepeating = true;
            //現在のBGM名を設定
            currentBGM = name;

            //再生開始
            MediaPlayer.Play(bgms[currentBGM]);
        }
        #endregion

        #region WAV関連
        /// <summary>
        /// SE(wav)の読み込み
        /// </summary>
        /// <param name="name">wavのアセット名</param>
        /// <param name="filepath">ファイルのパス</param>
        public void LoadSE(string name, string filepath = "./")
        {
            //すでに登録されていれば何もしない
            if(soundEffects.ContainsKey(name))
            {
                return;
            }

            //読み込みと追加
            soundEffects.Add(name, contentManager.Load<SoundEffect>(filepath + name));
        }

        /// <summary>
        /// 高度なwav再生のためのインスタンス化
        /// </summary>
        /// <param name="name">wavアセット名</param>
        public void CreateSEInstance(string name)
        {
            //すでに登録されていれば何もしない
            if(seInstances.ContainsKey(name))
            {
                return;
            }

            //WAV用ディクションナリに登録されていないとムリ
            Debug.Assert(soundEffects.ContainsKey(name), "先に" + name + "の読み込み処理をしてください");

            //WAVデータのインスタンス
            seInstances.Add(name, soundEffects[name].CreateInstance());
        }

        /// <summary>
        /// 単純SE再生（連続で呼ばれた場合、おとは重なる。途中停止不可）
        /// </summary>
        /// <param name="name"></param>
        public void PlaySE(string name)
        {
            //WAV用ディクションナリをチェック
            Debug.Assert(soundEffects.ContainsKey(name), ErrorMessage(name));

            soundEffects[name].Play();
        }

        public bool IsPlayingSEInstance(string name)
        {
            Debug.Assert(soundEffects.ContainsKey(name), ErrorMessage(name));

            return (seInstances[name].State == SoundState.Playing);
        }

        /// <summary>
        /// インスタンス化されたSEの再生
        /// </summary>
        /// <param name="name"></param>
        public void PlaySEInstance(string name, bool loopFlag = false)
        {
            //WAVインスタンス用ディクションナリをチェック
            Debug.Assert(seInstances.ContainsKey(name), ErrorMessage(name));

            //同じ音を再生しようとしているか？
            var data = seInstances[name];

            foreach(var x in sePlayList.FindAll(x => x == data))
            {
                if(x.State == SoundState.Playing)
                {
                    x.Stop();
                }
            }

            if(data.IsLooped != loopFlag)
            {
                data.IsLooped = loopFlag;
            }

            data.Play();

            sePlayList.Add(data);
            ////再生中ですか？
            //if (IsPlayingSEInstance(name))
            //{
            //    StopSEInstance(currentSE);
            //}

            //currentSE = name;

            //seInstances[currentSE].Play();
        }

        /// <summary>
        /// インスタンス化されたSE音の停止
        /// </summary>
        /// <param name="name"></param>
        //public void StopSEInstance(string name)
        //{
        //    //WAVインスタンス用ディクションナリをチェック
        //    Debug.Assert(seInstances.ContainsKey(name), ErrorMessage(name));

        //    seInstances[name].Stop();
        //    currentSE = null;
        //}

        /// <summary>
        /// sePlayListにある再生中の音を停止
        /// </summary>
        public void StoppedSE()
        {
            foreach(var se in sePlayList)
            {
                if(se.State == SoundState.Playing)
                {
                    se.Stop();
                }
            }
        }

        /// <summary>
        /// sePlayListにある再生中の音を一時停止
        /// </summary>
        /// <param name="name"></param>
        public void PausedSE(string name)
        {
            foreach(var se in sePlayList)
            {
                if(se.State == SoundState.Playing)
                {
                    se.Pause();
                }
            }
        }

        /// <summary>
        /// 停止している音の削除
        /// </summary>
        public void RemoveSE()
        {
            //停止中のモノはListから削除
            sePlayList.RemoveAll(se => (se.State == SoundState.Stopped));
        }
        #endregion

        /// <summary>
        /// 解放
        /// </summary>
        public void Unload()
        {
            bgms.Clear();
            soundEffects.Clear();
            sePlayList.Clear();
        }

        public void Update()
        {
            RemoveSE();
            //if (currentSE != null)
            //{
            //    if (seInstances[currentSE].State == SoundState.Stopped)
            //    {
            //        currentSE = null;
            //    }
            //}
            //if (currentBGM != null)
            //{
            //    if (MediaPlayer.State == MediaState.Stopped)
            //    {
            //        currentBGM = null;
            //    }
            //}
        }
    }
}
