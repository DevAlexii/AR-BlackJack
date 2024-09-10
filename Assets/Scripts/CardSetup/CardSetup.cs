using UnityEngine;


public class CardSetup : MonoBehaviour
{
    private float offsetX = .137f;
    private float offsetY = .51f;

    private float startOffsetX = -0.32f;
    private float startOffsetY = -0.37f;

    private int value;
    public int Value { get => value; }

    public void Init(int InValue)
    {
        value = InValue;

        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Material material = renderer.material;

            int column = value % 7;
            int row = value / 7;
            material.mainTextureOffset = new Vector2(startOffsetX + column * offsetX, startOffsetY + row * offsetY);
        }
    }
}