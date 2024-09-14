using TMPro;
using UnityEngine;

public class PlayerDebuggerInfo : MonoBehaviour
{
    [SerializeField]
    private TMP_Text nameUI;
    [SerializeField]
    private TMP_Text valueUI;
    [SerializeField]
    private TMP_Text stateUI;
    [SerializeField]
    private GameObject statContainer;
    [SerializeField]
    private GameObject optionContainer;

    public void UpdateUI(PlayerInfo inPlayerInfo)
    {
        nameUI.text = "Name " + inPlayerInfo.name;
        valueUI.text = "Value " + inPlayerInfo.cardsSum;
        stateUI.text = "State " + inPlayerInfo.state;
    }
    private void OnEnable()
    {
        statContainer.SetActive(false);
        optionContainer.SetActive(true);
    }
}
