using UnityEngine;


public class CardInfo : MonoBehaviour
{
    private int value;
    public int Value { get => value; }

    public void Init(int InValue)
    {
        value = InValue;
    }
}