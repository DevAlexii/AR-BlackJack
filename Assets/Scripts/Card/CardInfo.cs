using UnityEngine;


public class CardInfo : MonoBehaviour
{
    private int value;
    public int Value { get => value; }

    public void Init(int InValue)
    {
        //+1 cause spawn iterator start count from 0

        if (InValue + 1 == 1){
            value = 11;
            return;
        }
       
        value = Mathf.Clamp(InValue + 1,2,10); 
    }
}