using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastWall : MonoBehaviour
{
    private void OnDestroy()
    {
        FindObjectOfType<TheForthWallLight>().Active();
    }
}
