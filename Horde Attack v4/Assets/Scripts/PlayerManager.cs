using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerManager : MonoBehaviour
{

    [System.Serializable]
    public struct PlayerData
    {
        public HordeControllerPlayer horde;
        public HordeStats hordeStats;
        public Transform hordeSpawnPoint;
        public Transform hordeRallyPoint;
        public Transform finalBattleSpawnPoint;
        public Transform finalBattleRallyPoint;
    }
    public int playerCount = 0;

    public PlayerData[] players;

    public void PlayerJoined (PlayerInput p)
    {
        playerCount++;
        //players[playerCount].horde = HordeControllerPlayer.Create(players[playerCount].hordeRallyPoint.position, players[playerCount].hordeSpawnPoint.position, p, playerCount);
        players[playerCount].horde = p.GetComponent<HordeControllerPlayer>();
        players[playerCount].horde.setup(players[playerCount].hordeRallyPoint.position, players[playerCount].hordeSpawnPoint.position, playerCount);
        players[playerCount].hordeStats.setupHordeStats(players[playerCount].horde);
    }

    public void setupPlayersForFinalBattle()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].horde != null)
            {
                players[i].horde.removeAllMinions();
                players[i].horde.setup(players[i].finalBattleRallyPoint.position, players[i].finalBattleSpawnPoint.position, i, new List<SO_Minion>());
                players[i].hordeStats.gameObject.SetActive(false);
            }
        }
    }

    public void beginFinalBattleSpawning()
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].horde != null)
            {
                players[i].horde.minionSpawnQueue = new Queue<SO_Minion>(players[i].horde.minionBank);
                players[i].horde.hordeRespawnEnabled = false;
            }
        }
    }

}
