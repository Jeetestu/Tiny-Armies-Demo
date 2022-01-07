using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
    [Header("Camera Data")]
    public Vector3 finalBattleCameraPosition;
    public float finalBattleCameraSize;


    [Header("Timer Data")]
    public float startingTimeUntilFinalBattle = 600f;
    private float remainingTimeUntilFinalBattle;

    [Header("References")]
    public PlayerManager playerManager;
    public TMP_Text gameOverText;
    public Color[] teamColors;

    private UnitBank[] unitBanks;
    private bool finalBattleStarted;
    private List<HordeControllerPlayer> survivingPlayers;
    private int opsCounter = 0;
    private void Awake()
    {
        remainingTimeUntilFinalBattle = startingTimeUntilFinalBattle;
        unitBanks = FindObjectsOfType<UnitBank>();
        survivingPlayers = new List<HordeControllerPlayer>();
    }

    private void Update()
    {
        if (!finalBattleStarted)
        {
            remainingTimeUntilFinalBattle = remainingTimeUntilFinalBattle - Time.deltaTime;
            foreach (UnitBank b in unitBanks)
            {
                b.setText("Send units for final battle\n(" + Mathf.RoundToInt(remainingTimeUntilFinalBattle).ToString() + " seconds)");
            }
            if (remainingTimeUntilFinalBattle <= 0)
            {
                finalBattleStarted = true;
                setupFinalBattle();
                beginFinalBattle();
            }
        }
        if (finalBattleStarted)
        {
            opsCounter++;
            if (opsCounter >= 250)
            {
                //removes players who have died from the 'surviving players' list
                for (int i = survivingPlayers.Count - 1; i >= 0; i--)
                    if (survivingPlayers[i].hordeDestroyedInFinalBattle)
                        survivingPlayers.RemoveAt(i);

                //if only one person left in list, that is the winner
                if (survivingPlayers.Count == 1)
                    endGame(survivingPlayers[0].team);
                //if nobody left in list then it is a tie
                else if (survivingPlayers.Count == 0)
                    endGame(0);
            }
        }
    }

    //team 0 means a tie
    private void endGame (int winningTeam)
    {
        gameOverText.enabled = true;
        if (winningTeam == 0)
            gameOverText.text = "Game Over\n Game is a tie!";
        else
        {
            gameOverText.text = "The winner is\nPlayer " + winningTeam.ToString();
            gameOverText.color = teamColors[winningTeam];
        }
    }

    private void setupFinalBattle()
    {
        foreach (PlayerManager.PlayerData data in playerManager.players)
        {
            HordeControllerPlayer horde = data.horde;
            if (horde != null)
                if (horde.minionBank.Count > 0)
                    survivingPlayers.Add(horde);
        }

        playerManager.setupPlayersForFinalBattle();
        foreach (AIRegion region in FindObjectsOfType<AIRegion>())
        {
            region.enabled = false;
        }
        foreach (HordeControllerAI horde in FindObjectsOfType<HordeControllerAI>())
        {
            horde.removeAllMinions();
        }
        Camera.main.transform.position = finalBattleCameraPosition;
        Camera.main.orthographicSize = finalBattleCameraSize;
    }
    private void beginFinalBattle()
    {
        playerManager.beginFinalBattleSpawning();
    }


}
