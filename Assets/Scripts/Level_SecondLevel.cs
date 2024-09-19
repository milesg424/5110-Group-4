using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Level_SecondLevel : MonoBehaviour
{
    CameraController cc;
    BlackOutHandler blackoutHandler;
    LightSource lightSource;
    GSettings settings;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameManager.Instance.settings;
        StartCoroutine(ISetPlayerCanMoveAtBeginning(3));

        blackoutHandler = FindObjectOfType<BlackOutHandler>();
        blackoutHandler.SetAlpha(0.995f);
        blackoutHandler.SetPosition(1, Vector3.zero);
        blackoutHandler.SetPosition(2, Vector3.zero);
        blackoutHandler.SetRange(1, settings.playerRangeAfterLightUp);
        blackoutHandler.SetRange(2, settings.lightSourceRangeAfterLightUp);

        cc = FindObjectOfType<CameraController>();
        cc.canRotate = false;
        cc.canSwith3D = false;

        lightSource = FindObjectOfType<LightSource>();
        lightSource.InteractNoWaitTime();
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

    IEnumerator ISetPlayerCanMoveAtBeginning(float timer)
    {
        PlayerController.Instance.canMove = false;
        yield return new WaitForSeconds(timer);
        PlayerController.Instance.canMove = true;
    }
}
