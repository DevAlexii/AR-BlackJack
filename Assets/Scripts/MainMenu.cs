using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    TMP_InputField inputField;

    private void Start()
    {
        //Bind inputfield delegate when finish to insert number
        inputField.onEndEdit.AddListener(ChangeAINum);
    }
    public void StartGame()
    {
        //Call start game
        GameManager.StartGameCallback?.Invoke();
    }
    public void Quit()
    {
        //Close application
        Application.Quit();
    }
    public void ChangeAINum(string txt)
    {
        //Try and check if text in the inputfield is a valid number
        if (int.TryParse(txt, out int num))
        {
            //Number has to be me minimal 1 to start game and max 4
            num = Mathf.Clamp(num, 1, 4);

            //Update text if is greater or lesser 4 or 1
            inputField.text = num.ToString();

            GameManager.ChangeAINumCallback?.Invoke(num);
        }
        else
        {
            //Update text to minimal ai num to play when input is a wrong text
            inputField.text = "1";

            GameManager.ChangeAINumCallback?.Invoke(num);
        }
    }
}
