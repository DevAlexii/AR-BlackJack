using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Action StartGameCallback;
    public Action<int> ChangeAINumCallback;

    [SerializeField]
    TMP_InputField inputField;

    private void Awake()
    {
        CustomLibrary.SetGameManager(this);

        inputField.onEndEdit.AddListener(ChangeAINum);
    }

    public void StartGame()
    {
        StartGameCallback?.Invoke();
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void ChangeAINum(string txt)
    {
        if (int.TryParse(txt, out int num) && num > 1)
        {
            num = Mathf.Clamp(num, 1, 4);
            inputField.text = num.ToString();
            ChangeAINumCallback?.Invoke(num);
        }
        else
        {
            inputField.text = "1";
            ChangeAINumCallback?.Invoke(num);
        }
    }
}