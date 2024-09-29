using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
     public PlayerController PlayerController;
     bool isThirdCamera;
     public bool canPickUp;
     public bool getLeg1;
     public bool getLeg2;
     public bool getArm1;
     public bool getArm2;
     public bool getHead;
    // Start is called before the first frame update
    void Start()
    {

               
       
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!isThirdCamera)
        {
            gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up*200,ForceMode.Impulse);
            isThirdCamera = true;
            PlayerController.OnSwitchThirdPerson?.Invoke();
            PlayerController.isThirdPerson = !PlayerController.isThirdPerson;
        }
    }
}
