using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveCube : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(IDestroy());
    }

    IEnumerator IDestroy()
    {
        yield return new WaitForSeconds(1);
        Destroy(gameObject, 1);
        while (true)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 5);
            yield return new WaitForEndOfFrame();
        }
    }
}
