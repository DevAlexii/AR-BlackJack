using UnityEngine;
using System;

public class CardSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] cardsType;
    private GameObject[] cards;

    [SerializeField]
    private float offsetY;

    private void Start()
    {
        CustomLibrary.GetGameManager().StartGameCallback += Init;
    }

    private void Init()
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
        CustomLibrary.Shuffle(ref cards);
    }
}

public class ArrayUtilities
{
    

}
