using System;

public static class GameManager
{
    public static Action StartGameCallback;
    public static Action<int> ChangeAINumCallback;
    public static Action<bool> ChangeTurnCallback;
    public static Action ReducePendingPlayersCallback;
    public static Action<int> ResetPendingPlayersCallback;
}