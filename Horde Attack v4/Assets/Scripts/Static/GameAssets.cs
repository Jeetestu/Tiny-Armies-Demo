using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    private static GameAssets _i;

    public static GameAssets i
    {
        get
        {
            if (_i == null) _i = (Instantiate(Resources.Load("GameAssets")) as GameObject).GetComponent<GameAssets>();
            return _i;
        }
    }
    public GameObject basicCoinPrefab;
    public GameObject coinPrefab;
    public GameObject AIHordePrefab;
    public GameObject playerHordePrefab;
    public GameObject damagePopupPrefab;
    public GameObject soulPrefab;
    public GameObject minionPrefab;
    public Color damagedColor;

}
