using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRController : MonoBehaviour
{
    public float m_Gravity = 30.0f;
    public float m_Sensitivity = 0.1f;
    public float m_MaxSpeed = 1.0f;
    public float m_RotationIncrement = 90;

    public SteamVR_Action_Boolean m_RotatePress = null;
    public SteamVR_Action_Boolean m_MovePress = null;
    public SteamVR_Action_Vector2 m_MoveValue = null;

    private float m_Speed = 0.0f;

    private CharacterController m_CharactorController = null;
    private Transform m_CameraRig = null;
    private Transform m_Head = null;

    private void Awake()
    {
        m_CharactorController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        m_CameraRig = SteamVR_Render.Top().origin;
        m_Head = SteamVR_Render.Top().head;
    }

    private void Update()
    {
        HandleHeight();
        CalculateMovement();
        SnapRotation();
    }

    private void HandleHeight()
    {
        float headHeight = Mathf.Clamp(m_Head.localPosition.y, 1, 2);
        m_CharactorController.height = headHeight;

        Vector3 newCenter = Vector3.zero;
        newCenter.y = m_CharactorController.height / 2;
        newCenter.y += m_CharactorController.skinWidth;

        newCenter.x = m_Head.localPosition.x;
        newCenter.z = m_Head.localPosition.z;

        newCenter = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * newCenter;

        m_CharactorController.center = newCenter;
    }

    private void CalculateMovement()
    {
        Vector3 orientationEuler = new Vector3(0, m_Head.eulerAngles.y, 0);
        Quaternion orientation = Quaternion.Euler(orientationEuler);
        Vector3 movement = Vector3.zero;

        if (m_MovePress.GetStateUp(SteamVR_Input_Sources.Any))
        {
            m_Speed = 0;
        }

        if (m_MovePress.state)
        {
            m_Speed += m_MoveValue.axis.y * m_Sensitivity;
            m_Speed = Mathf.Clamp(m_Speed, -m_MaxSpeed, m_MaxSpeed);

            movement += orientation * (m_Speed * Vector3.forward);
        }

        movement.y -= m_Gravity * Time.deltaTime;

        m_CharactorController.Move(movement * Time.deltaTime);
    }

    private void SnapRotation()
    {
        float snapValue = 0.0f;

        if (m_RotatePress.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            snapValue = -Mathf.Abs(m_RotationIncrement);
        }

        if (m_RotatePress.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            snapValue = Mathf.Abs(m_RotationIncrement);
        }

        transform.RotateAround(m_Head.position, Vector3.up, snapValue);
    }

}
