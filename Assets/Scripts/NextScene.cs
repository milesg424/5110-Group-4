using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    // �ڱ༭���п����õĳ�������
    public string nextSceneName;

    // �л���ָ���ĳ�����ͨ���༭�����õĳ������ƣ�
    public void OnTriggerEnter(Collider other)
    {
        // �����ײ���Ƿ������
        if (other.CompareTag("Player"))
        {
            // ����������Ʋ�Ϊ�գ����л�����
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