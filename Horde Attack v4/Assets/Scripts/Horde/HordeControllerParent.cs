using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtils;
public abstract class HordeControllerParent : MonoBehaviour
{
    public int team;
    public bool isPlayer;
    public List<MinionController> minions;
    protected int operationsCounter;

    protected Vector2 centreOfHorde;
    protected float lastCentreOfHordeCall;
    //public bool enableHordeOutline;
    public Color hordeOutlineColor;

    private int souls;

    public delegate void CoinEvent(int change, int newAmount);
    public CoinEvent onRemoveCoins;
    public CoinEvent onGainCoins;

    public delegate void hordeEvent();
    public hordeEvent onMinionRemoved;
    public hordeEvent onMinionAdded;


    public void removeCoins(int count)
    {
        souls = souls - count;
        onRemoveCoins?.Invoke(count, souls);
    }

    public void gainCoins (int count)
    {
        souls = souls + count;
        onGainCoins?.Invoke(count, souls);
    }

    public int getCoins()
    {
        return souls;
    }

    //uses a TIME approach to prevent redundent calls (will use a stored value if the function was called in the last 0.5 seconds)
    public Vector2 getCentreOfHorde()
    {

        if (Time.time - lastCentreOfHordeCall > 0.5)
        {

            centreOfHorde = JUtilsClass.getCentreOfMinions(minions);
            lastCentreOfHordeCall = Time.time;
        }
        return centreOfHorde;

    }

    protected void Awake()
    {

        minions = new List<MinionController>();


        centreOfHorde = JUtilsClass.getCentreOfMinions(minions);
    }

    protected void Update()
    {
        operationsCounter++;
        if (operationsCounter > 10000)
            operationsCounter = 0;
    }

    public void removeMinion(MinionController minion)
    {
        if (!containsMinion(minion))
            Debug.LogError("Cannot find minion from minion death event. Minion is: " + minion.gameObject.name);
        minions.Remove(minion);
        onMinionRemoved?.Invoke();
        if (minions.Count <= 0)
            allMinionsDead();
    }

    public void removeAllMinions()
    {
        for (int i = minions.Count - 1; i >= 0; i--)
        {
            MinionController m = minions[i];
            removeMinion(m);
            Destroy(m.gameObject);
        }
    }

    public virtual void allMinionsDead()
    {

    }

    public void addMinion(MinionController minion)
    {
        minions.Add(minion);
        minion.onDeath += removeMinion;
        minion.Controller = this;
        onMinionAdded?.Invoke();
    }

    public bool containsMinion(Transform check)
    {
        MinionController checkedMember = check.GetComponent<MinionController>();
        if (checkedMember == null)
            return false;
        else
            return containsMinion(checkedMember);
    }

    public bool containsMinion(MinionController check)
    {
        return minions.Contains(check);
    }

    public Bounds getHordeBounds(float buffer = 0f)
    {
        if (minions.Count == 0)
            return new Bounds(transform.position, Vector3.one);

        float xMin = Mathf.Infinity;
        float yMin = Mathf.Infinity;
        float xMax = Mathf.NegativeInfinity;
        float yMax = Mathf.NegativeInfinity;

        foreach (MinionController m in minions)
        {
            if (m.transform.position.x < xMin)
                xMin = m.transform.position.x;
            if (m.transform.position.x > xMax)
                xMax = m.transform.position.x;
            if (m.transform.position.y > yMax)
                yMax = m.transform.position.y;
            if (m.transform.position.y < yMin)
                yMin = m.transform.position.y;
        }


        Vector3 size = new Vector3 (xMax - xMin + buffer, yMax-yMin + buffer);
        Vector3 centre = new Vector3(xMin + size.x / 2f, yMin + size.y / 2f);
        return new Bounds(centre, size);
    }




}
