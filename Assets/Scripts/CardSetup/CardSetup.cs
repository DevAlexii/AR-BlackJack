using System;
using UnityEngine;


public class CardSetup : MonoBehaviour
{
    private float offsetX = .137f;
    private float offsetY = .51f;

    [SerializeField]
    private int num;

    private float startOffsetX = -0.32f;
    private float startOffsetY = -0.37f;

    public void Init(int InNum)
    {
        num = InNum;

        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Material material = renderer.material;

            int column = num  % 7;
            int row = num  / 7;
            material.mainTextureOffset = new Vector2(startOffsetX + column * offsetX, startOffsetY + row * offsetY);
        }
    }
}