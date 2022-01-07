using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtils;
public class MinionMovement : MonoBehaviour
{
    protected MinionController controller;
    protected Rigidbody2D rb;
    protected Vector2 movementThisFrame;
    public Vector2 pushThisFrame;

    public Vector2 movementTarget;

    //COMMANDMOVING will continue to move towards target even if it comes across enemies (will try to attack while in range)
    //ATTACKMOVING will start moving towards enemies who aggro
    public enum MovementState
    {
        COMMANDMOVING, ATTACKMOVING, IDLE
    }

    public MovementState state;

    private void Awake()
    {
        controller = GetComponent<MinionController>();
        rb = GetComponent<Rigidbody2D>();
        state = MovementState.IDLE;
    }

    public void setMovementTarget(Vector2 target, bool commandMoving = false)
    {
        movementTarget = target;
        if (commandMoving)
            state = MovementState.COMMANDMOVING;
        else
            state = MovementState.ATTACKMOVING;
    }

    public void processMovement()
    {
        //if idle or attack moving and there is a valid nearby target (according to the aggro check), move towards attack target
        if (state == MovementState.IDLE && controller.attack.AttackTarget != null)
            state = MovementState.ATTACKMOVING;

        if (state == MovementState.IDLE)
            return;

        //if attack moving and there's a valid target nearby. Move towards the target
        if (state == MovementState.ATTACKMOVING && controller.attack.AttackTarget != null)
            calculateMovementTowardsTarget(controller.attack.AttackTarget.transform.position);
        //if within distance of movementTarget, set state to IDLE (it's fine if there is a move order or an attack move being given
        //because it will reset the movementState)
        else if (Vector2.Distance(movementTarget, transform.position) < controller.minionCollider.bounds.extents.x * 2f)
            state = MovementState.IDLE;
        //otherwise move towards target as normal
        else
            calculateMovementTowardsTarget(movementTarget);

    
    }

    public void calculateMovementTowardsTarget(Vector2 target)
    {
        movementThisFrame = JUtilsClass.getDirection(transform.position, target) * controller.moveSpeed;

    }

    public virtual void applyMovementAndPushThisFrame()
    {
        rb.MovePosition(rb.position + (movementThisFrame + pushThisFrame) * Time.fixedDeltaTime);
        controller.vis.walkAnimation(movementThisFrame);
        movementThisFrame = Vector2.zero;
        pushThisFrame = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if collides with an ally who is idle, then become idle
        if (state == MovementState.ATTACKMOVING)
        {
            MinionController m = collision.GetComponent<MinionController>();
            if (m != null)
                if (m.movement.state == MovementState.IDLE && m.Controller == this.controller.Controller)
                {
                    state = MovementState.IDLE;
                    movementThisFrame = Vector2.zero;

                }

        }
    }
}
