using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//VR用の名前空間
using Valve.VR;

public class ControllerGrabObject : MonoBehaviour
{
    //コントローラの種類（左手か右手か）を識別するための変数：handType
    public SteamVR_Input_Sources handType;

    //コントローラの状態を示すPose型の変数：controllerPose
    public SteamVR_Behaviour_Pose controllerPose;

    //掴むアクションが実行されているかどうかを示す変数：grabAction
    public SteamVR_Action_Boolean grabAction;

    //現在接触しているオブジェクトを示す変数：collidingObject
    private GameObject collidingObject;

    //現在掴んでいるオブジェクトを示す変数：objectInHand
    private GameObject objectInHand;

    //Collider型の引数を受け取り、接触しているオブジェクトを設定する関数
    private void SetCollidingObject(Collider col)
    {
        //接触しているオブジェクトがない
        //または
        //接触しているオブジェクトがRigidbodyコンポーネントを持っていない（＝物理判定がない）
        //何もしない
        if(collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }

        //接触しているオブジェクトを設定
        collidingObject = col.gameObject;
    }

    //物体と接触したときに呼び出される関数
    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    //物体と接触しているときに呼び出される関数
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    //物体と接触し終えたときに呼び出される関数
    public void OnTriggerExit(Collider other)
    {
        //接触しているオブジェクトがなければ、何もしない
        if (!collidingObject)
        {
            return;
        }

        //接触オブジェクトを存在しない設定に変更
        collidingObject = null;
    }

    //掴む操作を行う関数
    private void GrabObject()
    {
        //掴んでいるオブジェクトとして、接触オブジェクトを設定
        objectInHand = collidingObject;

        //代わりに接触オブジェクトは存在しない設定に変更
        collidingObject = null;

        //コントローラとオブジェクトを接続する関数を呼び出す
        var joint = AddFixedJoint();

        //コントローラとオブジェクトを接続
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();
    }

    //コントローラとオブジェクトを接続する関数
    private FixedJoint AddFixedJoint()
    {
        //オブジェクトにFixedJointコンポーネントを付与
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();

        //パラメータを設定
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }

    //オブジェクトを放す関数
    private void ReleaseObject()
    {
        //FixedJointがあれば？
        if (GetComponent<FixedJoint>())
        {
            //jointの接続を切る
            GetComponent<FixedJoint>().connectedBody = null;

            //FixedJointを破壊
            Destroy(GetComponent<FixedJoint>());

            //コントローラの速度をオブジェクトに付与
            objectInHand.GetComponent<Rigidbody>().velocity = controllerPose.GetVelocity();

            //コントローラの角速度をオブジェクトに付与
            objectInHand.GetComponent<Rigidbody>().angularVelocity = controllerPose.GetAngularVelocity();
        }

        //掴んでいるオブジェクトを存在しない設定に変更
        objectInHand = null;
    }

    //フレームごとに呼び出される関数
    void Update()
    {
        //掴む操作のボタンが押されていれば
        if (grabAction.GetLastStateDown(handType))
        {
            //接触しているオブジェクトがあれば
            if (collidingObject)
            {
                //オブジェクトを掴む
                GrabObject();
            }
        }

        //掴む操作用のボタンが離されたら
        if (grabAction.GetLastStateUp(handType))
        {
            //掴んでいるオブジェクトがあれば
            if (objectInHand)
            {
                //掴んでいるオブジェクトを放す
                ReleaseObject();
            }
        }
    }
}
