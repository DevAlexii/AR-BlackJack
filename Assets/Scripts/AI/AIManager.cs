using UnityEngine;

public class AIManager : MonoBehaviour
{
    int startPendingPlayers;
    int pendingPlayers;
    int playerStates;
    bool startRound = true;

    void Start()
    {
        GameManager.ReducePendingPlayersCallback += ReducePendingPlayer;
        GameManager.ResetPendingPlayersCallback += ResetPendingPlayers;
        GameManager.AIStateUpdateCallback += OnStateUpdate;
        GameManager.NewRoundCallback += () =>
        {
            ResetPendingPlayers(startPendingPlayers);
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
