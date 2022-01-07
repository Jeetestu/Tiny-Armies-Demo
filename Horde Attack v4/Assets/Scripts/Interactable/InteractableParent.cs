using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableParent : MonoBehaviour
{
    List<MinionController> minionsInRange;
    public bool onlyPlayers;
    public List<int> validTeams;

    protected void Awake()
    {
        minionsInRange = new List<MinionController>();
    }


    public bool isHordeInRange(HordeControllerParent h)
    {
        foreach (MinionController m in minionsInRange)
            if (m.Controller == h)
                return true;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MinionController m = collision.GetComponent<MinionController>();
        if (m != null)
            if (m.Controller.isPlayer || !onlyPlayers)
                if (validTeams.Count == 0 || validTeams.Contains(m.Controller.team))
                    minionsInRange.Add(m);
        updateVisual(minionsInRange.Count > 0);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MinionController m = collision.GetComponent<MinionController>();
        if (m != null)
            if (minionsInRange.Contains(m))
                minionsInRange.Remove(m);
        updateVisual(minionsInRange.Count > 0);
    }

    protected virtual void updateVisual(bool active)
    {

    }

    public virtual void interact(HordeControllerPlayer horde)
    {

    }
}
