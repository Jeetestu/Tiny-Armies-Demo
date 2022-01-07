using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtils;
using TMPro;
public class UnitBank : InteractableParent
{
    public GateController gateController;
    public Transform despawnPos;
    public TMP_Text text;
    private List<MinionController> despawningUnits;
    protected override void updateVisual(bool active)
    {
        gateController.toggleGate(!active, false);
    }

    public override void interact(HordeControllerPlayer horde)
    {
        List<MinionController> validMinions = new List<MinionController>(horde.minions);


        //remove any minions that were part of initial spawn (they cannot be deposited)
        for (int i = validMinions.Count - 1; i >= 0; i--)
            if (validMinions[i].ignoreForUnitBank)
                validMinions.RemoveAt(i);

        //will not deposit final unit
        if (validMinions.Count <= 1)
            return;

        MinionController minionToDeposit = JUtilsClass.getNearestMinionToPointFromList(despawnPos.position, validMinions);

        horde.removeMinion(minionToDeposit);

        foreach (Transform trans in minionToDeposit.GetComponentsInChildren<Transform>())
            trans.gameObject.layer = LayerMask.NameToLayer("Ignore");

        minionToDeposit.movement.setMovementTarget(despawnPos.position);

        horde.minionBank.Add(minionToDeposit.minionData);

        StartCoroutine(despawnMinion(minionToDeposit));

    }

    private IEnumerator despawnMinion(MinionController m)
    {
        SpriteRenderer rend = m.GetComponent<SpriteRenderer>();
        Color c = rend.color;
        
        while (c.a > 0.1f)
        {
            c.a = Mathf.Lerp(c.a, 0f, 1f * Time.deltaTime);
            rend.color = c;
            yield return new WaitForEndOfFrame();
        }

        Destroy(m.gameObject);
    }

    public void setTextTemporarily (string newText, float duration)
    {
        StartCoroutine(setTextTemp(newText, duration));
    }

    public void setText (string newText)
    {
        text.text = newText;
    }

    private IEnumerator setTextTemp (string newText, float duration)
    {
        string oldText = text.text;
        text.text = newText;
        yield return new WaitForSeconds(duration);
        text.text = oldText;
    }
}
