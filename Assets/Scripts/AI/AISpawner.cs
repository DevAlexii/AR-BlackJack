using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    [SerializeField]
    private Transform[] points;

    [SerializeField]
    private GameObject[] AIPrefab;

    private List<GameObject> AISpawned = new List<GameObject>();

    int AINum = 1;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.ChangeAINumCallback += ChangeNumAI;
        GameManager.StartGameCallback += Init;
    }
    void ChangeNumAI(int InAINum)
    {
        AINum = InAINum;
    }

    private void Init()
    {
        for (int i = AISpawned.Count - 1; i >= 0; --i)
        {
            Destroy(AISpawned[i]);
            AISpawned.RemoveAt(i);
        }


        List<Transform> list = points.ToList<Transform>();
        List<GameObject> prefabsCopy = AIPrefab.ToList();

        for (int i = 0; i < AINum; i++)
        {
            int index = Random.Range(0, list.Count);
            int random = Random.Range(0, prefabsCopy.Count);
            GameObject obj = Instantiate(prefabsCopy[random], list[index].position, list[index].rotation, transform);
            list.RemoveAt(index);
            prefabsCopy.RemoveAt(random);
            AISpawned.Add(obj);
        }
        GameManager.ResetPendingPlayersCallback(AINum);
    }
}
