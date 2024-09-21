using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    // 在编辑器中可设置的场景名称
    public string nextSceneName;

    // 切换到指定的场景（通过编辑器设置的场景名称）
    public void OnTriggerEnter(Collider other)
    {
        // 检查碰撞的是否是玩家
        if (other.CompareTag("Player"))
        {
            // 如果场景名称不为空，则切换场景
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogWarning("Scene name is empty.");
            }
        }
    }
}