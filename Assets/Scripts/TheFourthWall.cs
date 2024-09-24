using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheFourthWall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController.Instance.canMove = false;
            PlayerController.Instance.SetConstantForce(new Vector3(0, 0, -1));
            FindObjectOfType<TheForthWallLight>().NextLevel();
        }
    }
}
