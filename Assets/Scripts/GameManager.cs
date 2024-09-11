using System;
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
}