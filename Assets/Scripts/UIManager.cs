using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject adButton;
    [SerializeField] GameObject shiftadButton;
    [SerializeField] GameObject fButton;
    [SerializeField] GameObject qeButton;
    [SerializeField] Image tipBg;

    GameObject currentPopContent;
    Coroutine popCoroutine;

    public bool bStopShow;
    float tipA = 0;

    private static UIManager mInstance;
    public static UIManager Instance { get { return mInstance; } }
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
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Pop(GameObject content, float tiemr)
    {
        if (popCoroutine != null)
        {
            StopCoroutine(popCoroutine);
        }
        popCoroutine = StartCoroutine(IShowButton(content, tiemr));
    }

    public void PopF(float timer)
    {
        Pop(fButton, timer);
    }

    public void PopAD(float timer)
    {
        Pop(adButton, timer);
    }

    public void PopQE(float timer)
    {
        Pop(qeButton, timer);
    }

    public void PopShiftAD(float timer)
    {
        Pop(shiftadButton, timer);
    }

    IEnumerator IShowButton(GameObject content, float timer)
    {
        if (currentPopContent != null)
        {
            currentPopContent.SetActive(false);
        }
        currentPopContent = content;
        content.SetActive(true);
        foreach (Transform item in content.transform)
        {
            item.GetComponent<Graphic>().color = new Color(1, 1, 1, 0);
        }

        float a = 0;
        while (a < 0.995f)
        {
            a = Mathf.Lerp(a, 1, Time.deltaTime * 3);
            foreach (Transform item in content.transform)
            {
                item.GetComponent<Graphic>().color = new Color(1, 1, 1, a);
            }
            tipBg.color = new Color(1, 1, 1, a * tipA);
            yield return new WaitForEndOfFrame();
        }


        foreach (Transform item in content.transform)
        {
            item.GetComponent<Graphic>().color = new Color(1, 1, 1, 1);
        }
        tipBg.color = new Color(1, 1, 1, tipA);

        if (timer < 0)
        {
            while (true)
            {
                if (bStopShow)
                {
                    bStopShow = false;
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (timer > 0)
            {
                timer = timer - Time.deltaTime < 0 ? 0 : timer - Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }


        a = 1;
        while (a > 0.005f)
        {
            a = Mathf.Lerp(a, 0, Time.deltaTime * 5);
            foreach (Transform item in content.transform)
            {
                item.GetComponent<Graphic>().color = new Color(1, 1, 1, a);
            }
            tipBg.color = new Color(1, 1, 1, a * tipA);
            yield return new WaitForEndOfFrame();
        }


        foreach (Transform item in content.transform)
        {
            item.GetComponent<Graphic>().color = new Color(1, 1, 1, 0);
        }
        tipBg.color = new Color(1, 1, 1, 0);
    }
}
