using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BlackOutHandler : MonoBehaviour
{
    [SerializeField] Blit rf;
    [SerializeField] Material blackMat;
    [HideInInspector] public Material material;

    private static BlackOutHandler mInstance;
    public static BlackOutHandler Instance { get { return mInstance; } }
    private void Awake()
    {
        if (mInstance != null && mInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            mInstance = this;
        }
        DontDestroyOnLoad(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        material = new Material(blackMat);
        rf.settings.blitMaterial = material;
        rf.Create();
    }

    public void SetAlpha(float alpha)
    {
        material.SetFloat("_Alpha", alpha);
    }

    public float GetAlpha()
    {
        return material.GetFloat("_Alpha");
    }

    public void SetPosition(int index, Vector3 pos)
    {
        string temp = "_Pos" + index.ToString();
        material.SetVector(temp, pos);
    }

    public Vector3 GetPosition(int index)
    {
        string temp = "_Pos" + index.ToString();
        return material.GetVector(temp);
    }



    public void SetRange(int index, float range)
    {
        string temp = "_Size" + index.ToString();
        material.SetFloat(temp, range);
    }

    public float GetRange(int index)
    {
        string temp = "_Size" + index.ToString();
        return material.GetFloat(temp);
    }

    public void SetFadeOutSize(int index, float size)
    {
        string temp = "_FadeOutSize" + index.ToString();
        material.SetFloat(temp, size);
    }

    public void FirstLevelInit()
    {

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mInstance == this)
        {
            if (scene.buildIndex != 1 && scene.buildIndex != 2 && scene.buildIndex != 3)
            {
                rf.settings.blitMaterial = null;
            }
            else
            {
                if (rf.settings.blitMaterial == null)
                {
                    rf.settings.blitMaterial = material;
                }
            }
        }

    }
}
