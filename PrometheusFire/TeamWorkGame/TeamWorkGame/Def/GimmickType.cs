/////////////////////////////////////////////////////////////////////////////////////
// ギミックの種類列挙型
// 作成者：氷見悠人
// 最終更新日：12月22日
// by 葉梨竜太
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamWorkGame.Def
{
    public enum GimmickType
    {
        PLAYER = 0,      //草
        GROUND1,        //土1
        GROUND2,        //土2
        ROCK,           //岩
        ICE,            //氷
        IRON,           //鉄
        STRAW,          //藁
        COAL,           //炭
        LIGHT,          //松明
        GOAL,           //ゴール
        WATER,          //水
        WOOD,           //木材
        TREE,           //木
        SAND,           //砂
        BALLOON,        //気球
        HIGHLIGHT3,     //３マス縦移動松明
        SIDELIGHT3,　　 //３マス横移動松明
        HIGHLIGHT5,     //５マス縦移動松明
        SIDELIGHT5,　　 //５マス横移動松明
        JSIGN,          //飛ぶ看板
        BOMB,           //爆弾
        MSIGN,          //動く看板
        RSIGN,          //投げる看板
        LSIGN,          //入れ替わり看板
        CSIGN,          //カメラ看板
        //葉梨竜太
        IGNITER_UL,     //導火線
        IGNITER_UR,
        IGNITER_DL,
        IGNITER_DR,
        IGNITER_HIGHT,
        IGNITER_SIDE,
        //WATERLINE,      //水滝
    }
}
