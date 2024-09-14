using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public GameObject Head;
    public GameObject RemovedHead;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetUpHead());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator SetUpHead()
    {
        
          
        yield return new WaitForSeconds(6f);
        Head.SetActive(true);
        RemovedHead.SetActive(false);
    }

  
}
