using System.Collections.Generic;
using UnityEngine;
using JUtils;
using UnityEngine.InputSystem;
public class HordeControllerPlayer : HordeControllerParent
{
    private float startingUseSpeed = 1f;
    private float currentUseSpeed = 1f;
    private float lastTimeUsed;
    // Update is called once per frame

    public Vector2 startingSpawnPos;
    public List<SO_Minion> startingSpawn;
    public Queue<SO_Minion> minionSpawnQueue;
    private float timeOfNextSpawn;
    public List<SO_Minion> minionBank;


    public int maxMinions = 25;
    private bool mouseMoving;
    public bool hordeRespawnEnabled = true;
    public bool hordeDestroyedInFinalBattle;

    public static HordeControllerPlayer Create(Vector2 rallyPos, Vector2 minionSpawnPos, int team, List<SO_Minion> startingSpawn = null)
    {
        HordeControllerPlayer newHorde = Instantiate(GameAssets.i.playerHordePrefab, rallyPos, Quaternion.identity).GetComponent<HordeControllerPlayer>();
        newHorde.setup(rallyPos, minionSpawnPos, team, startingSpawn);
        return newHorde;
    }

    public void setup(Vector2 rallyPos, Vector2 minionSpawnPos, int team, List<SO_Minion> startingSpawn = null)
    {
        transform.position = rallyPos;
        startingSpawnPos = minionSpawnPos;
        this.team = team;
        if (startingSpawn != null)
            minionSpawnQueue = new Queue<SO_Minion>(startingSpawn);
        else
            minionSpawnQueue = new Queue<SO_Minion>(this.startingSpawn);

    }

    private void Awake()
    {
        base.Awake();
        minionBank = new List<SO_Minion>();
        isPlayer = true;
    }

    public override void allMinionsDead()
    {
        if (hordeRespawnEnabled)
        {
            minionSpawnQueue = new Queue<SO_Minion>(startingSpawn);
            removeCoins(getCoins());
        }
        else
            hordeDestroyedInFinalBattle = true;
    }

    void Update()
    {
        base.Update();

        #region oldInputSystem
        /* old movement input code
        if (Input.GetMouseButton(1))
        {
            //transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            foreach (MinionController m in minions)
                m.movement.setMovementTarget(Camera.main.ScreenToWorldPoint(Input.mousePosition), true);

            
        }
        else if (Input.GetMouseButtonUp(1))
        {
            foreach (MinionController m in minions)
                m.movement.setMovementTarget(getCentreOfHorde(), false);
        }
        */
        //for testing, spawns a coin
        //if (Input.GetMouseButtonDown(0))
        //{
        //    CoinController.Create(Camera.main.ScreenToWorldPoint(Input.mousePosition), new Vector2(Random.Range(-5f, 5f), Random.Range(-5f, 5f)));
        //}



        /* OLD INTERACT CODE
        if (Input.GetKey(KeyCode.Space))
        {
            if (Time.time - lastTimeUsed > currentUseSpeed)
            {
                lastTimeUsed = Time.time;
                currentUseSpeed = Mathf.Max(0.1f, currentUseSpeed - 0.3f);
                Interact();
            }

        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            currentUseSpeed = startingUseSpeed;
            lastTimeUsed = 0f;
        }
        */
        #endregion
        

        if (mouseMoving)
        {
            foreach (MinionController m in minions)
                m.movement.setMovementTarget(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), true);
        }

        //spawn minions if some left in queue
        if (minionSpawnQueue.Count > 0)
            if (Time.time > timeOfNextSpawn)
            {
                timeOfNextSpawn = Time.time + 0.5f;
                MinionController newMinion = MinionController.Create(startingSpawnPos, this, minionSpawnQueue.Dequeue());
                newMinion.movement.setMovementTarget(transform.position, false);
                newMinion.ignoreForUnitBank = true;
            }

    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            if (Time.time - lastTimeUsed > currentUseSpeed)
            {
                lastTimeUsed = Time.time;
                currentUseSpeed = Mathf.Max(0.1f, currentUseSpeed - 0.3f);
                List<InteractableParent> nearbyCamps = GetNearbyInteractables();
                foreach (InteractableParent camp in nearbyCamps)
                    if (camp.isHordeInRange(this))
                    {
                        camp.interact(this);
                    }
            }
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            currentUseSpeed = startingUseSpeed;
            lastTimeUsed = 0f;
        }

    }

    public void MoveToPosition (InputAction.CallbackContext ctx)
    {
        if (ctx.phase == InputActionPhase.Performed)
        {
            mouseMoving = true;

        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            mouseMoving = false;
            foreach (MinionController m in minions)
                m.movement.setMovementTarget(getCentreOfHorde(), false);
        }
    }

    public void MoveDirection(InputAction.CallbackContext ctx)
    {
        Vector2 direction = ctx.ReadValue<Vector2>();
        direction.Scale(new Vector2(1000f, 1000f));
        Vector2 targetPosition =  direction;
        if (ctx.phase == InputActionPhase.Performed)
        {
            foreach (MinionController m in minions)
                //m.movement.setMovementTarget(Camera.main.ScreenToWorldPoint(Mouse.current.position), true);
                m.movement.setMovementTarget(targetPosition);
        }
        else if (ctx.phase == InputActionPhase.Canceled)
        {
            foreach (MinionController m in minions)
                m.movement.setMovementTarget(getCentreOfHorde(), false);
        }
    }


    private List<InteractableParent> GetNearbyInteractables()
    {
        List<InteractableParent> camps = new List<InteractableParent>();
        float radius = JUtilsClass.convertBoundsToRadius(getHordeBounds(), 5f);
        foreach (Collider2D col in JUtilsClass.getAllCollidersInRadius(getCentreOfHorde(), radius))
        {
            if (col.tag == "Interactable")
            {
                camps.Add(col.GetComponentInParent<InteractableParent>());
            }

        }
        return camps;
    }

}
