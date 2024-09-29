using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour
{
    public int type;
    public GameObject bodyPart;
    public GameObject body;
    public GameObject removeBody;
    public GameObject mainBody;
    // Start is called before the first frame update
    void PickUpItem()
    {


        if(type != 6) { 
        bodyPart.SetActive(true);
        }
        switch (type)
        {
      
        case 4:
                removeBody.SetActive(false);
                body.SetActive(true);
                GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ChangeCamera>().getLeg1 = true;
            break;
        case 3:
                removeBody.SetActive(false);
                body.SetActive(true);
                GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ChangeCamera>().getLeg2 = true;
                break;
        case 2:
                GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ChangeCamera>().getArm1 = true;
                break;
        case 1:
                GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ChangeCamera>().getArm2 = true;
                break;
            case 6:
                if (GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ChangeCamera>().getLeg1 &&
                    GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ChangeCamera>().getLeg2 &&
                    GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ChangeCamera>().getArm1 &&
                    GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<ChangeCamera>().getArm2)
                StartCoroutine(GetHead());              
                GameObject.FindGameObjectsWithTag("Breakable")[0].GetComponent<Collider>().enabled = true;
                break;
            default:
                break;
        }
    
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            PickUpItem();
            if(type != 6)
            {
                gameObject.SetActive(false);
            }
            
        }
    }

    IEnumerator GetHead()
    {
        GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Rigidbody>().isKinematic = true;
        GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>().enabled = false;
        yield return new WaitForSeconds(1.0f);
     
      
        
        GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Animator>().
        SetBool("CanPickUp?", true);
        yield return new WaitForSeconds(2.0f);
        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
        body.SetActive(true);
        bodyPart.SetActive(true);
        GameObject.FindGameObjectsWithTag("Breakable")[0].
            GetComponent<Collider>().enabled = true;
        yield return new WaitForSeconds(6.0f);
        GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Animator>().
      SetBool("CanPickUp?", false);
        GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Rigidbody>().isKinematic = false;
        GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>().enabled = true;
    }
}
