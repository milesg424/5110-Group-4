using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Level_FirstLevel : MonoBehaviour
{
    [SerializeField] float playerFreezTimeWhenTouchLight;
    LightSource lightSource;
    BlackOutHandler blackoutHandler;

    GSettings settings;

    // Start is called before the first frame update
    void Start()
    {
        settings = GameManager.Instance.settings;
        CameraController cc = FindObjectOfType<CameraController>();
        lightSource = FindObjectOfType<LightSource>();
        lightSource.OnInteract += () => { StartCoroutine(IInteractWithLight()); };
        GameObject go = new GameObject("TempCameraFollowTarget");
        go.transform.position = PlayerController.Instance.transform.position + new Vector3(15, 0, 0);
        cc.SetFollowTarget(go.transform);
        cc.canRotate = false;
        cc.canSwith3D = false;
        Camera.main.backgroundColor = Color.black;

        blackoutHandler = FindObjectOfType<BlackOutHandler>();
        blackoutHandler.SetAlpha(0.995f);
        blackoutHandler.SetPosition(1, Vector3.zero);
        blackoutHandler.SetPosition(2, Vector3.zero);
        blackoutHandler.SetRange(1, settings.playerRangeBeforeLightUp);
        blackoutHandler.SetRange(2, settings.lightSourceRangeBeforeLightUp);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vigCenter1 = Vector3.Lerp(blackoutHandler.GetPosition(1), Camera.main.WorldToViewportPoint(PlayerController.Instance.transform.position), Time.deltaTime * 5);
        Vector3 vigCenter2 = Vector3.Lerp(blackoutHandler.GetPosition(2), Camera.main.WorldToViewportPoint(lightSource.transform.position), Time.deltaTime * 100);
        vigCenter1 = new Vector3(vigCenter1.x, vigCenter1.y, 0);
        vigCenter2 = new Vector3(vigCenter2.x, vigCenter2.y, 0);
        blackoutHandler.SetPosition(1, vigCenter1);
        blackoutHandler.SetPosition(2, vigCenter2);
    }

    IEnumerator IInteractWithLight()
    {
        PlayerController.Instance.canMove = false;
        StartCoroutine(ILightUp());
        yield return new WaitForSeconds(playerFreezTimeWhenTouchLight);
        PlayerController.Instance.canMove = true;
    }

    IEnumerator ILightUp()
    {
        float temp1 = settings.playerRangeBeforeLightUp;
        float temp2 = settings.lightSourceRangeBeforeLightUp;
        while (temp2 < settings.lightSourceRangeAfterLightUp - 0.02f)
        {
            temp1 = Mathf.Lerp(temp1, settings.playerRangeAfterLightUp, Time.deltaTime * 5);
            temp2 = Mathf.Lerp(temp2, settings.lightSourceRangeAfterLightUp, Time.deltaTime * 5);
            blackoutHandler.SetRange(1, temp1);
            blackoutHandler.SetRange(2, temp2);
            yield return new WaitForEndOfFrame();
        }
    }
}
