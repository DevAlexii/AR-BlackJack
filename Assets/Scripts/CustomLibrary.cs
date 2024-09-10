using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomLibrary
{
    private static GameManager GameManager;

    public static void Shuffle(ref GameObject[] array)
    {
        System.Random rng = new System.Random();
        int n = array.Length;

        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(0, i + 1);

            GameObject temp = array[i];
            array[i] = array[j];
            array[j] = temp;

            Vector3 tempPos = array[i].transform.position;
            array[i].transform.position = array[j].transform.position;
            array[j].transform.position = tempPos;
        }
    }

    public static void SetGameManager(GameManager InGameManager)
    {
        GameManager = InGameManager;
    }
    public static GameManager GetGameManager()
    {
        return GameManager;
    }
}
