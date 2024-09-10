using UnityEngine;
using System;

public class CardSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] cardsType;
    private GameObject[] cards;

    [SerializeField]
    private float offsetY;

    private void Awake()
    {
        cards = new GameObject[52];

        int cardIndex = 0;

        for (int i = 0; i < 52; i++)
        {
            if( i % 13 == 0)
            {
                ++cardIndex;
            }

            Vector3 pos = transform.position + Vector3.up * (i * offsetY);
            GameObject obj = Instantiate(cardsType[cardIndex - 1], pos, transform.rotation, transform);
            cards[i] = obj;

            if (obj != null && obj.TryGetComponent<CardSetup>(out CardSetup card))
            {
                card.Init(i - ((cardIndex - 1) * 13));
            }

        }

    }
    public void ShuffleCards()
    {
        ArrayUtilities.Shuffle(ref cards);
    }
}

public class ArrayUtilities
{
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

}
