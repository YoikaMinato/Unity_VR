using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

//コントローラの掴む操作を実現するクラス
public class GrabTest : MonoBehaviour
{
    //コントローラを表す変数：hand
    public SteamVR_Input_Sources hand;

    //コントローラの行動を表す変数：grabAction
    public SteamVR_Action_Boolean grabAction;

    //コントローラのポーズを表す変数：poseAction
    public SteamVR_Behaviour_Pose poseAction;

    //掴む対象のオブジェクトを表す変数：grababbleObject
    GameObject grababbleObject;

    //コントローラとオブジェクトの相対位置を固定するためのFixed Joint：joint
    FixedJoint joint;

    //最初に呼び出す関数
    void Start()
    {
        //jointを初期化
        joint = gameObject.GetComponent<FixedJoint>();
    }

    //フレームごとに呼び出す関数
    void Update()
    {
        //コントローラの状態を調べ、ボタンが押されていたらオブジェクトを掴む
        if (grabAction.GetState(hand))
        {
            //ログに表示
            Debug.Log("Grab!");

            //オブジェクトを掴む
            grab();
        }
        else
        {
            //オブジェクトを放す
            release();
        }
    }

    //オブジェクトを掴む関数
    void grab()
    {
        //掴む対象が存在するかチェック
        if(grababbleObject == null || joint.connectedBody != null)
        {
            //存在しなければ何もしない
            return;
        }

        //jointにオブジェクトを接続
        joint.connectedBody = grababbleObject.GetComponent<Rigidbody>();
    }

    //オブジェクトを放す関数
    void release()
    {
        //放す対象が存在するかチェック
        if(joint.connectedBody == null)
        {
            //存在しなければ何もしない
            return;
        }

        //オブジェクトを変数rigidbodyに格納
        Rigidbody rigidbody = joint.connectedBody;

        //jointとの接続を切る
        joint.connectedBody = null;

        //コントローラの速度を放したオブジェクトに付与
        rigidbody.velocity = poseAction.GetVelocity();

        //コントローラの角速度を放したオブジェクトに付与
        rigidbody.angularVelocity = poseAction.GetAngularVelocity();
    }

    //コントローラがボタンを押したときに動作する関数
    void OnTriggerEnter(Collider other)
    {
        //対象オブジェクトを設定
        grababbleObject = other.gameObject;
    }

    //コントローラがボタンを離したときに動作する関数
    void OnTriggerExit(Collider other)
    {
        //対象オブジェクトをnullに
        grababbleObject = null;
    }
}
