using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Level_FirstLevel : MonoBehaviour
{
    [SerializeField] float playerFreezTimeWhenTouchLight;
    [SerializeField] GameObject nextLevel;
    [SerializeField] GameObject wall;
    LightSource lightSource;

    GSettings settings;

    // Start is called before the first frame update
    void Start()
    {
        settings = GameManager.Instance.settings;
        CameraController cc = FindObjectOfType<CameraController>();
        lightSource = FindObjectOfType<LightSource>();
        lightSource.OnInteract += () => { StartCoroutine(IInteractWithLight()); };
        lightSource.OnInteract += () => { nextLevel.SetActive(true); wall.SetActive(false); };
        GameObject go = new GameObject("TempCameraFollowTarget");
        go.transform.position = PlayerController.Instance.transform.position + new Vector3(15, 0, 0);
        cc.SetFollowTarget(go.transform);
        cc.canRotate = false;
        cc.canSwith3D = false;
        Camera.main.backgroundColor = Color.black;

        BlackOutHandler.Instance.SetAlpha(0.995f);
        BlackOutHandler.Instance.SetPosition(1, Vector3.zero);
        BlackOutHandler.Instance.SetPosition(2, Vector3.zero);
        BlackOutHandler.Instance.SetRange(1, settings.playerRangeBeforeLightUp);
        BlackOutHandler.Instance.SetRange(2, settings.lightSourceRangeBeforeLightUp);
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
    }

    IEnumerator IInteractWithLight()
    {
        PlayerController.Instance.SetPlayerCanMove(playerFreezTimeWhenTouchLight);
        StartCoroutine(ILightUp());
        yield return new WaitForSeconds(playerFreezTimeWhenTouchLight);
    }

    IEnumerator ILightUp()
    {
        float temp1 = settings.playerRangeBeforeLightUp;
        float temp2 = settings.lightSourceRangeBeforeLightUp;
        while (temp2 < settings.lightSourceRangeAfterLightUp - 0.02f)
        {
            temp1 = Mathf.Lerp(temp1, settings.playerRangeAfterLightUp, Time.deltaTime * 5);
            temp2 = Mathf.Lerp(temp2, settings.lightSourceRangeAfterLightUp, Time.deltaTime * 5);
            BlackOutHandler.Instance.SetRange(1, temp1);
            BlackOutHandler.Instance.SetRange(2, temp2);
            yield return new WaitForEndOfFrame();
        }
    }
}
