/////////////////////////////////////////////////
// 設定用パラメータのクラス
// 作成時間：2016年9月24日
// By 氷見悠人
// 最終修正時間：2016年12月14日
// By 葉梨竜太
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TeamWorkGame.Def
{
    /// <summary>
    /// ゲーム内で使われているパラメータ
    /// </summary>
    public static class Parameter
    {
        //ゲーム関連
        public const int ScreenWidth = 1280;      //windowsのSize
        public const int ScreenHeight = 720;      //windowsのSize
        public const int TextWidth = 231;         //text画像の横幅
        public const int TextHeight = 50;         //text画像1つ分の縦幅、切り取り用

        //ゲーム内容関連
        public const float GForce = 0.5f;               //重力の大きさ（1フレームにつきスピードの縦分量の増量）
        public const float GroundFriction = 0.4f;         //地面摩擦
        public const float AirFriction = 0.1f;            //空中摩擦
        public const float PlayerAccelerationX = 1.5f;    //左右操作の時プレーヤーの横方向の加速度
        public const float PlayerJumpPower = 13f;
        public const float MaxPlayerHorizontalSpeed = 5f;   //Player横方向の最大速度
        public const float MaxPlayerVerticalSpeed = 10f;    //Player横方向の最大速度
        public const float MapSpeedTime = 45f;     //マップ上でのキャラクターが一回につき移動する時間（フレーム）

        //2016.12.22  by柏
        //花火関連
        public const int FireworksCount = 16;       //一回の爆発生成してくる花火の個数
        public const int FireworksReBurnCount = 2;  //再爆発回数
        public const float FireworksSpeedDownRate = 0.95f;  //一回の爆発生成してくる花火の個数
        public const int FireworksStartSpeedMin = 20;  //花火生成時の最小初速度
        public const int FireworksStartSpeedMax = 35;  //花火生成時の最大初速度
        //Clearの段階表示
        public const int ClearSelectLevel = 2;
        public const int ClearFireworksLevel = 1;

        //葉梨竜太
        //public const int FireUpSpeed = 15;            　　　↓　  //真上に投げる時のSpeed；
        //public const int FireHorizontalSpeedY = 12;  　　　 ↓    //横に投げる時のY方向Speed分量
        //public const int FireHorizontalSpeedX = 12;　　　　 ↓　  //横に投げる時のX方向Speed分量
        public const int FireSpeed = 11;　　　　　　　　//火の速度（全方向一緒）
        public const int FireFly = 6;                   //火の飛ぶ距離
        //葉梨竜太
        public const int FireFall = 10;                  //火の落ちる速度

        //ギミック関連
        public const int FireMaxNum = 5;                //火の総量   
        public const int FirstFireNum = 2;           //最初の火の量
        public const int MoveLightSpeed = 3;        //動く松明の速度
        public const int BalloonDown = 3;           //気球が下降する速度
        public const int BalloonUp = 3;            //気球が上昇する速度  
        public const int BalloonMove = 3;           //気球の移動速度 ２０１６．１２．７   Ｂｙ柏
        public const float StrawColTime = 0.03f;   //藁ブロックの当たり判定が消えるまでの時間
        public const float StrawBurn = 0.2f;       //藁ブロックの火が燃え移り始めるまでの時間        ※11/30現在、当たり判定がなくなってから火が燃え移り始める。
        public const float StrawAnimeTime = 0.5f;  //藁ブロックアニメ―ションの時間
        public const float TreeColTime = 0.3f;    //木ブロックの当たり判定が消えるまでの時間
        public const float TreeAnimeTime = 2.0f;   //木ブロックアニメーションの時間
        public const float WoodAnimeTime = 1.0f;   //木材ブロックアニメーションの時間
        public const float WoodSpawnTime = 2.5f;   //木材ブロックの復活時間
        public const float IronAnimeTime = 1.0f;   //鉄ブロックアニメーションの時間
        public const float IronSpawnTime = 4.0f;   //鉄ブロックの復活時間
        public const float IceAnimeTime = 1.0f;    //アイスブロックアニメーションの時間
        public const float IceSpawnTime = 3.5f;    //アイスブロックの復活時間
        public const float WaterFlowSpeed = 0.3f;  //水が下のマスに流れるまでの時間
        public const float BombColTime = 2.0f;     //爆弾が消えるまでの時間
        public const float BombAnimeTime = 0.5f;   //爆発エフェクトのアニメーション時間

        //必要な火の量(数字以上で燃える)
        public const int icefire = 2;
        public const int treefire = 3;
        public const int ironfire = 5;
        public const int woodfire = 1;
        public const int strawfire = 1;

        //カメラのスケール
        public const float CameraScale = 1.0f;

        //操作関連
        public const Keys JumpKey = Keys.Z;
        public const Keys TeleportKey = Keys.C;
        public const Keys ThrowKey = Keys.X;
        public const Keys MenuKey = Keys.Enter;
        public const Keys CameraKey = Keys.LeftShift;

        public const Buttons JumpButton = Buttons.A;
        public const Buttons TeleportButton = Buttons.LeftTrigger;
        public const Buttons ThrowButton = Buttons.RightTrigger;
        public const Buttons MenuButton = Buttons.Start;
        public const Buttons CameraButton = Buttons.LeftShoulder;

        //UI関連
        public static readonly Vector2 CoalPosition = new Vector2(1070, 60);

        public static readonly List<GimmickType> ImpassableTiles = new List<GimmickType>()
        {
            GimmickType.GROUND1, GimmickType.GROUND2,
        };

        public static readonly List<GimmickType> PlatformTiles = new List<GimmickType>()
        {
            
        };

        public static readonly List<GimmickType> PassableTiles = new List<GimmickType>()
        {
            
        };
    }
}
