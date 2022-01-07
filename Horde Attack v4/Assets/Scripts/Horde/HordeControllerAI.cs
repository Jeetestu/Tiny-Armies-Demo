using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HordeControllerAI : HordeControllerParent
{
    public enum HordeAIState
    {
        IDLE, COMBAT, MOVING
    }
    private float aggroDuration;
    private float lastAggroTime;
    private HordeAIState state;
    private HordeControllerParent targetHorde;

    public bool wandering;
    public float wanderDistance;
    private float nextWanderTime;

    [Header("AI Horde Info")]
    public AIRegion region;
    Queue<SO_Minion> minionSpawnQueue;
    float minMinionSpawnTime;
    float maxMinionSpawnTime;
    private float timeOfNextSpawn;
    private Vector2 spawnPoint;

    public static HordeControllerAI Create(Queue<SO_Minion> spawnQueue, AIRegion region, Vector2 initialMovePos, Vector2 spawnPoint,  float minMinionSpawnTime = 0.1f, float maxMinionSpawnTime = 0.4f)
    {
        HordeControllerAI newHorde = Instantiate(GameAssets.i.AIHordePrefab, initialMovePos, Quaternion.identity).GetComponent<HordeControllerAI>();
        newHorde.minionSpawnQueue = spawnQueue;
        newHorde.spawnPoint = spawnPoint;
        newHorde.region = region;
        newHorde.minMinionSpawnTime = minMinionSpawnTime;
        newHorde.maxMinionSpawnTime = maxMinionSpawnTime;
        return newHorde;
    }
    private void Awake()
    {
        base.Awake();
        isPlayer = false;
        if (minionSpawnQueue == null)
            minionSpawnQueue = new Queue<SO_Minion>();
        aggroDuration = 1f;
        state = HordeAIState.IDLE;

        timeOfNextSpawn = Random.Range(minMinionSpawnTime, maxMinionSpawnTime);
    }

    private void Update()
    {
        base.Update();

        //every 60 frames, checks to see if the aggroDuration has passed. If so, it cancels the movement command
        if (operationsCounter % 60 == 0)
        {
            //if enough time has passed since the last aggro, set state to idle
            if (state == HordeAIState.COMBAT)
            {
                //cancel aggro if too much time has passed since the last aggro
                if (Time.time - lastAggroTime > aggroDuration || targetHorde == null)
                {
                    state = HordeAIState.IDLE;
                    //if wandering, minions will go to random point in region, otherwise regroup at local centre
                    if (wandering)
                        transform.position = region.getRandomPointInRegion(getCentreOfHorde());
                    else
                        transform.position = getCentreOfHorde();
                }
                else
                {
                    transform.position = targetHorde.getCentreOfHorde();
                }

                foreach (MinionController m in minions)
                    m.movement.setMovementTarget(transform.position, false);
            }



            //if not in combat, will move towards the transform of the horde controller
            if (state != HordeAIState.COMBAT)
            {
                //if wandering is true, will continue counting time until the transform moves to another point in the region
                if (wandering)
                {
                    if (Time.time > nextWanderTime)
                    {
                        transform.position = region.getRandomPointInRegion(getCentreOfHorde());
                        nextWanderTime = Time.time +  (int)Random.Range(5f, 20f);
                        state = HordeAIState.MOVING;
                    }
                }

                //if the centre of the horde is far from the transform, will refresh the movement command
                if (Vector2.Distance(getCentreOfHorde(), transform.position) > 3f)
                    foreach (MinionController m in minions)
                        m.movement.setMovementTarget(transform.position, false);
            }

        }
        

        //spawn minions if some left in queue
        if (minionSpawnQueue.Count >0)
            if (Time.time > timeOfNextSpawn)
            {
                timeOfNextSpawn = Random.Range(minMinionSpawnTime, maxMinionSpawnTime) + Time.time;
                MinionController newMinion = MinionController.Create(spawnPoint, this, minionSpawnQueue.Dequeue());
                newMinion.movement.setMovementTarget(transform.position, false);
                newMinion.onDamaged += aggroHorde;
            }

    }

    private void aggroHorde (int amount, HordeControllerParent target)
    {
        if (target == null)
            return;
        targetHorde = target;
        lastAggroTime = Time.time;
        if (state != HordeAIState.COMBAT)
        {
            transform.position = targetHorde.getCentreOfHorde();
            state = HordeAIState.COMBAT;
            foreach (MinionController m in minions)
                m.movement.setMovementTarget(transform.position, false);
        }
    }

    public override void allMinionsDead()
    {
        Destroy(this.gameObject);
    }


}
