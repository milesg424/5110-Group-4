using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TheForthWallLight : MonoBehaviour
{
    [SerializeField] Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = transform.Find("Image").GetComponent<Image>();
        Material temp = new Material(image.material);
        image.material = temp;
    }

    public void Active()
    {
        image.gameObject.SetActive(true);
        StartCoroutine(IShow());
    }

    public void NextLevel()
    {
        StartCoroutine(IActive());
    }

    IEnumerator IActive()
    {
        float power = image.material.GetFloat("_Power");
        StartCoroutine(INext());
        while (power > 0)
        {
            power = Mathf.Lerp(power, 0, Time.deltaTime * 1.5f);
            image.material.SetFloat("_Power", power);
            yield return new WaitForEndOfFrame();
        }
        image.material.SetFloat("_Power", 0);

    }

    IEnumerator IShow()
    {
        float alpha = 0;
        Color col = image.material.color;
        while (alpha < 0.95f)
        {
            alpha = Mathf.Lerp(alpha, 1, Time.deltaTime * 2);
            image.material.SetColor("_Color", new Color(col.r, col.g, col.b, alpha));
            yield return new WaitForEndOfFrame();
        }
        image.material.SetColor("_Color", new Color(col.r, col.g, col.b, 1));

    }

    IEnumerator INext()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Level5");
    }
}
