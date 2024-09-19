using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DealerController : BasePlayer
{
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
    bool startDraw = true;
    bool canSelectWinner;

    //VFX 
    [SerializeField]
    private GameObject[] selectionVFX;// 0=>Winner vfx,1=>Loser vfx
    private GameObject spawnedVFX;

    //Popup
    [SerializeField]
    GameObject popupDealer;

    protected override void Start()
    {
        GameManager.StartGameCallback += OnStartGame;
        GameManager.StopDragPlayerCallback += OnForceStopDrag;
        GameManager.DelearReceiverCallback += OnDelearReceiver;
        GameManager.EnableChooseWinnerCallback += OnEnableChooseWinenr;
        GameManager.GameOverCallback += () =>
        {
            chooseWinnerBtn.SetActive(false);
            startNewRoundBtn.SetActive(false);
            chooseWinnerBtn.transform.parent.gameObject.SetActive(false);
            fadingCardObj.SetActive(false);
        };
        chooseWinnerBtn.GetComponent<Button>().onClick.AddListener(OnChooseWinnerClick);
        colliderRef = GetComponent<Collider>();
        colliderRef.enabled = false;
        playerName = "Player";
        isDelear = true;
        fadingCardObj.SetActive(false);
        winnerSelected.Clear();
        base.Start();
    }
    protected override void OnNewRound()
    {
        OnStartGame();
    }
    private void OnEnableChooseWinenr()
    {
        chooseWinnerBtn.SetActive(true);
    }

    private void OnChooseWinnerClick()
    {
        chooseWinnerBtn.SetActive(false);
        canSelectWinner = true;
        fadingCardObj.SetActive(false);
    }

    private void OnStartGame()
    {
        hasAce = false;
        GameManager.ResetPlayer();
        isDragging = false;
        draggedCard = null;
        myTurn = true;
        receivedCards = 0;
        startDraw = true;
        cardsSum = 0;
        GameManager.UpdatePlayer("Player", cardsSum);
        chooseWinnerBtn.SetActive(false);
        colliderRef.enabled = false;
        Destroy(spawnedVFX);
        StopAllCoroutines();
        fadingCardObj.SetActive(false);
        winnerSelected.Clear();
        DisablePopupDelear();
    }

    void Update()
    {
        if (!myTurn) return;
        if (canSelectWinner && Input.GetMouseButtonDown(0))
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

    List<string> winnerSelected = new List<string>();
    private void WinnerTrace()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.tag == "AI")
            {
                string name = hit.transform.GetComponent<BasePlayer>().PlayerName;
                winnerSelected.Add(name);

                if (winnerSelected.Count >= GameManager.GetWinnersAmount())
                {
                    startNewRoundBtn.SetActive(true);
                    canSelectWinner = false;


                    bool correctSelection = GameManager.IsCorrectPlayerToWin(winnerSelected);

                    GameManager.AddHappinessCallback?.Invoke(correctSelection ? .25f : -.25f);
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
    }

    void Tracer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int layerMask = 1 << LayerMask.NameToLayer("Card") | 1 << LayerMask.NameToLayer("Player");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Vector3 playerPos = hit.transform.position + hit.transform.forward * 1f;
                if (hit.transform.TryGetComponent<BasePlayer>(out BasePlayer player))
                {
                    PlayerCardsDebugger.Instance.DebuggerShowPopUp(player.PlayerName, playerPos, player.GetState());
                }
                return;
            }

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
        draggedCard.transform.position = Vector3.Slerp(draggedCard.transform.position, targetPosition, Time.deltaTime * 8f);
    }

    [SerializeField]
    float force;
    void StopDrag()
    {
        if (isDragging && draggedCard)
        {
            //StartCoroutine(ResetCardCoroutine(releaseCardTimer));
            isDragging = false;

            Rigidbody rb = draggedCard.GetComponent<Rigidbody>();
            if (!rb)
            {
                rb = draggedCard.AddComponent<Rigidbody>();
            }
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 starPos = ray.origin + (ray.direction * 2.0f);
            Vector3 dir = (starPos - transform.position) + Vector3.up;
            dir.Normalize();

            rb.AddForce(dir * force);

            GameManager.ThrowCardCallback?.Invoke(draggedCard);
            draggedCard = null;
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

    protected override void OnChangeTurn(bool InDealerTurn)
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
            draggedCard = null;
        }
    }

    private void OnDelearReceiver()
    {
        colliderRef.enabled = !colliderRef.enabled;
        fadingCardObj.SetActive(colliderRef.enabled && cardsSum < 21);

        if (colliderRef.enabled && !startDraw)
        {
            string txtPopup = cardsSum >= 16 ? "Dont draw card" : "Draw card";
            popupDealer.GetComponentInChildren<TMP_Text>().text = txtPopup;
            popupDealer.SetActive(true);
            Invoke(nameof(DisablePopupDelear), 1f);
        }
    }
    private void DisablePopupDelear()
    {
        popupDealer.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (cardsSum < 21)
        {
            HandleCard(other);
            SoundManager.self.PlayClip(ClipType.PlayCard);
        }
    }

    protected override void HandleCard(Collider other)
    {
        base.HandleCard(other);
        other.gameObject.layer = 0;
        isDragging = false;
        colliderRef.enabled = !(receivedCards > 1 && startDraw);
        if (receivedCards > 1 && startDraw)
        {
            startDraw = false;
            fadingCardObj.SetActive(false);
            GameManager.ChangeTurnCallback?.Invoke(false);
        }

        if (cardsSum >= 21)
        {
            fadingCardObj.SetActive(false);
        }
    }
}