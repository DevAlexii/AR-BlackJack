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
    public static Action NewRoundCallback;
    public static Action<float> AddHappinessCallback;
    public static Action GameOverCallback;
    public static Action<GameObject> ThrowCardCallback;

    static Dictionary<string, int> players = new Dictionary<string, int>();

    public static void ResetPlayer()
    {
        players.Clear(); //Clear all players infos
    }
    public static void UpdatePlayer(string inPlayerName, int inPlayerCardNum)
    {
        //Add/Update player in scene 
        players[inPlayerName] = inPlayerCardNum;
        PlayerCardsDebugger.Instance.DebuggerUpdatePlayerStat(inPlayerName, inPlayerCardNum);
    }

    public static bool IsCorrectPlayerToWin(List<string> inPlayerName)
    {
        //Check if selected player is a winner by is cardsSum value
        //If there are 2 or more player with same value, it is not managed according black jack rule, first found in the dictionary is the winner

        //string winnerName = "";
        //int highValue = 0;
        //foreach (var name in players.Keys)
        //{
        //    int currentValue = players[name];
        //    if (currentValue <= 21 && currentValue > highValue)
        //    {
        //        highValue = currentValue;
        //        winnerName = name;
        //    }
        //}

        foreach (var name in winnersList)
        {
            if (!inPlayerName.Contains(name))
            {
                return false;
            }
        }
        return true;
    }

    static List<string> winnersList = new List<string>();
    public static void UpdateWinnerList()
    {
        CalculateMax();
        winnersList.Clear(); 

        foreach (var key in players.Keys)
        {
            int currentValue = players[key];

            if (currentValue >= maxValue && currentValue <= 21)
            {
                winnersList.Add(key);
            }
        }
    }

    static int maxValue;
    private static void CalculateMax()
    {
        maxValue = 0;
        foreach (var key in players.Keys)
        {
            int currentValue = players[key];

            if (maxValue < currentValue && currentValue <= 21)
            {
                maxValue = currentValue;
            }
        }
    }

    public static int GetWinnersAmount() => winnersList.Count;
}