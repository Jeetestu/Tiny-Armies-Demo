using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MercenaryCamp : InteractableParent
{
    [Header("References")]
    public TMP_Text costText;
    public Image minionImage;
    public GateController gateController;
    public Transform spawnPos;
    [Header("Game Settings")]
    public int cost;
    public SO_Minion minionType;
    public Sprite minionSprite;


    private void Awake()
    {
        base.Awake();
        costText.text = cost.ToString();
        minionImage.sprite = minionSprite;
    }

    protected override void updateVisual(bool active)
    {
        gateController.toggleGate(!active, false);
    }

    public override void interact(HordeControllerPlayer horde)
    {
        if (horde.getCoins() >= cost && horde.minions.Count < horde.maxMinions)
        {
            horde.removeCoins(cost);
            spawnMinion(horde);
        }

    }

    private void spawnMinion(HordeControllerPlayer horde)
    {
        MinionController m = MinionController.Create(spawnPos.position, horde, minionType);
        m.movement.setMovementTarget(horde.getCentreOfHorde());
    }
}
