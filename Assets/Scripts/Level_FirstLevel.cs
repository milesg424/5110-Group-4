using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Level_FirstLevel : MonoBehaviour
{
    [SerializeField] float playerFreezTimeWhenTouchLight;
    [SerializeField, Range(0, 1)] float blackOutRangeBeforeLightUp; 
    [SerializeField, Range(0, 1)] float blackOutRangeAfterLightUp; 
    Volume vol;
    Vignette vig;
    LightSource lightSource;
    // Start is called before the first frame update
    void Start()
    {
        CameraController cc = FindObjectOfType<CameraController>();
        lightSource = FindObjectOfType<LightSource>();
        lightSource.OnInteract += () => { StartCoroutine(IInteractWithLight()); };
        GameObject go = new GameObject("TempCameraFollowTarget");
        go.transform.position = PlayerController.Instance.transform.position + new Vector3(15, 0, 0);
        cc.SetFollowTarget(go.transform);
        cc.canRotate = false;

        vol = FindObjectOfType<Volume>();
        vol.profile.TryGet<Vignette>(out vig);
        vig.intensity.Override(blackOutRangeBeforeLightUp);
        Camera.main.backgroundColor = Color.black;

        UniversalRenderPipelineAsset urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        ScriptableRendererData[] rendererData = urpAsset?.GetType()
                .GetField("m_RendererDataList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
                .GetValue(urpAsset) as ScriptableRendererData[];
        foreach (var feature in rendererData[0].rendererFeatures)
        {
            if (string.Compare(feature.name, "BlackOutRenderer") == 0)
            {
                feature.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 vigCenter = Vector3.Lerp(vig.center.value, Camera.main.WorldToViewportPoint(PlayerController.Instance.transform.position), Time.deltaTime * 5);
        vig.center.Override(vigCenter);
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
        float temp = blackOutRangeBeforeLightUp;
        while (temp > blackOutRangeAfterLightUp + 0.02f)
        {
            temp = Mathf.Lerp(temp, blackOutRangeAfterLightUp, Time.deltaTime * 5);
            vig.intensity.Override(temp);
            yield return new WaitForEndOfFrame();
        }
    }
}
