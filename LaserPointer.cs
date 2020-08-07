using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//VR用の名前空間
using Valve.VR;

public class LaserPointer : MonoBehaviour
{
    //コントローラの種類（左手か右手か）を識別するための変数：handType
    public SteamVR_Input_Sources handType;

    //コントローラの状態を示すPose型の変数：controllerPose
    public SteamVR_Behaviour_Pose controllerPose;

    //テレポート用のアクションが実行されているかどうかを示す変数：teleportAction
    public SteamVR_Action_Boolean teleportAction;

    //レーザーのプレハブを参照する変数：laserPrefab
    public GameObject laserPrefab;

    //レーザーを参照する変数：laser
    private GameObject laser;

    //レーザーの位置を参照する変数：laserTransform;
    private Transform laserTransform;

    //レーザの照射地点を示す変数：hitPoint
    private Vector3 hitPoint;

    //カメラの位置を参照する変数：cameraRigTransform
    public Transform cameraRigTransform;

    //マーカーのプレハブを参照する変数：teleportReticlePrefab
    public GameObject teleportReticlePrefab;

    //マーカーを参照する変数：reticle
    private GameObject reticle;

    //マーカーの位置を参照する変数：teleportReticleTransform
    private Transform teleportReticleTransform;

    //ユーザの位置を参照する変数：headTransform
    public Transform headTransform;

    //オフセットを格納する変数：teleportReticleOffset
    public Vector3 teleportReticleOffset;

    //テレポート用のレイヤーマスク：teleportMask
    public LayerMask teleportMask;

    //テレポートの是非を問う変数 : shouldTeleport
    private bool shouldTeleport;

    //開始時に呼び出される関数
    void Start()
    {
        //レーザーをプレハブで初期化
        laser = Instantiate(laserPrefab);

        //レーザーの位置を設定
        laserTransform = laser.transform;

        //マーカーをプレハブで初期化
        reticle = Instantiate(teleportReticlePrefab);

        //マーカーの位置を設定
        teleportReticleTransform = reticle.transform;
    }

    //フレームごとに呼び出される関数
    void Update()
    {
        //テレポート用のボタンが押されていれば
        if (teleportAction.GetState(handType))
        {
            //RaycastHit型の変数：hit
            RaycastHit hit;

            //Raycastは指定した場所から光線を打ち、光線に当たったオブジェクトの情報を取得する
            //Physics.Raycast(Vector3 origin(rayの開始地点), Vector3 direction(rayの向き), RaycastHit hitInfo(当たったオブジェクトの情報を格納), 
            //  float distance(rayの発射距離 : デフォは無限), int layerMask(レイヤマスクの設定：デフォは初期設定のレイヤ));
            if(Physics.Raycast(controllerPose.transform.position, transform.forward, out hit, 100, teleportMask))
            {
                //当たったオブジェクトの位置をセット
                hitPoint = hit.point;

                //レーザーを設定
                ShowLaser(hit);

                //マーカーをアクティブに変更
                reticle.SetActive(true);

                //マーカーの位置を変更
                teleportReticleTransform.position = hitPoint + teleportReticleOffset;

                //テレポートを実行するように変更
                shouldTeleport = true;
            }
        }
        //テレポート用のボタンが押されていなければ
        else
        {
            //レーザーのアクティブを切る
            laser.SetActive(false);

            //マーカーのアクティブを切る
            reticle.SetActive(false);
        }

        //テレポート用のボタンが押されていれば
        if(teleportAction.GetStateUp(handType) && shouldTeleport)
        {
            //テレポートする
            Teleport();
        }
    }

    //レーザーを照射地点とコントローラとの中間に配置し、大きさを変更して適切なサイズに変更する関数
    private void ShowLaser(RaycastHit hit)
    {
        //レーザーをアクティブに変更
        laser.SetActive(true);

        //レーザーの位置をコントローラの状態から設定
        //Lerpは2点間を線形補間する（.5fは２点のちょうど中間の位置）
        laserTransform.position = Vector3.Lerp(controllerPose.transform.position, hitPoint, .5f);

        //レーザーの照射先に注目
        laserTransform.LookAt(hitPoint);

        //大きさを変更
        laserTransform.localScale = new Vector3(laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
    }

    //テレポートを実行する関数
    private void Teleport()
    {
        //テレポートを実行しないようにする
        shouldTeleport = false;

        //マーカーのアクティブを切る
        reticle.SetActive(false);

        //カメラの位置とユーザの位置の差を取得
        Vector3 difference = cameraRigTransform.position - headTransform.position;

        //y軸方向へは変化しないよう設定
        difference.y = 0;

        //カメラの位置を変更
        cameraRigTransform.position = hitPoint + difference;
    }
}
