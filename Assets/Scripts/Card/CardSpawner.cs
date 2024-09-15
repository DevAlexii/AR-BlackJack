using System.Collections.Generic;
using UnityEngine;

public class CardSpawner : MonoBehaviour
{
    [Header("Cards Setup")]
    [SerializeField]
    private GameObject[] cardsType;

    private GameObject[] originalDeck = new GameObject[52]; //52 = original deck size
    private List<GameObject> currentDeck = new List<GameObject>();
    private List<GameObject> removedCards = new List<GameObject>();

    [SerializeField]
    private float offsetY; //Offset deck card posY

    //TextureOffset Setting
    private float tileOffsetX = .1415f;
    private float tileOffsetY = .5f;
    private float startTileOffsetX = .685f;
    private float startTileOffsetY = .571f;

    private void Start()
    {
        GameManager.StartGameCallback += Init;
        GameManager.StopDragPlayerCallback += RemoveCardFromDeck;
    }

    private void Init()
    {
        //Destroy all spawned cards if are in scene
        if (originalDeck[0])
        {
            for (int i = 0; i < originalDeck.Length; ++i)
            {
                Destroy(originalDeck[i]);
            }
        }

        //clear list info
        currentDeck.Clear();
        removedCards.Clear();

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
            GameObject obj = Instantiate(cardsType[cardIndex - 1], pos, Quaternion.Euler(90,0,0), transform);
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
        currentDeck[currentDeck.Count - 1].layer = LayerMask.NameToLayer("Card");
    }
    public void ShuffleCards()
    {
        currentDeck[currentDeck.Count - 1].layer = 0; // remove layer from top card
        CustomLibrary.Shuffle(ref currentDeck); //shuffle
        currentDeck[currentDeck.Count - 1].layer = LayerMask.NameToLayer("Card");// set layer to top card after shuffle
        SoundManager.self.PlayClip(ClipType.Shuffle); //Play shuffle sound
    }

    private void RemoveCardFromDeck(GameObject card)
    {
        //Remove card from current deck and add it to removedcard list and change layer to not be picked anymore during round
        removedCards.Add(card);
        currentDeck.Remove(card);
        currentDeck[currentDeck.Count - 1].layer = LayerMask.NameToLayer("Card");
    }

    public void StartNewRound()
    {
        GameManager.ResetPlayer();

        //On New round move all current card up to leave space on bottom to last removed cards
        foreach (var obj in currentDeck)
        {
            obj.transform.position += Vector3.up * (offsetY * removedCards.Count);
            obj.layer = 0;
        }


        for (int i = 0; i < removedCards.Count; i++)
        {
            GameObject obj = removedCards[i];
            currentDeck.Insert(i, obj);
            obj.transform.position = transform.position + Vector3.up * (i * offsetY);
            obj.transform.rotation = Quaternion.Euler(90, 0, 0);
            obj.layer = 0;
        }
        removedCards.Clear();


        currentDeck[currentDeck.Count - 1].layer = LayerMask.NameToLayer("Card");//Set layer on new top card

        GameManager.NewRoundCallback?.Invoke();
    }

    public void OnFinishRound()
    {
        //Show all players hide cards
        foreach (var obj in removedCards)
        {
            if (obj.transform.eulerAngles.x <= 90f)
            {
                obj.transform.eulerAngles += new Vector3(180, 0, 0);
            }
        }
    }
}