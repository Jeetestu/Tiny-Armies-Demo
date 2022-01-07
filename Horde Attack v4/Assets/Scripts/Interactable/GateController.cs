using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    public Collider2D col;
    public SpriteRenderer rend;
    public Sprite openDoorSprite;
    public Sprite closeDoorSprite;

    public void toggleCollider(bool closed)
    {
        col.enabled = closed;
    }

    public void toggleGate(bool closed, bool alsoToggleCollider = true)
    {
        if (closed)
            rend.sprite = closeDoorSprite;
        else
            rend.sprite = openDoorSprite;

        if (alsoToggleCollider)
            toggleCollider(closed);
    }
}
