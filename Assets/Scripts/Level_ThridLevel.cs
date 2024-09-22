using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_ThridLevel : MonoBehaviour
{
    [SerializeField] Transform sceneStartTarget;

    LightSource lightSource;
    CameraController cc;

    GSettings settings;

    float camFollowPlayerSpeed = 5;//used only for the beginning because when camera move from center to follow player, the black out range will be wiggle strongly if this value is too small;
    float originalLightSpeed;
    float originalCamSize;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameManager.Instance.settings;

        lightSource = FindObjectOfType<LightSource>();
        lightSource.InteractNoWaitTime();
        lightSource.isUseMaxDistance = false;
        originalLightSpeed = settings.lightSourceMoveSpeed;
        settings.lightSourceMoveSpeed = settings.lightSourceSpeedLevelThree;

        BlackOutHandler.Instance.SetAlpha(0.995f);
        BlackOutHandler.Instance.SetFadeOutSize(2, 1.5f);
        BlackOutHandler.Instance.SetPosition(1, Vector3.zero);
        BlackOutHandler.Instance.SetPosition(2, Vector3.zero);
        BlackOutHandler.Instance.SetRange(1, settings.playerRangeAfterLightUp);
        BlackOutHandler.Instance.SetRange(2, settings.lightSourceRangeAfterLightUp + 5f);

        cc = FindObjectOfType<CameraController>();
        originalCamSize = cc.vCam.m_Lens.OrthographicSize;

        StartCoroutine(ISceneStart());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vigCenter1 = Vector3.Lerp(BlackOutHandler.Instance.GetPosition(1), Camera.main.WorldToViewportPoint(PlayerController.Instance.transform.position), Time.deltaTime * camFollowPlayerSpeed);
        Vector3 vigCenter2 = Vector3.Lerp(BlackOutHandler.Instance.GetPosition(2), Camera.main.WorldToViewportPoint(lightSource.transform.position), Time.deltaTime * 100);
        vigCenter1 = new Vector3(vigCenter1.x, vigCenter1.y, 0);
        vigCenter2 = new Vector3(vigCenter2.x, vigCenter2.y, 0);
        BlackOutHandler.Instance.SetPosition(1, vigCenter1);
        BlackOutHandler.Instance.SetPosition(2, vigCenter2);
        BlackOutHandler.Instance.SetRange(1, settings.playerRangeAfterLightUp * (settings.cameraSize / cc.vCam.m_Lens.OrthographicSize));
        BlackOutHandler.Instance.SetRange(2, settings.lightSourceRangeAfterLightUp + 5f);
    }

    IEnumerator ISceneStart()
    {
        float timer = settings.stopMovingThird;
        camFollowPlayerSpeed = 100;
        PlayerController.Instance.SetPlayerCanMove(timer + 0.1f);
        cc.canRotate = false;
        cc.Follow(sceneStartTarget);
        cc.vCam.m_Lens.OrthographicSize = 20;
        yield return new WaitForSeconds(timer);
        cc.Follow(PlayerController.Instance.transform);
        cc.canRotate = true;
        StartCoroutine(ISetCameraSizeBack());
        settings.lightSourceMoveSpeed = originalLightSpeed;

        yield return new WaitForSeconds(0.8f);
        camFollowPlayerSpeed = 5;
    }

    IEnumerator ISetCameraSizeBack()
    {
        CinemachineVirtualCamera vcam = cc.vCam;
        float size = vcam.m_Lens.OrthographicSize;
        while (Mathf.Abs(size - settings.cameraSize) > 0.1f)
        {
            size = Mathf.Lerp(size, settings.cameraSize, Time.deltaTime * 2);
            vcam.m_Lens.OrthographicSize = size;
            yield return new WaitForEndOfFrame();
        }
        vcam.m_Lens.OrthographicSize = size;
    }
}
