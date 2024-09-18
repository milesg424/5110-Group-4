using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Level_FirstLevel : MonoBehaviour
{
    [SerializeField] float playerFreezTimeWhenTouchLight;
    [SerializeField] float blackOutRangeBeforeLightUp; 
    [SerializeField] float playerBlackOutRangeBeforeLightUp; 
    [SerializeField] float blackOutRangeAfterLightUp;
    [SerializeField] Material blackMat;
    [SerializeField] Blit rf;
    LightSource lightSource;
    Material tempMat;
    //-------------
    //UniversalRenderPipelineAsset urpAsset;
    //ScriptableRendererData[] rendererData;
    //ScriptableRendererFeature _feature;
    //Material blackOutMaterial;

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
        Camera.main.backgroundColor = Color.black;

        //urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        //rendererData = urpAsset?.GetType()
        //        .GetField("m_RendererDataList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?
        //        .GetValue(urpAsset) as ScriptableRendererData[];
        //foreach (ScriptableRendererFeature feature in rendererData[0].rendererFeatures)
        //{
        //    if (string.Compare(feature.name, "BlackOutRenderer") == 0)
        //    {
        //        _feature = feature;
        //        //_feature.SetActive(true);
        //    }
        //}
        //FieldInfo[] fields = _feature.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
        //FullScreenPassRendererFeature fr = _feature as FullScreenPassRendererFeature;
        //blackOutMaterial = fr.passMaterial;

        tempMat = new Material(blackMat);
        rf.settings.blitMaterial = tempMat;
        tempMat.SetFloat("_Alpha", 0.995f);
        tempMat.SetVector("_Pos1", Vector3.zero);
        tempMat.SetVector("_Pos2", Vector3.zero);
        tempMat.SetFloat("_Size1", playerBlackOutRangeBeforeLightUp);
        tempMat.SetFloat("_Size2", blackOutRangeBeforeLightUp);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vigCenter1 = Vector3.Lerp(tempMat.GetVector("_Pos1"), Camera.main.WorldToViewportPoint(PlayerController.Instance.transform.position), Time.deltaTime * 5);
        Vector3 vigCenter2 = Vector3.Lerp(tempMat.GetVector("_Pos2"), Camera.main.WorldToViewportPoint(lightSource.transform.position), Time.deltaTime * 100);
        vigCenter1 = new Vector3(vigCenter1.x, vigCenter1.y, 0);
        vigCenter2 = new Vector3(vigCenter2.x, vigCenter2.y, 0);
        tempMat.SetVector("_Pos1", vigCenter1);
        tempMat.SetVector("_Pos2", vigCenter2);
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
        float temp2 = playerBlackOutRangeBeforeLightUp;
        while (temp < blackOutRangeAfterLightUp - 0.02f)
        {
            temp = Mathf.Lerp(temp, blackOutRangeAfterLightUp, Time.deltaTime * 5);
            temp2 = Mathf.Lerp(temp2, blackOutRangeAfterLightUp, Time.deltaTime * 5);
            tempMat.SetFloat("_Size2", temp);
            yield return new WaitForEndOfFrame();
        }
    }

    //private void OnDestroy()
    //{
    //    blackMat.SetFloat("_Alpha", 0);
    //    blackMat.SetVector("_Pos1", Vector3.zero);
    //    blackMat.SetVector("_Pos2", Vector3.zero);
    //    blackMat.SetFloat("_Size1", 0);
    //    blackMat.SetFloat("_Size2", 0);
    //}
}
