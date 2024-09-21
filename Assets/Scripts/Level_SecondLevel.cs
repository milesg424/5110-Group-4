using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore.Text;

public class Level_SecondLevel : MonoBehaviour
{
    [SerializeField] GameObject fakeShadow;
    [SerializeField] GameObject helpPuzzle;

    bool isFirstPuzzleSolved;
    bool isTriggeredFirstPuzzle;

    CameraController cc;
    LightSource lightSource;
    GSettings settings;
    SpriteRenderer shadowSprite;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameManager.Instance.settings;
        PlayerController.Instance.SetPlayerCanMove(3);

        BlackOutHandler.Instance.SetAlpha(0.995f);
        BlackOutHandler.Instance.SetPosition(1, Vector3.zero);
        BlackOutHandler.Instance.SetPosition(2, Vector3.zero);
        BlackOutHandler.Instance.SetRange(1, settings.playerRangeAfterLightUp);
        BlackOutHandler.Instance.SetRange(2, settings.lightSourceRangeAfterLightUp);

        cc = FindObjectOfType<CameraController>();
        cc.canRotate = false;
        cc.canSwith3D = false;

        lightSource = FindObjectOfType<LightSource>();
        lightSource.InteractNoWaitTime();

        shadowSprite = Instantiate(fakeShadow, PlayerController.Instance.transform).GetComponent<SpriteRenderer>();

        GameObject go = Instantiate(helpPuzzle, lightSource.relayPoints[0].transform.position, Quaternion.identity);
        go.GetComponent<Puzzle_Help>().OnComplete += () => { StartCoroutine(ICompleteHelpPuzzle()); };
        go.GetComponent<Puzzle_Help>().OnComplete += () => { StartCoroutine(IIncreaseLighting()); };
        go.GetComponent<Puzzle_Help>().OnComplete += () => { PlayerController.Instance.SetPlayerCanMove(3); };
        go.GetComponent<Puzzle_Help>().OnComplete += () => { lightSource.GoToNextRelayPoint(); };
        go.GetComponent<Puzzle_Help>().OnOneCharacterComplete += () => { PlayerController.Instance.SetPlayerCanMove(0.5f); };
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vigCenter1 = Vector3.Lerp(BlackOutHandler.Instance.GetPosition(1), Camera.main.WorldToViewportPoint(PlayerController.Instance.transform.position), Time.deltaTime * 5);
        Vector3 vigCenter2 = Vector3.Lerp(BlackOutHandler.Instance.GetPosition(2), Camera.main.WorldToViewportPoint(lightSource.transform.position), Time.deltaTime * 100);
        vigCenter1 = new Vector3(vigCenter1.x, vigCenter1.y, 0);
        vigCenter2 = new Vector3(vigCenter2.x, vigCenter2.y, 0);
        BlackOutHandler.Instance.SetPosition(1, vigCenter1);
        BlackOutHandler.Instance.SetPosition(2, vigCenter2);

        if (!isFirstPuzzleSolved)
        {
            float distance = Mathf.Abs(lightSource.relayPoints[0].position.x - PlayerController.Instance.transform.position.x);
            shadowSprite.color = new Color(0, 0, 0, Mathf.Clamp((15 - distance) / 7 * 0.5f, 0, 0.5f));
            if (Mathf.Abs(PlayerController.Instance.transform.position.x - helpPuzzle.transform.position.x) < 15 && !isTriggeredFirstPuzzle)
            {
                StartCoroutine(ITriggeredFirstPuzzle());
            }
        }
    }

    #region FirstPuzzle------------------
    IEnumerator ITriggeredFirstPuzzle()
    {
        isTriggeredFirstPuzzle = true;
        PlayerController.Instance.canMove = false;
        cc.Follow(lightSource.transform);
        yield return new WaitForSeconds(3);
        PlayerController.Instance.canMove = true;

    }

    IEnumerator ICompleteHelpPuzzle()
    {
        isFirstPuzzleSolved = true;
        cc.Follow(PlayerController.Instance.transform);
        float alpha = shadowSprite.color.a;
        while (alpha > 0.01f)
        {
            alpha = Mathf.Lerp(alpha, 0, Time.deltaTime * 2);
            shadowSprite.color = new Color(0, 0, 0, alpha);
            yield return new WaitForEndOfFrame();
        }
        shadowSprite.color = new Color(0, 0, 0, 0);
    }
    #endregion----------------------

    IEnumerator IIncreaseLighting()
    {
        float alpha = BlackOutHandler.Instance.GetAlpha();
        while (alpha > 0.95f)
        {
            alpha = Mathf.Lerp(alpha, 0.95f, Time.deltaTime * 2);
            BlackOutHandler.Instance.SetAlpha(alpha);
            yield return new WaitForEndOfFrame();
        }
    }

}
