using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private void OnDestroy()
    {
        GameManager.Instance.PlaySound(GameManager.Instance.settings.gameOverClip);
    }
}
