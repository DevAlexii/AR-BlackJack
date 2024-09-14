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
        //Binding callbacks
        GameManager.ChangeAINumCallback += ChangeNumAI;
        GameManager.StartGameCallback += Init;
    }
    void ChangeNumAI(int InAINum)
    {
        //Update player ai numbers
        AINum = InAINum;
    }

    private void Init()
    {
        //Destroy all players in scene
        for (int i = AISpawned.Count - 1; i >= 0; --i)
        {
            Destroy(AISpawned[i]);
            AISpawned.RemoveAt(i);
        }

        //Copy points list and prefab to remove each time a random index without lost it from original list (cause each info are necessary for restart game
        List<Transform> pointsCopy = points.ToList<Transform>();
        List<GameObject> prefabsCopy = AIPrefab.ToList();

        //Spawn Players
        for (int i = 0; i < AINum; i++)
        {
            int index = Random.Range(0, pointsCopy.Count); //random index from pointsCopyList
            int random = Random.Range(0, prefabsCopy.Count);//random index from prefabsCopyList

            GameObject obj = Instantiate(prefabsCopy[random], pointsCopy[index].position, pointsCopy[index].rotation, transform); //Generate gameobject

            //remove random indexes from copy list
            pointsCopy.RemoveAt(index);
            prefabsCopy.RemoveAt(random);

            //Store spawned obj to list
            AISpawned.Add(obj);
        }

        //Set pending player to max player in scene
        GameManager.ResetPendingPlayersCallback(AINum);
    }
}
