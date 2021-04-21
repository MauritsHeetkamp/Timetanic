using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] bool repeat = true;
    [SerializeField] bool startOnStart = true;
    [SerializeField] Spawner[] spawners;

    List<Coroutine> currentRoutines = new List<Coroutine>();

    // Start is called before the first frame update
    void Start()
    {
        if (startOnStart)
        {
            StartSpawn();
        }
    }


    public void StartSpawn()
    {
        foreach (Spawner spawner in spawners)
        {
            currentRoutines.Add(StartCoroutine(spawner.Spawn(objectToSpawn, repeat)));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    [System.Serializable]
    public class Spawner
    {
        public Transform spawnLocation;
        public float minSpawnDelay, maxSpawnDelay;
        public float lifetime = 3;

        public IEnumerator Spawn(GameObject spawnedItem, bool shouldRepeat)
        {
            while (shouldRepeat)
            {
                yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
                GameObject spawnedObject = Instantiate(spawnedItem, spawnLocation.position, spawnLocation.rotation);

                if(lifetime > 0)
                {
                    Destroy(spawnedObject, lifetime);
                }
            }
        }
    }
}
