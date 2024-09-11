using System.Collections.Generic;
using UnityEngine;

public static class CustomLibrary
{
    public static void Shuffle(ref List<GameObject> container)
    {
        System.Random rng = new System.Random();
        int n = container.Count;

        for (int i = n - 1; i > 0; i--)
        {
            //Store random index value to swap
            int j = rng.Next(0, i + 1);

            //Swap elements in Array indexes
            GameObject temp = container[i];
            container[i] = container[j];
            container[j] = temp;

            //Swap elements in world position
            Vector3 tempPos = container[i].transform.position;
            container[i].transform.position = container[j].transform.position;
            container[j].transform.position = tempPos;
        }
    }
}
