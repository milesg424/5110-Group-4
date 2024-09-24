using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastWall : MonoBehaviour
{
    private void OnDestroy()
    {
        if (FindObjectOfType<TheForthWallLight>() != null)
        {
            FindObjectOfType<TheForthWallLight>().Active();

        }
    }
}
