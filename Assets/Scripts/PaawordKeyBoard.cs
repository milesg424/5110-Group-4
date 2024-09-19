
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PasswordKeyBoard : MonoBehaviour
{
    [SerializeField] GameObject number;
    [SerializeField] GridLayoutGroup inputPanelNumbersGrid;
    [SerializeField] Color NumberForegroundColor;
    [SerializeField] Color NumberBackgroundColor;

    Animator animator;
    List<PasswordNumber> showNumbers;
    List<int> enteredNumber;
    int MaxInput;

    GSettings settings;
    // Start is called before the first frame update
    void Start()
    {
        settings = GameManager.Instance.settings;
        animator = GetComponent<Animator>();
        MaxInput = settings.password.ToString().Length;
        enteredNumber = new List<int>();
        showNumbers = new List<PasswordNumber>();
        for (int i = 0; i < MaxInput; i++)
        {
            GameObject go = Instantiate(number, inputPanelNumbersGrid.transform);
            PasswordNumber num = go.GetComponent<PasswordNumber>();
            num.Init();
            num.ChangeNnumber(-1, NumberForegroundColor, NumberBackgroundColor);
            showNumbers.Add(num);
        }
        inputPanelNumbersGrid.cellSize = new Vector2(inputPanelNumbersGrid.GetComponent<RectTransform>().sizeDelta.x / MaxInput, 100);
        UpdateInput();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Cancel"))
        {
            Time.timeScale = 1;
            animator.Play("Anim_WindowShutDown");

            gameObject.SetActive(false);
        }
        ReceiveInput();
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.Play("Anim_WindowPop");
    }

    void UpdateInput()
    {
        for (int i = 0; i < MaxInput; i++)
        {
            if (i < enteredNumber.Count)
            {
                showNumbers[i].ChangeNnumber(enteredNumber[i], NumberForegroundColor, NumberBackgroundColor);
            }
            else
            {
                showNumbers[i].ChangeNnumber(-1, NumberForegroundColor, NumberBackgroundColor);
            }
        }
        //inputPanel.text = temp;
    }

    void ReceiveInput()
    {
        if (Input.anyKeyDown)
        {
            if (enteredNumber.Count < MaxInput)
            {
                string input = Input.inputString;
                int temp;
                
                if (int.TryParse(input, out temp))
                {
                    enteredNumber.Add(temp);
                }
                UpdateInput();
            }
            if (Input.GetKeyDown(KeyCode.Backspace) && enteredNumber.Count > 0)
            {
                enteredNumber.RemoveAt(enteredNumber.Count - 1);
                UpdateInput();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                CheckPassword();
            }
        }
    }

    void CheckPassword()
    {
        if (enteredNumber.Count < MaxInput)
        {
            Debug.Log("Incorrect");
        }
        else
        {
            int pass = 0;
            for (int i = 0; i < MaxInput; i++)
            {
                pass += enteredNumber[i] * (int)Mathf.Pow(10, MaxInput - i - 1);
            }
            if (pass == settings.password)
            {
                Debug.Log("Correct");
            }
            else
            {
                Debug.Log("Incorrect" + pass);
            }
        }
    }
}
