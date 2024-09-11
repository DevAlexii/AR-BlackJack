using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [Header("Cards Setup")]
    [SerializeField]
    private GameObject[] cardsType;

    private GameObject[] originalDeck  = new GameObject[52];
    private List<GameObject> currentDeck = new List<GameObject>();

    [SerializeField]
    private float offsetY;


    //TextureOffset Setting
    private float tileOffsetX = .137f;
    private float tileOffsetY = .51f;
    private float startTileOffsetX = -0.32f;
    private float startTileOffsetY = -0.37f;

    private void Start()
    {
        GameManager.StartGameCallback += Init;
        GameManager.StopDragPlayerCallback += RemoveCardFromDeck;
    }

    private void Init()
    {
        if (originalDeck[0])
        {
            for (int i = 0; i < originalDeck.Length; ++i)
            {
                Destroy(originalDeck[i]);
            } 
        }

        int cardIndex = 0;

        for (int i = 0; i < 52; i++)
        {
            //Change cardtype to spawn when full suit card are spawned
            if (i % 13 == 0)
            {
                ++cardIndex;
            }

            //Calculate new card position
            Vector3 pos = transform.position + Vector3.up * (i * offsetY);

            //Instantiate new game object and store it in the array
            GameObject obj = Instantiate(cardsType[cardIndex - 1], pos, transform.rotation, transform);
            originalDeck[i] = obj;
            currentDeck.Add(obj);

            //Change offsetTexture in spawned card material to change visible number
            if (obj != null && obj.TryGetComponent<CardInfo>(out CardInfo card))
            {
                int cardValue = i - ((cardIndex - 1) * 13); //card index - 1 * 13 => when "i" goes over 13 has to restart from 1 the card value

                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Material material = renderer.material;

                    //Card texture has 7 columns and 2 rows
                    int column = cardValue % 7; // using module to restart counter when reach card value greater then columns
                    int row = cardValue / 7; // using division to go at the head 
                    material.mainTextureOffset = new Vector2(startTileOffsetX + column * tileOffsetX, startTileOffsetY + row * tileOffsetY);
                }

                card.Init(cardValue);
            }
        }
    }
    public void ShuffleCards()
    {
        CustomLibrary.Shuffle(ref currentDeck);
    }

    private void RemoveCardFromDeck(GameObject card)
    {
        currentDeck.Remove(card);
    }
}