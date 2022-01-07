using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtils;

public class Obelisk : MonoBehaviour
{
    private List<MinionController> membersInRange;
    private SpriteRenderer rend;

    public SO_Minion minionData;
    public int soulCostPerUse;

    private void Awake()
    {
        membersInRange = new List<MinionController>();
        rend = GetComponent<SpriteRenderer>();
        updateVisual();
    }
    public bool isMemberInRange(MinionController m)
    {
        return membersInRange.Contains(m);
    }

    public bool isHordeInRange(HordeControllerParent h)
    {
        foreach (MinionController m in membersInRange)
            if (m.Controller == h)
                return true;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        MinionController m = collision.GetComponent<MinionController>();
        if (m != null)
            membersInRange.Add(m);
        updateVisual();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        MinionController m = collision.GetComponent<MinionController>();
        if (m != null)
            membersInRange.Remove(m);
        updateVisual();
    }

    private void updateVisual()
    {
        if (membersInRange.Count > 0)
            rend.color = Color.white;
        else
            rend.color = Color.black;
    }

    public void spawnMinion (HordeControllerPlayer horde)
    {
        if (horde.getCoins() >= soulCostPerUse)
        {
            horde.removeCoins(soulCostPerUse);
            MinionController.Create(JUtilsClass.getRandomPointInBounds(horde.getHordeBounds()), horde, minionData);
        }

    }
}
