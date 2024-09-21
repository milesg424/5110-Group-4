using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Glow : MonoBehaviour
{
    public int mixIntensity;
    public int maxIntensity;
    public Light light;
    public float speed;
    float time;
    bool canGlow;
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(StartGlow());
    }

    // Update is called once per frame
    void Update()
    {
        if(light.intensity < maxIntensity && canGlow) { 
        light.intensity += speed;
        }
        if(light.intensity > maxIntensity)
        {
            SceneManager.LoadScene(2);
        }
    }


    IEnumerator StartGlow()
    {
        yield return new WaitForSeconds(2f);
        GetComponent<BreakableObject>().Break();
        yield return new WaitForSeconds(0.5f);
        canGlow = true;
    }
}
