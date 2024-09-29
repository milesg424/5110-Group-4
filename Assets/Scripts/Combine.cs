using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combine : MonoBehaviour
{
    public bool canCombine = false;

    void Start()
    {
        StartCoroutine(SetCombine());

    }

    IEnumerator SetCombine()
    {
        yield return new WaitForSeconds(1.0f);
        canCombine = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && canCombine)
        {
            collision.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            collision.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            collision.gameObject.transform.GetChild(4).gameObject.SetActive(true);
            collision.gameObject.GetComponent<ChangeCamera>().canPickUp = true;
            collision.gameObject.GetComponent<ChangeCamera>().getHead = true;
            gameObject.SetActive(false);
        }
    }
}
