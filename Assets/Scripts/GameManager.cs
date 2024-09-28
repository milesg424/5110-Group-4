using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] FullScreenPassRendererFeature fr;
    [SerializeField] GSettings originalSettings;
    [HideInInspector] public GSettings settings;

    AudioSource audioSource;

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
        settings = Instantiate(originalSettings);
        SceneManager.sceneLoaded += OnSceneLoad;

        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            SceneManager.LoadScene(4);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            SceneManager.LoadScene(3);

        }
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level6")
        {
            fr.passMaterial.SetFloat("_ColorThreshold", 1.7f);
        }
        else
        {
            fr.passMaterial.SetFloat("_ColorThreshold", 0.1f);

        }
    }

    private void OnDestroy()
    {
        if (mInstance == this)
        {
            fr.passMaterial.SetFloat("_ColorThreshold", 10);
        }
    }

    public void PlaySound(AudioClip clip, float volume = 1)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}
