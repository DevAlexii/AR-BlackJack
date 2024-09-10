using UnityEngine;

public class AIManager : MonoBehaviour
{
    int pendingPlayers;

    void Start()
    {
        GameManager.ReducePendingPlayersCallback += ReducePendingPlayer;
        GameManager.ResetPendingPlayersCallback += ResetPendingPlayers;
    }
    private void ReducePendingPlayer()
    {
        pendingPlayers--;
    }
    private void ResetPendingPlayers(int InPendingPlayers)
    {
        pendingPlayers = InPendingPlayers;  
    }
}
