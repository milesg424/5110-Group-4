using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level4 : MonoBehaviour
{
    [SerializeField] Transform trigger1;
    [SerializeField] Transform trigger2;

    bool isTriggered1;
    bool isTriggered2;

    LightSource lightSource;
    CameraController cc;
    GSettings settings;

    // Start is called before the first frame update
    void Start()
    {
        settings = GameManager.Instance.settings;

        lightSource = FindObjectOfType<LightSource>();
        lightSource.InteractNoWaitTime();
        lightSource.moveDirection = 1;
        lightSource.doHide = false;

        cc = FindObjectOfType<CameraController>();
        cc.SetSideView();
        cc.canRotate = false;

        BlackOutHandler.Instance.SetAlpha(0.99f);
        BlackOutHandler.Instance.SetFadeOutSize(2, 1.5f);
        BlackOutHandler.Instance.SetRange(1, settings.playerRangeAfterLightUp);
        BlackOutHandler.Instance.SetRange(2, settings.lightSourceRangeAfterLightUp);

        StartCoroutine(IStart());
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vigCenter1 = Vector3.Lerp(BlackOutHandler.Instance.GetPosition(1), Camera.main.WorldToViewportPoint(PlayerController.Instance.transform.position), Time.deltaTime * 5);
        Vector3 vigCenter2 = Vector3.Lerp(BlackOutHandler.Instance.GetPosition(2), Camera.main.WorldToViewportPoint(lightSource.transform.position), Time.deltaTime * 100);
        vigCenter1 = new Vector3(vigCenter1.x, vigCenter1.y, 0);
        vigCenter2 = new Vector3(vigCenter2.x, vigCenter2.y, 0);
        BlackOutHandler.Instance.SetPosition(1, vigCenter1);
        BlackOutHandler.Instance.SetPosition(2, vigCenter2);
        BlackOutHandler.Instance.SetRange(1, settings.playerRangeAfterLightUp * (settings.cameraSize / cc.vCam.m_Lens.OrthographicSize));
        BlackOutHandler.Instance.SetRange(2, settings.lightSourceRangeAfterLightUp);

        CheckTrigger();
    }

    void CheckTrigger()
    {
        if (!isTriggered1)
        {
            if (Mathf.Abs(PlayerController.Instance.transform.position.z - trigger1.transform.position.z) < 0.5f)
            {
                isTriggered1 = true;
                UIManager.Instance.PopShiftAD(1.5f);
            }
        }
        if (!isTriggered2)
        {
            if (Mathf.Abs(PlayerController.Instance.transform.position.z - trigger2.transform.position.z) < 0.5f)
            {
                isTriggered2 = true;
                StartCoroutine(ITrigger2());
            }
        }
    }

    IEnumerator IStart()
    {
        PlayerController.Instance.SetPlayerCanMove(3);
        cc.Follow(lightSource.transform);
        yield return new WaitForSeconds(3);
        cc.Follow(PlayerController.Instance.transform);
    }

    IEnumerator ITrigger2()
    {
        PlayerController.Instance.SetPlayerCanMove(3);
        lightSource.GoToNextRelayPoint();
        lightSource.isUseMaxDistance = false;
        cc.Follow(lightSource.transform);
        yield return new WaitForSeconds(3);
        cc.Follow(PlayerController.Instance.transform);

    }
}
