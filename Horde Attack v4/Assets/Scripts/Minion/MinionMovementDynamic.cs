using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtils;
public class MinionMovementDynamic : MinionMovement
{
    public GameObject pushback;

    private void Awake()
    {
        controller = GetComponent<MinionController>();
        rb = GetComponent<Rigidbody2D>();
        state = MovementState.IDLE;
        controller.onDeath += disablePushback;
    }

    private void disablePushback(MinionController minion)
    {
        Destroy(pushback);
    }

    public override void applyMovementAndPushThisFrame()
    {
       // rb.MovePosition(rb.position + (movementThisFrame + pushThisFrame) * Time.fixedDeltaTime);
        rb.AddForce((movementThisFrame + pushThisFrame) * 10f, ForceMode2D.Force);
        controller.vis.walkAnimation(movementThisFrame);
        movementThisFrame = Vector2.zero;
        pushThisFrame = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if collides with an ally who is idle, then become idle
        if (state == MovementState.ATTACKMOVING)
        {
            MinionController m = collision.collider.GetComponent<MinionController>();
            if (m != null)
                if (m.movement.state == MovementState.IDLE && m.Controller == this.controller.Controller)
                {
                    state = MovementState.IDLE;
                    movementThisFrame = Vector2.zero;

                }

        }
    }

}
