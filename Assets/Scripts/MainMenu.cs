using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputField;

    private void Start()
    {
        inputField.onEndEdit.AddListener(ChangeAINum);
    }
    public void StartGame()
    {
        GameManager.StartGameCallback?.Invoke();
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
            GameManager.ChangeAINumCallback?.Invoke(num);
        }
        else
        {
            inputField.text = "1";
            GameManager.ChangeAINumCallback?.Invoke(num);
        }
    }
}
