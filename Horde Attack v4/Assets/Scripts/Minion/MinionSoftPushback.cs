using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSoftPushback : MonoBehaviour
{
    private MinionController controller;
    //used to prevent jittery movement as a result of soft pushback
    private Vector2 softPushbackFromLastFrame;
    private float maxSoftPushback;
    private void Awake()
    {
        softPushbackFromLastFrame = Vector2.zero;
        controller = GetComponent<MinionController>();
    }

    private void Start()
    {
        maxSoftPushback = controller.moveSpeed * 1.8f;
    }

    // Update is called once per frame
    public void calculateSoftPushbackThisFrame(float scale = 1f)
    {
        //apply a pushback from overlapping members
        foreach (Collider2D collision in controller.overlaps)
            if (collision != null && collision.tag == "Minion")
            {
                softPushbackFromLastFrame = Vector2.Lerp(softPushbackFromLastFrame, getSoftPushbackVector(collision) * scale, 0.3f);
                controller.movement.pushThisFrame += softPushbackFromLastFrame;
                //OLD CODE THAT DIDN'T MAX INCREASE IN SOFTPUSHBACK
                //controller.movement.pushThisFrame += getSoftPushbackVector(collision) * scale;
            }

    }

    // gets moveSpeed given overlap with another collider, where the greater the overlap, the stronger the pushback
    private Vector2 getSoftPushbackVector(Collider2D collision)
    {

        Vector2 directionOfOther = (transform.position - collision.gameObject.transform.position).normalized;
        //Expressed as a % from 1 to 100, where 1 means 1% of the way to centre, and 100% means their collider overlaps with our centre
        float distanceOfOther = 1 - (Vector2.Distance(collision.ClosestPoint(transform.position), transform.position) / (controller.minionCollider.bounds.extents.x));
        if (float.IsNaN(distanceOfOther))
            return Vector2.zero;
        return (directionOfOther * Mathf.Max(0.1f, distanceOfOther) * maxSoftPushback);
    }
}
