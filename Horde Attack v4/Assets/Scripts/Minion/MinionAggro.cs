using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtils;
public class MinionAggro : MonoBehaviour
{
    private MinionController controller;
    private bool aggrod;
    public delegate void minionAggro(bool state);

    public minionAggro onAggroStateChange;

    public bool Aggrod { get => aggrod;}

    void Awake()
    {
        controller = GetComponent<MinionController>();
    }

    // Update is called once per frame
    public void checkIfAggro()
    {
        Collider2D[] minionsInAggroRange = Physics2D.OverlapCircleAll(transform.position, controller.aggroRange, 1 << LayerMask.NameToLayer("Minion"));
        List<MinionController> nearbyEnemyMinions = new List<MinionController>();

        MinionController temp;
        foreach (Collider2D col in minionsInAggroRange)
        {
            //makes sure that units on opposite sides of a wall don't aggro each other

            RaycastHit2D hit = Physics2D.Raycast(transform.position, (col.transform.position - transform.position).normalized, Vector2.Distance(transform.position, col.transform.position), layerMask: 1 << LayerMask.NameToLayer("Walls"));
            if (hit.collider == null)
            {
                temp = col.GetComponent<MinionController>();
                if (temp.Controller.team != this.controller.Controller.team)
                    nearbyEnemyMinions.Add(temp);
            }

        }

        //if valid target, sets valid target
        if (nearbyEnemyMinions.Count > 0)
            controller.attack.AttackTarget = JUtilsClass.getNearestMinionToPointFromList(this.transform.position, nearbyEnemyMinions);

        //if there's been a change in aggro state, run event
        if ((controller.attack.AttackTarget != null) != Aggrod)
        {
            aggrod = controller.attack.AttackTarget != null;
            onAggroStateChange?.Invoke(Aggrod);
        }
    }
}
