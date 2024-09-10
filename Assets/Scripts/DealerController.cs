using System.Collections;
using UnityEngine;

public class DealerController : MonoBehaviour
{
    private GameObject draggedCard;
    private bool isDragging;
    [SerializeField]
    private float offsetY = .5f;
    [SerializeField]
    private float releaseCardTimer = .5f;
    private Vector3 lastPosCard;
    private bool isMineTurn = true;

    private void Start()
    {
        GameManager.ChangeTurnCallback += OnChangeTurn;
    }

    void Update()
    {
        if(!isMineTurn) return;

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
            draggedCard.GetComponent<Collider>().enabled = false;
            lastPosCard = draggedCard.transform.position;
        }
    }

    private void Drag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 targetPosition;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            targetPosition = hit.point + Vector3.up * offsetY;
        }
        else
        {
            targetPosition = ray.origin + ray.direction * 5f;
        }

        draggedCard.transform.position = Vector3.Lerp(draggedCard.transform.position,targetPosition,Time.deltaTime * 5f);
    }

    void StopDrag()
    {
        if (isDragging)
        {
            StartCoroutine(ResetCardCoroutine(releaseCardTimer));
            draggedCard.GetComponent<Collider>().enabled = true;
            isDragging  = false;
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
        isMineTurn = InDealerTurn;

        if (!isMineTurn)
        {
            isDragging = false;
            draggedCard = null;
        }
    }
}
