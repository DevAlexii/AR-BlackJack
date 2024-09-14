using UnityEngine;

public enum AIState
{
    Thinking = 0,
    Bust = 1,
    Hit = 2,
    Stand = 3,
}
[RequireComponent(typeof(Animator))]
public class AI : BasePlayer
{
    bool hasEnoughCard => receivedCards >= 2;

    //AI Info
    AIState state;
    public AIState State
    {
        get => state;
        set
        {
            state = value;
            if (!anim) return;

            switch (state)
            {
                case AIState.Stand:
                    anim.SetTrigger("Stay");
                    break;
                case AIState.Hit:
                    anim.SetBool("RandomWinner", Random.Range(0, 2) == 0);
                    anim.SetTrigger("Hit");
                    break;
                case AIState.Bust:
                    anim.SetTrigger("Angry");
                    break;
                case AIState.Thinking:
                    break;
                default:
                    break;
            }
        }
    }
    private Animator anim;

    protected override void Start()
    {
        anim = GetComponent<Animator>();
        GameManager.ChangeTurnCallback += OnChangeTurn;
        GameManager.NewRoundCallback += OnNewRound;
        State = AIState.Hit;
        playerName = "AI" + Random.Range(1, 10000);
        base.Start();
    }
    private void OnNewRound()
    {
        myTurn = false;
        State = AIState.Hit;
        receivedCards = 0;
        cardsSum = 0;
    }

    private void OnChangeTurn(bool inDealerTurn)
    {
        myTurn = !inDealerTurn;
    }

    private void Update()
    {
        if (myTurn)
        {
            if (state != AIState.Bust && state != AIState.Stand)
            {
                if (!ShouldActBasedOnCardValue()) 
                {
                    State = AIState.Stand;
                    GameManager.AIStateUpdateCallback?.Invoke(true); //Reduce pending players when is in state stand
                }
                else
                {
                    State = AIState.Hit;
                    GameManager.AIStateUpdateCallback?.Invoke(false);
                }
            }
            else
            {
                GameManager.AIStateUpdateCallback?.Invoke(true); //Reduce pending players when is in state stand/bust
            }
            myTurn = false; //Finish AI Turn
        }
    }

    bool ShouldActBasedOnCardValue()
    {
        // Calculate the percentage to decide whether to hit a card
        float percentage = ((21.0f - cardsSum) / 21.0f) * 100.0f;
        percentage = Mathf.Clamp(percentage, .0f, 100.0f);

        // Random decision not based on intelligence
        int randomCheck = Random.Range(0, 100);

        return randomCheck <= percentage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasEnoughCard || state == AIState.Hit)
        {
            HandleCard(other);
        }
    }
    protected override void HandleCard(Collider other)
    {
        base.HandleCard(other);
        if (receivedCards > 1)
        {
            GameManager.ReducePendingPlayersCallback(); //When Get enough cards change state and reduce pending players
            State = AIState.Thinking;
        }
        if (cardsSum > 21)
        {
            State = AIState.Bust; //when reach cards sum over 21 it means player has bust
        }
    }
}