using UnityEngine;
using UnityEngine.UI;

public class AIManager : MonoBehaviour
{
    float amountHappiness = 1;
    float Happiness
    {
        get => amountHappiness;
        set
        {
            amountHappiness = value;
            happinessImage.fillAmount = amountHappiness;
        }
    }

    [SerializeField]
    Image happinessImage;

    int startPendingPlayers;
    int pendingPlayers;
    int playerStates;
    bool startRound = true;

    void Start()
    {
        //Binding Callbacks
        GameManager.ReducePendingPlayersCallback += ReducePendingPlayer;
        GameManager.ResetPendingPlayersCallback += ResetPendingPlayers;
        GameManager.AIStateUpdateCallback += OnStateUpdate;
        GameManager.NewRoundCallback += () =>
        {
            ResetPendingPlayers(startPendingPlayers);
        };
        GameManager.AddHappinessCallback += (float value) =>
        {
            Happiness = Mathf.Clamp(Happiness + value, .0f, 1.0f);
            if (Happiness <= 0)
            {
                GameManager.GameOverCallback?.Invoke();
            }
        };
        GameManager.StartGameCallback += () =>
        {
            Happiness = 1;
        };
    }
    private void ReducePendingPlayer()
    {
        pendingPlayers--;

        if (pendingPlayers == 0)
        {
            pendingPlayers = startPendingPlayers;
            playerStates = startPendingPlayers;

            if (startRound)
            {
                GameManager.DelearReceiverCallback?.Invoke();
                startRound = false;
            }
            else
            {
                GameManager.ChangeTurnCallback?.Invoke(false);
            }
        }
    }
    private void ResetPendingPlayers(int InPendingPlayers)
    {
        startPendingPlayers = InPendingPlayers;
        pendingPlayers = InPendingPlayers;
        playerStates = InPendingPlayers;
        startRound = true;
    }
    private void OnStateUpdate(bool reducePending)
    {
        playerStates--;
        pendingPlayers -= reducePending ? 1 : 0;

        if (playerStates <= 0)
        {
            GameManager.ChangeTurnCallback?.Invoke(true);
            if (pendingPlayers <= 0)
            {
                GameManager.DelearReceiverCallback?.Invoke();
                GameManager.EnableChooseWinnerCallback?.Invoke();
            }
        }
    }
}
