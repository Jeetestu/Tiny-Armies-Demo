using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtils;

public class MinionController : MonoBehaviour
{


    public int health;
    public int moveSpeed;
    public int damage;
    public float attackTime;
    public float reloadTime;
    public float attackRange;
    public float attackPushback;
    public float aggroRange;
    public bool isDead;
    private Vector2 thisFrameMovement;

    public GameObject weapon;

    public Transform targetMoveTransform;

    private HordeControllerParent controller;

    public SO_Minion minionData;
    //special flag to prevent the initial spawn units from being deposited into the bank
    public bool ignoreForUnitBank;


    //subcomponents
    [HideInInspector] public MinionVisualization vis;
    [HideInInspector] public MinionMovement movement;
    [HideInInspector] public MinionSoftPushback softPushback;
    [HideInInspector] public MinionAggro aggro;
    [HideInInspector] public MinionAttack attack;



    private Rigidbody2D rb;
    public Collider2D minionCollider;
    public CircleCollider2D pushbackCollider;
    public List<Collider2D> overlaps;
    private int aggroCounter;
    private int coins;

    public HordeControllerParent Controller { get => controller; set => controller = value; }
    public Vector2 ThisFrameMovement { get => thisFrameMovement; set => thisFrameMovement = value; }



    //Events
    public delegate void MinionEvent();
    public delegate void MinionAttackEvent(Transform target);

    public MinionAttackEvent onAttackStart;

    public delegate void minionDeath(MinionController minion);

    public minionDeath onDeath;

    public delegate void DamageEvent(int amount, HordeControllerParent attacker);
    public DamageEvent onDamaged;

    public static MinionController Create(Vector2 spawnPos, HordeControllerParent horde, SO_Minion minionData)
    {
        MinionController newMinion = Instantiate(GameAssets.i.minionPrefab, spawnPos, Quaternion.identity).GetComponent<MinionController>();
        newMinion.loadData(minionData);
        newMinion.Controller = horde;
        horde.addMinion(newMinion);
        return newMinion;

    }

    private void loadData(SO_Minion minionData)
    {
        this.minionData = minionData;
        aggroCounter = Random.Range(1, 50);
        health = minionData.health;
        moveSpeed = minionData.moveSpeed;
        damage = minionData.damage;
        attackTime = minionData.attackTime;
        reloadTime = minionData.reloadTime;
        attackRange = minionData.attackRange;
        attackPushback = minionData.attackPushback;
        aggroRange = minionData.aggroRange;
        coins = minionData.coins;
        GetComponent<Animator>().runtimeAnimatorController = minionData.animationController;
        GetComponent<CircleCollider2D>().radius = minionData.radius;
        GetComponent<CircleCollider2D>().offset = minionData.offset;


        rb = GetComponent<Rigidbody2D>();
        minionCollider = GetComponent<Collider2D>();
        pushbackCollider.radius = minionData.pushbackRadius;
        

        overlaps = new List<Collider2D>();
        //ranges are set from the edge of the collider
        aggroRange += minionCollider.bounds.size.x / 2f;

        //setup references to sub-components
        vis = GetComponent<MinionVisualization>();
        movement = GetComponent<MinionMovement>();
        softPushback = GetComponent<MinionSoftPushback>();
        attack = GetComponent<MinionAttack>();
        aggro = GetComponent<MinionAggro>();

        //setup weapon
        if (minionData.weaponSprite == null)
            Destroy(weapon);
        else
        {
            weapon.GetComponent<SpriteRenderer>().sprite = minionData.weaponSprite;
            vis.hasWeapon = true;
            vis.weaponPivotOffset = minionData.weaponPivotOffset;
        }
    }

    public void takeDamage(int amount, MinionController attacker)
    {
        health = health - amount;
        if (attacker == null)
            onDamaged?.Invoke(amount, null);
        else
            onDamaged?.Invoke(amount, attacker.Controller);
        if (health <= 0)
            die();
    }

    private void die()
    {
        Destroy(minionCollider);
        isDead = true;
        if (!Controller.isPlayer)
            for (int i = 0; i < coins; i++)
                CoinController.Create(transform.position, new Vector2(Random.Range(-7f, 7f), Random.Range(-7f, 7f)));
        onDeath?.Invoke(this);
    }


    private void Update()
    {
        //if dead, don't do anything
        if (isDead)
            return;

        //refreshes target info in case target died
        if (attack.AttackTarget != null)
            if (attack.AttackTarget.isDead)
            {
                attack.AttackTarget = null;
                aggro.checkIfAggro();
            }

        aggroCounter++;
        //if not attacking, but it's been a while, run an aggro check
        if (attack.AttackTarget == null && aggroCounter >= 50)
        {
            aggroCounter = 0;
            aggro.checkIfAggro();
        }


        //if there's a potential target, attempt an attack
        if (attack.AttackTarget != null && attack.state == MinionAttack.AttackState.READY)
            attack.attemptAttack();
    }

    private void FixedUpdate()
    {
        if (!isDead)
        {
            if (attack.state != MinionAttack.AttackState.ATTACKING)
                movement.processMovement();

            movement.applyMovementAndPushThisFrame();

        }

    }


    //maintains a list of overlapping colliders
    private void OnTriggerEnter2D(Collider2D collision)
    {
        overlaps.Add(collision);
    } 

    private void OnTriggerExit2D(Collider2D collision)
    {
        overlaps.Remove(collision);
    }

}
