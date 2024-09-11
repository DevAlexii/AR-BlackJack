using System.Collections;
using UnityEngine;

public class DealerController : MonoBehaviour
{
    [SerializeField]
    Transform cardPoint;

    private Collider colliderRef;

    private GameObject draggedCard;
    private bool isDragging;
    [SerializeField]
    private float offsetY = .5f;
    [SerializeField]
    private float releaseCardTimer = .5f;
    private Vector3 lastPosCard;
    private bool myTurn = true;
    int receivedCards = 0;
    bool startDraw = true;

    private void Start()
    {
        GameManager.StartGameCallback += OnStartGame;
        GameManager.ChangeTurnCallback += OnChangeTurn;
        GameManager.StopDragPlayerCallback += OnForceStopDrag;
        GameManager.DelearReceiverCallback += OnDelearReceiver;
        colliderRef = GetComponent<Collider>();
        colliderRef.enabled = false;
    }

    private void OnStartGame()
    {
        colliderRef.enabled = false;
        isDragging = false;
        draggedCard = null;
        myTurn = true;
        receivedCards = 0;
        startDraw = true;
        StopAllCoroutines();
    }

    void Update()
    {
        if (!myTurn) return;

        //Attempt start Drag
        if (Input.GetMouseButtonDown(0) && !draggedCard)
        {
            Tracer();
        }

        //Tick Drag
        if (isDragging)
        {
            Drag();
        }

        //End Drag
        if (Input.GetMouseButtonUp(0))
        {
            StopDrag();
        }

    }

    void Tracer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = 1 << LayerMask.NameToLayer("Card");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            draggedCard = hit.collider.gameObject;
            isDragging = true;
            lastPosCard = draggedCard.transform.position;
        }
    }

    private void Drag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPosition;
        int layerMask = ~(1 << LayerMask.NameToLayer("Card"));

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            targetPosition = hit.point + Vector3.up * offsetY;
        }
        else
        {
            targetPosition = ray.origin + ray.direction * 5f;
        }

        draggedCard.transform.position = Vector3.Lerp(draggedCard.transform.position, targetPosition, Time.deltaTime * 5f);
    }

    void StopDrag()
    {
        if (isDragging)
        {
            StartCoroutine(ResetCardCoroutine(releaseCardTimer));
            isDragging = false;
        }
    }

    IEnumerator ResetCardCoroutine(float duration)
    {
        Vector3 startPos = draggedCard.transform.position;
        Vector3 endPos = lastPosCard;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            draggedCard.transform.position = Vector3.Slerp(startPos, endPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        draggedCard.transform.position = endPos;
        draggedCard = null;
    }

    private void OnChangeTurn(bool InDealerTurn)
    {
        myTurn = InDealerTurn;

        if (!myTurn)
        {
            OnForceStopDrag(null);
        }
    }

    private void OnForceStopDrag(GameObject card)
    {
        isDragging = false;
        if (draggedCard)
        {
            draggedCard.GetComponent<Collider>().enabled = false;
            draggedCard = null;
        }
    }

    private void OnDelearReceiver()
    {
        colliderRef.enabled = !colliderRef.enabled;
    }



    private void OnTriggerEnter(Collider other)
    {
        if (startDraw)
        {
            HandleCard(other);
        }
    }

    private void HandleCard(Collider other)
    {
        ++receivedCards;
        isDragging = false;
        StartCoroutine(CardAnimation(.5f));
        colliderRef.enabled = !(receivedCards > 1);
    }

    IEnumerator CardAnimation(float duration)
    {
        Vector3 startPos = draggedCard.transform.position;
        Vector3 endPos = cardPoint.position +
                         transform.right * .2f * (receivedCards - 1) +
                         Vector3.up * .02f * (receivedCards - 1);
        Quaternion startRot = draggedCard.transform.rotation;
        Vector3 eulerRot = cardPoint.eulerAngles + new Vector3(180, 0, 0);
        Quaternion targetRot = Quaternion.Euler(eulerRot);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = elapsedTime / duration;

            draggedCard.transform.position = Vector3.Slerp(startPos, endPos, alpha);
            draggedCard.transform.rotation = Quaternion.Slerp(startRot, targetRot, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        draggedCard.transform.position = endPos;
        GameManager.StopDragPlayerCallback(draggedCard);

        if (receivedCards > 1)
        {
            startDraw = false;
            GameManager.ChangeTurnCallback?.Invoke(false);
        }
       
    }
}

