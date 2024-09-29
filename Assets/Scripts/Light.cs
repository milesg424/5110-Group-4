using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightExplode : MonoBehaviour
{
    public float rangeSpeed;
    public float intensitySpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Light>().intensity += intensitySpeed;
        GetComponent<Light>().range += rangeSpeed;
        if (GetComponent<Light>().intensity >= 50000)
        {
            SceneManager.LoadScene(6);
        }
    }
}
