using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingSpikeTrap : MonoBehaviour
{
    public int damage;
    public bool armed = true;

    private Collider2D col;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        anim.SetBool("Raised", armed);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        MinionController m = collision.GetComponent<MinionController>();
        if (m != null && armed)
        {
            if (!m.isDead)
            {
                m.takeDamage(damage, null);
                armed = false;
                col.enabled = false;
                anim.SetBool("Raised", false);
            }

        }
    }

}
