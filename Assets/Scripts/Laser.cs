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
    // Start is called before the first frame update
    void Start()
    {
        StartMove();
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
        LaserObject.SetActive(true);
        LaserObject.transform.position = targetLoc.transform.position;
        shouldStart = true;
    }
    public void StopMove()
    {
        shouldStart = false;
        LaserObject.SetActive(false);
    }
}
