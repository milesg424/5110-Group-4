using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float CameraRotateSpeed;
    [SerializeField] float YOffset;
    [SerializeField] float HorizontalOffset;

    //int PlayerController.Instance.currentFacingDirection = 1;
    bool isRotating;

    float desiredX;
    float xRotation;

    Vector2[] facingLogic;
    CinemachineVirtualCamera vCam;
    CinemachineTransposer transposer;

    private void Start()
    {
        facingLogic = new Vector2[4] { new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0) };
        vCam = GetComponent<CinemachineVirtualCamera>();
        transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        vCam.Follow = PlayerController.Instance.transform;
        transposer.m_FollowOffset = new Vector3(0, YOffset, -HorizontalOffset);

        PlayerController.Instance.OnSwitchThirdPerson += SwitchToThirdPerson;
    }

    private void Update()
    {
        if (!PlayerController.Instance.isThirdPerson)
        {
            if (!isRotating)
            {
                if (Input.GetButtonDown("RotateClockwise"))
                {
                    isRotating = true;
                    PlayerController.Instance.currentFacingDirection = 1;
                    StartCoroutine(IRotate());
                }
                else if (Input.GetButtonDown("RotateCounterClockwise"))
                {
                    isRotating = true;
                    PlayerController.Instance.currentFacingDirection = 4;
                    StartCoroutine(IRotate());
                }
            }
        }
        else
        {
            float num = Input.GetAxis("Mouse X") * Time.deltaTime * PlayerController.Instance.thirdPersonCameraSensitive;
            float num2 = Input.GetAxis("Mouse Y") * Time.deltaTime * PlayerController.Instance.thirdPersonCameraSensitive;
            desiredX = transform.localRotation.eulerAngles.y + num;
            xRotation -= num2;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        }
    }

    //private void FixedUpdate()
    //{
    //    if (PlayerController.Instance.isThirdPerson)
    //    {
    //        float num = Input.GetAxis("Mouse X") * Time.fixedDeltaTime * PlayerController.Instance.thirdPersonCameraSensitive;
    //        float num2 = Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * PlayerController.Instance.thirdPersonCameraSensitive;
    //        desiredX = transform.localRotation.eulerAngles.y + num;
    //        xRotation -= num2;
    //        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    //        transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
    //    }
    //}

    IEnumerator IRotate()
    {
        isRotating = true;
        Vector3 rot = transform.rotation.eulerAngles;
        float targetX = 0;
        float targetZ = 0;

        switch (PlayerController.Instance.currentFacingDirection)
        {
            case 1:
                rot = new Vector3(rot.x, 0, rot.z);
                break;
            case 4:
                rot = new Vector3(rot.x, -90, rot.z);
                break;
        }

        Quaternion target = Quaternion.Euler(rot);

        targetX = facingLogic[PlayerController.Instance.currentFacingDirection - 1].x * HorizontalOffset;
        targetZ = facingLogic[PlayerController.Instance.currentFacingDirection - 1].y * HorizontalOffset;
        while (Mathf.Abs(transform.rotation.eulerAngles.y - target.eulerAngles.y) > 0.1f)
        {
            float currentX = Mathf.Lerp(transposer.m_FollowOffset.x, targetX, Time.deltaTime * CameraRotateSpeed);
            float currentZ = Mathf.Lerp(transposer.m_FollowOffset.z, targetZ, Time.deltaTime * CameraRotateSpeed);


            transposer.m_FollowOffset = new Vector3(currentX, YOffset, currentZ);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * CameraRotateSpeed);
            yield return new WaitForEndOfFrame();

        }

        transposer.m_FollowOffset = new Vector3(targetX, YOffset, targetZ);
        transform.rotation = Quaternion.Euler(rot);
        PlayerController.Instance.currentFacingDirection = PlayerController.Instance.currentFacingDirection;
        isRotating = false;
    }

    //IEnumerator ISwitchThirdPerson()
    //{

    //}

    void SwitchToThirdPerson()
    {
        vCam.AddCinemachineComponent<CinemachineFramingTransposer>().m_CenterOnActivate = false;
        vCam.GetComponent<CinemachineCollider>().enabled = true;
        vCam.m_Lens.Orthographic = false;
        transform.localRotation = Quaternion.Euler(90, 0, 0);
    }
}
