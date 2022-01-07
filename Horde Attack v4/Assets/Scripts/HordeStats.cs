using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class HordeStats : MonoBehaviour
{
    public TMP_Text hordeName;
    public TMP_Text soulCount;
    public TMP_Text minionCount;
    public HordeControllerPlayer horde;

    private void Awake()
    {
        if (horde == null)
            this.gameObject.SetActive(false);
        else
            setupHordeStats(horde);
    }



    public void setupHordeStats(HordeControllerPlayer player)
    {
        this.gameObject.SetActive(true);
        horde = player;
        updateMinionCount();
        updateMinionCount();

        horde.onGainCoins += updateMinionCount;
        horde.onRemoveCoins += updateMinionCount;
        horde.onMinionRemoved += updateMinionCount;
        horde.onMinionAdded += updateMinionCount;
    }

    private void updateMinionCount(int change = 0, int newAmount = 0)
    {
        soulCount.text = horde.getCoins().ToString();
    }

    private void updateMinionCount()
    {
        minionCount.text = horde.minions.Count.ToString() + " / " + horde.maxMinions.ToString();
    }
}
