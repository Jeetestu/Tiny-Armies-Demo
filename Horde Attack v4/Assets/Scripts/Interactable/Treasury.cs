using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JUtils;
public class Treasury : InteractableParent
{
    public Transform spawnPos;
    public GateController gateController;
    public int coins = 0;
    public Collider2D coinDestinationArea;
    public int team;

    public void Awake()
    {
        base.Awake();
        validTeams = new List<int>();
        validTeams.Add(team);
    }

    protected override void updateVisual(bool active)
    {
        gateController.toggleGate(!active, false);
    }

    public override void interact(HordeControllerPlayer horde)
    {
        if (horde.getCoins() > 0)
        {
            horde.removeCoins(1);
            spawnCoin();
        }
    }

    private void spawnCoin()
    {
        BasicCoinController.Create(spawnPos.position, JUtilsClass.getRandomPointInBounds(coinDestinationArea.bounds));
        coins++;
    }
}
