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
    }

    private void Update()
    {
        if (!isRotating)
        {
            if (Input.GetButtonDown("RotateClockwise"))
            {
                isRotating = true;
                PlayerController.Instance.currentFacingDirection = PlayerController.Instance.currentFacingDirection == 4 ? 1 : PlayerController.Instance.currentFacingDirection + 1;
                StartCoroutine(IRotate(1));
            }
            else if (Input.GetButtonDown("RotateCounterClockwise"))
            {
                isRotating = true;
                PlayerController.Instance.currentFacingDirection = PlayerController.Instance.currentFacingDirection == 1 ? 4 : PlayerController.Instance.currentFacingDirection - 1;
                StartCoroutine(IRotate(-1));
            }
        }
    }

    IEnumerator IRotate(int direction)
    {
        isRotating = true;
        Vector3 rot = transform.rotation.eulerAngles;
        float targetX = 0;
        float targetZ = 0;

        switch (direction)
        {
            case 1:
                rot = new Vector3(rot.x, rot.y + 90, rot.z);
                break;
            case -1:
                rot = new Vector3(rot.x, rot.y - 90, rot.z);
                break;
        }

        Quaternion target = Quaternion.Euler(rot);

        while (Mathf.Abs(transform.rotation.eulerAngles.y - target.eulerAngles.y) > 0.1f)
        {
            targetX = facingLogic[PlayerController.Instance.currentFacingDirection - 1].x * HorizontalOffset;
            targetZ = facingLogic[PlayerController.Instance.currentFacingDirection - 1].y * HorizontalOffset;

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
}
