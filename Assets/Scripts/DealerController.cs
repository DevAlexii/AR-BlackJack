using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DealerController : MonoBehaviour
{
    //Card Position
    [SerializeField]
    Transform cardPoint;

    //UI Reference
    [SerializeField]
    GameObject chooseWinnerBtn;
    [SerializeField]
    GameObject startNewRoundBtn;

    private Collider colliderRef;

    //Dragging setup
    private GameObject draggedCard;
    private bool isDragging;
    [SerializeField]
    private float dragOffsetY = .01f;
    [SerializeField]
    private float releaseCardTimer = .5f;
    private Vector3 lastPosCard;

    //Delear Info
    private bool myTurn = true;
    int receivedCards = 0;
    bool startDraw = true;
    int currentNumCard;
    bool canSelectWinner;

    //VFX 
    [SerializeField]
    private GameObject[] selectionVFX;// 0=>Winner vfx,1=>Loser vfx
    private GameObject spawnedVFX;

    private void Start()
    {
        GameManager.StartGameCallback += OnStartGame;
        GameManager.ChangeTurnCallback += OnChangeTurn;
        GameManager.StopDragPlayerCallback += OnForceStopDrag;
        GameManager.DelearReceiverCallback += OnDelearReceiver;
        GameManager.EnableChooseWinnerCallback += OnEnableChooseWinenr;
        GameManager.NewRoundCallback += OnStartGame;
        chooseWinnerBtn.GetComponent<Button>().onClick.AddListener(OnChooseWinnerClick);
        colliderRef = GetComponent<Collider>();
        colliderRef.enabled = false;
    }

    private void OnEnableChooseWinenr()
    {
        chooseWinnerBtn.SetActive(true);
    }

    private void OnChooseWinnerClick()
    {
        chooseWinnerBtn.SetActive(false);
        canSelectWinner = true;
    }

    private void OnStartGame()
    {
        colliderRef.enabled = false;
        isDragging = false;
        draggedCard = null;
        myTurn = true;
        receivedCards = 0;
        startDraw = true;
        currentNumCard = 0;
        GameManager.UpdatePlayer("Player", currentNumCard);
        chooseWinnerBtn.SetActive(false);
        GameManager.ResetPlayer();
        colliderRef.enabled = false;
        Destroy(spawnedVFX);
        StopAllCoroutines();
    }

    void Update()
    {
        if (!myTurn) return;
        if (canSelectWinner && Input.GetMouseButtonUp(0))
        {
            WinnerTrace();
            return;
        }

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
    private void WinnerTrace()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "AI")
            {
                startNewRoundBtn.SetActive(true);
                canSelectWinner = false;

                string name;
                if (hit.transform.gameObject == gameObject)
                {
                    name = "Player";
                }
                else
                {
                    name = hit.transform.GetComponent<AI>().AIName;
                }

                bool correctSelection = GameManager.IsCorrectPlayerToWin(name);
                int index = correctSelection ? 0 : 1;
                spawnedVFX = Instantiate(selectionVFX[index], hit.transform.position, Quaternion.Euler(-90, 0, 0), hit.transform);
                SoundManager.self.PlayClip(correctSelection ? ClipType.Winner : ClipType.Loser);

                Animator anim = hit.transform.GetComponent<Animator>();
                if (anim)
                {
                    anim.SetTrigger("Winner");
                    anim.SetBool("RandomWinner", Random.Range(0, 2) == 0);
                }
            }
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
        //int layerMask = ~((1 << LayerMask.NameToLayer("Card")) | (1 << LayerMask.NameToLayer("Player")));
        int layerMask = LayerMask.NameToLayer("Table");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            targetPosition = hit.point + Vector3.up * dragOffsetY;
        }
        else
        {
            targetPosition = ray.origin + ray.direction * 2.5f;
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
        HandleCard(other);
        SoundManager.self.PlayClip(ClipType.PlayCard);
    }

    private void HandleCard(Collider other)
    {
        draggedCard.gameObject.layer = 0;
        ++receivedCards;
        isDragging = false;
        StartCoroutine(CardAnimation(.25f));
        colliderRef.enabled = !(receivedCards > 1 && startDraw);
        if (draggedCard.TryGetComponent<CardInfo>(out CardInfo cardInfo))
        {
            currentNumCard += cardInfo.Value;
            GameManager.UpdatePlayer("Player", currentNumCard);
        }
    }

    IEnumerator CardAnimation(float duration)
    {
        Vector3 startPos = draggedCard.transform.position;
        Vector3 endPos = cardPoint.position +
                         Vector3.up * .001f * (receivedCards - 1) +
                         -Vector3.right * .05f * (receivedCards - 1);
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

        if (receivedCards > 1 && startDraw)
        {
            startDraw = false;
            GameManager.ChangeTurnCallback?.Invoke(false);
        }
    }
}