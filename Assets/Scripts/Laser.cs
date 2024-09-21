using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public GameObject targetLoc;
    public GameObject startLoc;
    public GameObject LaserObject;
    bool shouldStart;
    public float speed;
    float time;
    bool cut;
    public GameObject tri;
    // Start is called before the first frame update
    void Start()
    {
        //StartMove();
        GetComponent<Renderer>().enabled = (false);
        Invoke("StartMove", 2.0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldStart)
        {
            time += speed;
            LaserObject.transform.position = Vector3.Lerp(startLoc.transform.position, targetLoc.transform.position, time);
        }
        if (time >= 1)
        {
            StopMove();
            time= 0f;
        }
    }

    public void StartMove()
    {
        GetComponent<Renderer>().enabled = (true);
        LaserObject.SetActive(true);
        LaserObject.transform.position = targetLoc.transform.position;
        shouldStart = true;
    }

    public void StopMove()
    {
        time = 0f;
        LaserObject.transform.position = startLoc.transform.position;


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !cut)
        {
            other.transform.GetChild(0).gameObject.SetActive(false);
            other.transform.GetChild(1).gameObject.SetActive(true);
            other.transform.GetChild(2).gameObject.SetActive(true);
            other.transform.GetChild(2).parent = null;
            cut = true;
            gameObject.SetActive(false);
            
        }
    }

   
}
