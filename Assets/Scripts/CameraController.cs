using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public bool canRotate;
    [HideInInspector] public bool canSwith3D;
    bool isRotating;

    float desiredX;
    float xRotation;

    Vector2[] facingLogic;
    CinemachineVirtualCamera vCam;
    CinemachineTransposer transposer;
    GSettings settings;
    private void Start()
    {
        settings = GameManager.Instance.settings;
        facingLogic = new Vector2[4] { new Vector2(0, -1), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(1, 0) };

        vCam = GetComponent<CinemachineVirtualCamera>();
        vCam.Follow = PlayerController.Instance.transform;
        vCam.m_Lens.OrthographicSize = settings.cameraSize;
        vCam.m_Lens.FieldOfView = settings.cameraFOV;

        transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset = new Vector3(0, settings.YOffset, -settings.HorizontalOffset);

        PlayerController.Instance.OnSwitchThirdPerson += SwitchToThirdPerson;
        PlayerController.Instance.OnSwitchLockCamera += SwitchTo2D;

        canRotate = true;
        canSwith3D = true;
    }

    private void Update()
    {
        if (!PlayerController.Instance.isThirdPerson && canRotate)
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
        else if (PlayerController.Instance.isThirdPerson)
        {
            float num = Input.GetAxis("Mouse X") * Time.deltaTime * settings.thirdPersonCameraSensitive;
            float num2 = Input.GetAxis("Mouse Y") * Time.deltaTime * settings.thirdPersonCameraSensitive;
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

        targetX = facingLogic[PlayerController.Instance.currentFacingDirection - 1].x * settings.HorizontalOffset;
        targetZ = facingLogic[PlayerController.Instance.currentFacingDirection - 1].y * settings.HorizontalOffset;
        while (Mathf.Abs(transform.rotation.eulerAngles.y - target.eulerAngles.y) > 0.1f)
        {
            float currentX = Mathf.Lerp(transposer.m_FollowOffset.x, targetX, Time.deltaTime * settings.CameraRotateSpeed);
            float currentZ = Mathf.Lerp(transposer.m_FollowOffset.z, targetZ, Time.deltaTime * settings.CameraRotateSpeed);


            transposer.m_FollowOffset = new Vector3(currentX, settings.YOffset, currentZ);
            transform.rotation = Quaternion.Lerp(transform.rotation, target, Time.deltaTime * settings.CameraRotateSpeed);
            yield return new WaitForEndOfFrame();

        }

        transposer.m_FollowOffset = new Vector3(targetX, settings.YOffset, targetZ);
        transform.rotation = Quaternion.Euler(rot);
        //PlayerController.Instance.currentFacingDirection = PlayerController.Instance.currentFacingDirection;
        isRotating = false;
    }

    //IEnumerator ISwitchThirdPerson()
    //{

    //}

    void SwitchToThirdPerson()
    {
        vCam.m_Lens.NearClipPlane = 0.01f;
        vCam.AddCinemachineComponent<CinemachineFramingTransposer>();
        vCam.GetComponent<CinemachineCollider>().enabled = true;
        vCam.m_Lens.Orthographic = false;
        StartCoroutine(ISwitchToThirdPerson());
    }

    void SwitchTo2D()
    {
        vCam.m_Lens.NearClipPlane = -100f;
        transposer = vCam.AddCinemachineComponent<CinemachineTransposer>();
        transposer.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
        vCam.GetComponent<CinemachineCollider>().enabled = false;
        vCam.m_Lens.Orthographic = true;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        StartCoroutine(IRotate());
    }

    IEnumerator ISwitchToThirdPerson()
    {
        transform.localRotation = Quaternion.Euler(10, transform.localRotation.eulerAngles.y, 0);
        xRotation = 10;
        CinemachineFramingTransposer trans = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        trans.m_XDamping = 0;
        trans.m_YDamping = 0;
        trans.m_ZDamping = 0;
        yield return new WaitForEndOfFrame();
        trans.m_XDamping = 1;
        trans.m_YDamping = 1;
        trans.m_ZDamping = 1;
    }

    public void SetFollowTarget(Transform target)
    {
        vCam.Follow = target;
    }

    public void Follow(Transform trans)
    {
        vCam.Follow = trans;
    }
}
