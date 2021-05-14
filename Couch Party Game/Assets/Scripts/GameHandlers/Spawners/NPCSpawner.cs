using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NPCSpawner : MonoBehaviour
{
    public bool spawnBasedOnPlayerAmount;

    public List<GameObject> availableNPCs = new List<GameObject>();
    [SerializeField] GameObject[] NPCPrefabs;
    public int minNPC, maxNPC;

    [SerializeField] Transform[] allSpawnLocations;
    List<Transform> availableSpawnLocations = new List<Transform>();
    [SerializeField] bool spawnAroundLocation;
    [SerializeField] float spawnOffset = 1;

    [SerializeField] float autoKillDelay = 0.1f;

    public UnityAction onCompletedSpawn, onRemovedNPCS;

    public Score aliveNPCCounter;

    Coroutine killAllNpcsActiveRoutine;
    public UnityAction onKilledAllNpcs;

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnAdditionalNPC(int amount)
    {

    }

    public void CheckNPCCount()
    {
        if(availableNPCs.Count <= 0)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameHandler>().FinishGame();
        }
    }

    public void SpawnToMax()
    {
        int amountToSpawn = maxNPC - availableNPCs.Count;
    }

    public GameObject SpawnNPC(Vector3 location)
    {
        if(NPCPrefabs.Length > 0)
        {
            GameObject selectedPrefab = NPCPrefabs[Random.Range(0, NPCPrefabs.Length)];
            GameObject spawnedNPC = Instantiate(selectedPrefab, location, Quaternion.identity);
            spawnedNPC.GetComponent<Passenger>().SetRandomIdleState();
            spawnedNPC.transform.rotation = Quaternion.LookRotation(new Vector3(Random.Range(-1, 2), 0, Random.Range(-1, 2)));
            availableNPCs.Add(spawnedNPC);
            aliveNPCCounter.ChangeScore(1);
            return spawnedNPC;
        }

        return null;
    }

    public void KillAliveNPCS()
    {
        if(killAllNpcsActiveRoutine == null)
        {
            StartCoroutine(KillAliveNPCSRoutine());
        }
    }

    IEnumerator KillAliveNPCSRoutine()
    {
        for(int i = availableNPCs.Count - 1; i >= 0; i--)
        {
            availableNPCs[i].GetComponent<Entity>().ConfirmDeath();
            yield return null;
        }

        killAllNpcsActiveRoutine = null;

        if(onKilledAllNpcs != null)
        {
            onKilledAllNpcs.Invoke();
        }
    }

    public IEnumerator RemoveAllNPC()
    {
        for(int i = availableNPCs.Count - 1; i >= 0; i--)
        {
            Destroy(availableNPCs[i]);
            availableNPCs.RemoveAt(i);
            yield return null;
        }

        availableSpawnLocations = new List<Transform>(allSpawnLocations);

        if(onRemovedNPCS != null)
        {
            onRemovedNPCS.Invoke();
        }
    }

    public void StartSpawnNPC()
    {
        Debug.Log("FSDFSDF");
        onRemovedNPCS += ActualStartSpawnNPC;
        StartCoroutine(RemoveAllNPC());
    }

    public void ActualStartSpawnNPC()
    {
        onRemovedNPCS -= ActualStartSpawnNPC;
        StartCoroutine(SpawnNPCRoutine(Random.Range(minNPC, maxNPC + 1)));
    }

    public IEnumerator SpawnNPCRoutine(int amount)
    {
        if (spawnBasedOnPlayerAmount)
        {
            amount *= PlayerManager.instance.connectedToLobbyPlayers.Count;
        }

        for(int i = 0; i < amount; i++)
        {
            if(availableSpawnLocations.Count == 0)
            {
                break;
            }

            Transform selectedSpawn = availableSpawnLocations[Random.Range(0, availableSpawnLocations.Count)];
            SpawnNPC(selectedSpawn.position);
            availableSpawnLocations.Remove(selectedSpawn);

            yield return null;
        }

        if(onCompletedSpawn != null)
        {
            onCompletedSpawn.Invoke();
            onCompletedSpawn = null;
        }
    }
}
