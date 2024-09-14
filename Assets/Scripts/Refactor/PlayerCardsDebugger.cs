using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInfo
{
    public string name;
    public int cardsSum;
    public string state;

    public PlayerInfo(string inName, int inCardsSum,string inState)
    {
        this.name = inName;
        this.cardsSum = inCardsSum;
        this.state = inState;
    }
}

public class PlayerCardsDebugger : MonoBehaviour
{
    public static PlayerCardsDebugger Instance;
    private Dictionary<string, int> players = new Dictionary<string, int>(); //string = player name, int = cards sum value
    [SerializeField] private GameObject DebuggerUI;

    private void Awake()
    {
        //Fake Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        //Binding GameManager callbacks
        GameManager.StartGameCallback += () =>
        {
            players.Clear();
        };

        GameManager.NewRoundCallback += () =>
        {
            players.Clear();
        };

        GameManager.StartGameCallback += () =>
        {
            DebuggerUI.SetActive(false);
        };

        GameManager.NewRoundCallback += () =>
        {
            DebuggerUI.SetActive(false);
        };

        DebuggerUI.SetActive(false);
    }

    public void DebuggerUpdatePlayerStat(string inName, int inCardsSum)
    {
        players[inName] = inCardsSum;
    }


    private Coroutine coroutineRef;
    public void DebuggerShowPopUp(string inName,Vector3 playerPos,string inState)
    {
        if (coroutineRef is not null)
        {
            StopCoroutine(coroutineRef);
        }
        coroutineRef = StartCoroutine(HideUI(5f));
        DebuggerUI.SetActive(false);
        transform.position = playerPos + Vector3.up * .5f;
        DebuggerUI.SetActive(true);
        DebuggerUI.GetComponent<PlayerDebuggerInfo>().UpdateUI(DebuggerGetPlayerInfo(inName, inState));
    }
    private PlayerInfo DebuggerGetPlayerInfo(string inName, string inState)
    {
        return new PlayerInfo(inName, players[inName],inState);
    }

    public void OnShowStat()
    {
        if (coroutineRef is not null)
        {
            StopCoroutine(coroutineRef);
        }

        coroutineRef = StartCoroutine(HideUI());
    }
    IEnumerator HideUI(float time = 2f)
    {
        yield return new WaitForSeconds(time);
        DebuggerUI.SetActive(false);
    }
}