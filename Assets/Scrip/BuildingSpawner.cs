using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    [System.Serializable]
    public class BuildingOption
    {
        public GameObject prefab;
        public Building.BuildingType type;
        public int count;
        public bool canWarp;
        public Transform warpTarget;
    }

    public Transform spawnPoint;
    public List<BuildingOption> buildingOptions;
    public float spawnInterval = 1.0f;

    private Queue<BuildingOption> spawnQueue = new Queue<BuildingOption>();

    void Start()
    {
        PrepareSpawnQueue();
        StartCoroutine(SpawnSequence());
    }

    void PrepareSpawnQueue()
    {
        List<BuildingOption> warpList = new List<BuildingOption>();
        List<BuildingOption> nonWarpList = new List<BuildingOption>();

        foreach (var option in buildingOptions)
        {
            for (int i = 0; i < option.count; i++)
            {
                if (option.canWarp)
                    warpList.Add(option);
                else
                    nonWarpList.Add(option);
            }
        }

        List<BuildingOption> shuffled = new List<BuildingOption>();

        if (nonWarpList.Count > 0)
        {
            int firstIndex = Random.Range(0, nonWarpList.Count);
            spawnQueue.Enqueue(nonWarpList[firstIndex]);
            nonWarpList.RemoveAt(firstIndex);
        }

        shuffled.AddRange(nonWarpList);
        shuffled.AddRange(warpList);

        while (shuffled.Count > 0)
        {
            int index = Random.Range(0, shuffled.Count);
            spawnQueue.Enqueue(shuffled[index]);
            shuffled.RemoveAt(index);
        }
    }

    IEnumerator SpawnSequence()
    {
        while (spawnQueue.Count > 0)
        {
            BuildingOption option = spawnQueue.Dequeue();

            GameObject building = Instantiate(option.prefab, spawnPoint.position, Quaternion.identity);
            Building buildingScript = building.GetComponent<Building>();

            if (buildingScript != null)
            {
                buildingScript.type = option.type;
                buildingScript.canWarpOnGround = option.canWarp;

                if (option.canWarp && option.warpTarget != null)
                {
                    buildingScript.warpTarget = option.warpTarget;
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
