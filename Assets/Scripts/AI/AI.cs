using UnityEngine;
using System.Collections;
using TMPro;


public enum AIState
{
    Thinking = 0,
    Bust = 1,
    Hit = 2,
    Stay = 3,
}
[RequireComponent(typeof(Animator))]
public class AI : MonoBehaviour
{
    [Header("AI Setup")]
    [SerializeField]
    Transform startCardPoint;

    [SerializeField]
    float cardAnimationTime = .5f;

    [SerializeField]
    float rightOffset = .2f;

    [SerializeField]
    TMP_Text valueTxt;

    [SerializeField]
    TMP_Text stateTxt;

    [SerializeField]
    TMP_Text nameTXT;

    //Card Reference
    GameObject cardRef;
    int receivedCard;
    bool hasEnoughCard => receivedCard >= 2;
    int currentSumCardValues = 0;

    //AI Info
    bool myTurn = false;
    AIState state = AIState.Hit;
    AIState State
    {
        set
        {
            int stateNum = (int)state;

            state = value;
            switch (state)
            {
                case AIState.Stay:
                    stateTxt.text = "State:Stay";
                    anim.SetTrigger("Stay");
                    break;
                case AIState.Hit:
                    stateTxt.text = "State:Hit";
                    anim.SetBool("RandomWinner", Random.Range(0,2) == 0);
                    anim.SetTrigger("Hit");
                    break;
                case AIState.Bust:
                    stateTxt.text = "State:Bust";
                    anim.SetTrigger("Angry");
                    break;
                case AIState.Thinking:
                    stateTxt.text = "State:Thinking..";
                    break;
                default:
                    break;
            }
        }
    }
    string aiName;
    public string AIName => aiName;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.ChangeTurnCallback += OnChangeTurn;
        GameManager.NewRoundCallback += OnNewRound;
        State = AIState.Hit;
        aiName = "AI" + Random.Range(1, 10000);
        GameManager.UpdatePlayer(AIName, currentSumCardValues);
        nameTXT.text = AIName;
    }
    private void OnNewRound()
    {
        myTurn = false;
        State = AIState.Hit;
        receivedCard = 0;
        cardRef = null;
        currentSumCardValues = 0;
        valueTxt.text = "CurrentValue:";
    }

    private void OnChangeTurn(bool inDealerTurn)
    {
        myTurn = !inDealerTurn;
    }

    private void Update()
    {
        if (myTurn)
        {
            if (state != AIState.Bust && state != AIState.Stay)
            {
                if (!ShouldActBasedOnCardValue()) //if do not want to hit reduce pending players
                {
                    State = AIState.Stay;
                    GameManager.AIStateUpdateCallback?.Invoke(true);
                }
                else
                {
                    State = AIState.Hit;
                    GameManager.AIStateUpdateCallback?.Invoke(false);
                }
            }
            else
            {
                GameManager.AIStateUpdateCallback?.Invoke(true);
            }
            myTurn = false;
        }
    }

    bool ShouldActBasedOnCardValue()
    {
        // Calculate the percentage to decide whether to hit a card
        float percentage = ((21.0f - currentSumCardValues) / 21.0f) * 100.0f;
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
            GameManager.UpdatePlayer(AIName, currentSumCardValues);
            SoundManager.self.PlayClip(ClipType.PlayCard);
        }
    }

    private void HandleCard(Collider other)
    {
        ++receivedCard;
        cardRef = other.gameObject;
        GameManager.StopDragPlayerCallback(cardRef);
        StartCoroutine(CardAnimation(cardAnimationTime));

        if (receivedCard > 1)
        {
            GameManager.ReducePendingPlayersCallback();
            State = AIState.Thinking;
        }

        if (cardRef.TryGetComponent<CardInfo>(out CardInfo card))
        {
            currentSumCardValues += card.Value;
            valueTxt.text = "CurrentValue:" + currentSumCardValues;
            if (currentSumCardValues > 21)
            {
                State = AIState.Bust;
            }
        }
    }

    IEnumerator CardAnimation(float duration)
    {
        Vector3 startPos = cardRef.transform.position;
        Vector3 endPos = startCardPoint.position +
                         transform.right * rightOffset * (receivedCard - 1) +
                         Vector3.up * .005f * (receivedCard - 1);
        Quaternion startRot = cardRef.transform.rotation;
        float rollOffset = receivedCard > 1 ? 180 : 0;
        Vector3 eulerRot = startCardPoint.eulerAngles + new Vector3(rollOffset, 0, 0);
        Quaternion targetRot = Quaternion.Euler(eulerRot);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = elapsedTime / duration;

            cardRef.transform.position = Vector3.Slerp(startPos, endPos, alpha);
            cardRef.transform.rotation = Quaternion.Slerp(startRot, targetRot, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cardRef.transform.position = endPos;
    }
}
