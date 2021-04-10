using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    List<GameObject> allSpawnedNPCS = new List<GameObject>();
    public int minNPC, maxNPC;

    public List<Transform> spawnLocations = new List<Transform>();
    [SerializeField] bool spawnAroundLocation;
    [SerializeField] float spawnOffset = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnAdditionalNPC(int amount)
    {

    }

    public void SpawnToMax()
    {
        int amountToSpawn = maxNPC - allSpawnedNPCS.Count;
    }

    public void RemoveAllNPC()
    {
        for(int i = allSpawnedNPCS.Count - 1; i >= 0; i--)
        {
            Destroy(allSpawnedNPCS[i]);
            allSpawnedNPCS.RemoveAt(i);
        }
    }

    public void SpawnNPC()
    {
        RemoveAllNPC();

        int spawnAmount = Random.Range(minNPC, maxNPC + 1);

        for(int i = 0; i < spawnAmount; i++)
        {
            if(spawnLocations.Count == 0)
            {
                break;
            }

            Transform selectedSpawn = spawnLocations[Random.Range(0, spawnLocations.Count)];


        }
    }
}
