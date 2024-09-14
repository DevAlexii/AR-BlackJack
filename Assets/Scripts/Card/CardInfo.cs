using UnityEngine;


public class CardInfo : MonoBehaviour
{
    private int value;
    public int Value { get => value; }

    public void Init(int InValue)
    {
        //set correct value to cards according blackjack rule
        //1 = 11;
        //each figure card  = 10;
        //rest same value card

        //+1 cause spawn iterator start count from 0
        if (InValue + 1 == 1){
            value = 11;
            return;
        }
       
        value = Mathf.Clamp(InValue + 1,2,10); 
    }
}