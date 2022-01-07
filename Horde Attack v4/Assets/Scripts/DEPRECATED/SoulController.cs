using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulController : MonoBehaviour
{
    private float startingLifespan;
    private float remainingLifespan;
    public bool collectable = false;
    private HordeControllerParent originHorde;
    private SpriteRenderer rend;
    private Color soulColor;
    public static SoulController Create(Vector2 spawnPos, HordeControllerParent originHorde = null, float lifeSpan = 10f)
    {
        Transform soulTransform = Instantiate(GameAssets.i.soulPrefab, spawnPos, Quaternion.identity).transform;
        SoulController soul = soulTransform.GetComponent<SoulController>();

        soul.originHorde = originHorde;
        soul.rend = soul.GetComponent<SpriteRenderer>();
        if (originHorde != null)
            soul.rend.color = originHorde.hordeOutlineColor;
        else
            soul.rend.color = Color.white;
        soul.startingLifespan = lifeSpan;
        soul.remainingLifespan = lifeSpan;
        soul.soulColor = soul.rend.color;
        return soul;
    }

    public void setCollectable()
    {
        collectable = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!collectable)
            return;
        MinionController minion = other.GetComponent<MinionController>();
        if (minion != null && !minion.isDead && minion.Controller != originHorde)
        {
            minion.Controller.gainCoins(1);
            Destroy(this.gameObject);
        }

    }

    private void Update()
    {
        if (!collectable)
            return;
        remainingLifespan = remainingLifespan - Time.deltaTime;
        if (remainingLifespan <= 0f)
            Destroy(this.gameObject);
        soulColor.a = remainingLifespan / startingLifespan;
        rend.color = soulColor;
    }


}
