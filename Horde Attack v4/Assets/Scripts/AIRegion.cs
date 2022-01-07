using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtils;
public class AIRegion : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnProbabilities
    {
        public SO_Minion minionType;
        public int spawnWeighting;
    }

    
    private List<Transform> spawnPoints;
    private Collider2D regionCol;

    [Header("Spawn info")]
    public int minHordeCount;
    public int maxHordeCount;
    public int minMinionCountPerHorde;
    public int maxMinionCountPerHorde;
    public SpawnProbabilities[] spawns;

    [Header("Current AI info")]
    public List<HordeControllerAI> aIHordes;


    private int operationsCounter;

    private void Awake()
    {
        operationsCounter = Random.Range(1, 1000);
        regionCol = GetComponent<Collider2D>();

        spawnPoints = new List<Transform>();

        //sets up spawn point list based on all objects tagged 'spawn point' in the collider
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("AISpawn"))
            if (regionCol.OverlapPoint(g.transform.position))
                spawnPoints.Add(g.transform);
    }

    // Update is called once per frame
    void Update()
    {
        operationsCounter++;
        if (operationsCounter >= 2000)
        {
            operationsCounter = 0;
            //remove hordes that have been destroyed from the reference list
            for (int i = aIHordes.Count - 1; i >= 0; i--)
                if (aIHordes[i] == null)
                {
                    aIHordes.RemoveAt(i);
                    //will also increase size of subsequent hordes
                    //maxMinionCountPerHorde++;
                }


            if (aIHordes.Count < maxHordeCount)
                while (aIHordes.Count < minHordeCount)
                    spawnAIHorde();
        }
    }

    private void spawnAIHorde()
    {
        Queue<SO_Minion> minionSpawnQueue = new Queue<SO_Minion>();
        for (int i = 0; i < Random.Range(minMinionCountPerHorde, maxMinionCountPerHorde); i++)
        {
            minionSpawnQueue.Enqueue(pickMinionType());
        }
        Vector2 spawnPoint = getSpawnPointInRegion();
        aIHordes.Add(HordeControllerAI.Create(minionSpawnQueue, this, getRandomPointInRegion(spawnPoint), spawnPoint));
    }

    public Vector2 getRandomPointInRegion()
    {
        return JUtilsClass.RandomPointInCollider(regionCol);
    }

    public Vector2 getRandomPointInRegion(Vector2 origin)
    {
        Vector2 pos;
        RaycastHit2D hit;
        int count = 0;
        do
        {
            count++;

            pos = JUtilsClass.RandomPointInCollider(regionCol);

            //makes sure that it doesn't pass through a wall
            hit = Physics2D.Raycast(origin, (pos - origin).normalized, Vector2.Distance(origin, pos), layerMask: 1 << LayerMask.NameToLayer("Walls"));

            if (count > 1000)
            {
                Debug.LogWarning("Count exceeded in getting random point in region: " + this.gameObject.name + "\n impact with: " + hit.collider.name);
                return pos;
            }
        }
        while (hit.collider != null);
        return pos;
    }

    private Vector2 getSpawnPointInRegion()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        return spawnPoint.transform.position;
    }

    private SO_Minion pickMinionType()
    {
        //Making this list can be optimized
        int[] probabilities = new int[spawns.Length];
        for (int i = 0; i < spawns.Length; i++)
            probabilities[i] = spawns[i].spawnWeighting;

        return spawns[JUtilsClass.getIndexByWeighting(probabilities)].minionType;
    }
}
