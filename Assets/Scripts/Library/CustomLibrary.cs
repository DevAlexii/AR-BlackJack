using UnityEngine;

public static class CustomLibrary
{
    public static void Shuffle(ref GameObject[] array)
    {
        System.Random rng = new System.Random();
        int n = array.Length;

        for (int i = n - 1; i > 0; i--)
        {
            //Store random index value to swap
            int j = rng.Next(0, i + 1);

            //Swap elements in Array indexes
            GameObject temp = array[i];
            array[i] = array[j];
            array[j] = temp;

            //Swap elements in world position
            Vector3 tempPos = array[i].transform.position;
            array[i].transform.position = array[j].transform.position;
            array[j].transform.position = tempPos;
        }
    }
}
