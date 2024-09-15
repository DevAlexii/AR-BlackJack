using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class BasePlayer : MonoBehaviour
{
    //Card Position && Animation setup
    [SerializeField]
    protected Transform cardPoint;
    [SerializeField]
    protected float cardAnimationTime = .5f;
    [SerializeField]
    protected float rightOffset = .2f;
    protected float upOffset = .001f;
    [SerializeField]
    protected GameObject fadingCardObj;

    //Card Info
    protected int receivedCards = 0;
    protected int cardsSum = 0;

    //PlayerInfo
    protected bool isDelear = false;
    protected bool myTurn = false;
    protected string playerName;
    public string PlayerName => playerName;

    protected virtual void Start()
    {
        GameManager.UpdatePlayer(playerName, cardsSum);
        GameManager.NewRoundCallback += OnNewRound;
        GameManager.ChangeTurnCallback += OnChangeTurn;
    }
    protected virtual void OnNewRound() { }
    protected virtual void OnChangeTurn(bool inDelearTurn) { }

    protected virtual void HandleCard(Collider other)
    {
        ++receivedCards;
        GameManager.StopDragPlayerCallback(other.gameObject);
        StartCoroutine(CardAnimation(cardAnimationTime,other.gameObject));
        if (other.TryGetComponent<CardInfo>(out CardInfo card))
        {
            cardsSum += card.Value;
        }
        GameManager.UpdatePlayer(playerName, cardsSum);
        SoundManager.self.PlayClip(ClipType.PlayCard);
    }
    IEnumerator CardAnimation(float duration,GameObject obj)
    {
        Vector3 startPos = obj.transform.position;
        Vector3 endPos = cardPoint.position +
                         transform.right * rightOffset * (receivedCards - 1) +
                         Vector3.up * .005f * (receivedCards - 1);
        Quaternion startRot = obj.transform.rotation;
        float rollOffset = receivedCards > 1 || isDelear ? 180 : 0;
        Vector3 eulerRot = cardPoint.eulerAngles + new Vector3(rollOffset, 0, 0);
        Quaternion targetRot = Quaternion.Euler(eulerRot);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = elapsedTime / duration;

            obj.transform.position = Vector3.Slerp(startPos, endPos, alpha);
            obj.transform.rotation = Quaternion.Slerp(startRot, targetRot, alpha * 1.5f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        obj.transform.position = endPos;
    }

    public virtual string GetState() { return ""; }
}
