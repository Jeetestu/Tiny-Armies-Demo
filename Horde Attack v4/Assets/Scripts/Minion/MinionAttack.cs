using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAttack : MonoBehaviour
{
    public enum AttackState
    {
        READY,ATTACKING,RELOADING
    }
    MinionController controller;
    private float lastAttackTime;
    public AttackState state;
    private MinionController attackTarget;
    private Collider2D attackTargetCollider;
    private Collider2D col;

    public MinionController AttackTarget
    {
        get => attackTarget;
        set
        {
            attackTarget = value;
            if (value != null)
                attackTargetCollider = attackTarget.GetComponent<Collider2D>();
            else
                attackTargetCollider = null;

        }
    }

    void Awake()
    {
        col = GetComponent<Collider2D>();
        controller = GetComponent<MinionController>();
        state = AttackState.READY;
    }



    public void attemptAttack()
    {
        if (Time.time - lastAttackTime > controller.attackTime)
        {
            if (col.Distance(attackTargetCollider).distance < controller.attackRange)
            {
                state = AttackState.ATTACKING;
                lastAttackTime = Time.time;
                controller.onAttackStart?.Invoke(attackTarget.transform);
            }
            else
                controller.aggro.checkIfAggro();
        }
    }

    public void attackHit()
    {

        if (attackTarget != null)
        {
            if (attackTarget.isDead)
                AttackTarget = null;
            else
            {
                attackTarget.movement.pushThisFrame += (Vector2)(attackTarget.transform.position - transform.position).normalized * controller.attackPushback;
                attackTarget.takeDamage(controller.damage, controller);
            }
        }
        state = AttackState.RELOADING;
    }

    public void attackComplete()
    {
        state = AttackState.READY;
        controller.aggro.checkIfAggro();
        if (attackTarget == null)
        {
            controller.movement.setMovementTarget(controller.Controller.getCentreOfHorde());
        }

    }
}
