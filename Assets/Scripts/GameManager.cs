using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GSettings _settings;
    [HideInInspector] public GSettings settings;

    private static GameManager mInstance;
    public static GameManager Instance { get { if (mInstance == null) Debug.Log("场景中没有GameManager, 从prefab文件夹中托一个到场景里"); return mInstance; } }

    private void Awake()
    {
        if (mInstance != null && mInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            mInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        settings = Instantiate(_settings);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
