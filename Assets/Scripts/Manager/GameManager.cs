using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager
{
    public static Action StartGameCallback;
    public static Action<int> ChangeAINumCallback;
    public static Action<bool> ChangeTurnCallback;
    public static Action ReducePendingPlayersCallback;
    public static Action<int> ResetPendingPlayersCallback;
    public static Action<GameObject> StopDragPlayerCallback;
    public static Action<bool> AIStateUpdateCallback;
    public static Action DelearReceiverCallback;
    public static Action EnableChooseWinnerCallback;

    static Dictionary<string, int> players = new Dictionary<string, int>();

    public static void ResetPlayer()
    {
        players.Clear();
    }
    public static void UpdatePlayer(string inPlayerName, int inPlayerCardNum)
    {
        players[inPlayerName] = inPlayerCardNum;
    }
    public static bool IsCorrectPlayerToWin(string inPlayerName)
    {
        string winnerName = "";
        int highValue = 0;
        foreach (var name in players.Keys)
        {
            int currentValue = players[name];
            if (currentValue <= 21 && currentValue > highValue)
            {
                highValue = currentValue;
                winnerName = name;
            }
        }

        return winnerName == inPlayerName;
    }
}