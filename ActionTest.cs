using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

//VR用の名前空間
using Valve.VR;

public class ActionTest : MonoBehaviour
{
    //コントローラの種類（左手か右手か）を識別するための変数：handType
    public SteamVR_Input_Sources handType;

    //テレポート用のアクションが実行されているかを示す変数：teleportAction
    public SteamVR_Action_Boolean teleportAction;

    //掴むアクションが実行されているかどうかを示す変数：grabAction
    public SteamVR_Action_Boolean grabAction;
    
    //フレームごとに呼び出される関数
    void Update()
    {
        //テレポートが実行されているかどうか
        if (GetTeleportDown())
        {
            //どちらの手がテレポートを実行しているか表示
            print("Teleport " + handType);
        }

        //掴む操作が行われているかどうか
        if (GetGrab())
        {
            //どちらの手が掴む操作を実行しているか表示
            print("Grab " + handType);
        }
    }

    //テレポート用のボタンが押されているかどうかを返す関数
    public bool GetTeleportDown()
    {
        //テレポート用のボタンの状態を返す
        return teleportAction.GetStateDown(handType);
    }

    //掴む操作用のボタンが押されているかどうかを返す関数
    public bool GetGrab()
    {
        //掴む操作用のボタンの状態を返す
        return grabAction.GetState(handType);
    }
}
