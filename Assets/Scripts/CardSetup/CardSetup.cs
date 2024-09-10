using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
struct CardInfo
{
    public int num;
}
enum Suit
{
    Diamonds,
    Clubs,
    Hearths,
    Spades
}

public class CardSetup : MonoBehaviour
{
    [SerializeField]
    private CardInfo cardInfo;

    private void Awake()
    {
        MeshRenderer meshRender = GetComponent<MeshRenderer>();
        meshRender.materials[0].SetTextureOffset("Offset", new Vector2(10, 10));
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
