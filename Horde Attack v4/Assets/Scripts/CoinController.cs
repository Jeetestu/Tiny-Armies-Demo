using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public SpriteRenderer rend;
    public Collider2D col;
    public Rigidbody2D rb;
    public AudioSource audio;
    private Color coinColor;

    private float fadeStartTime;
    private float startingLifespan;
    private float remainingLifespan;

    private bool stoppedMoving;
    private float maxSpeedForPickup;
    public static CoinController Create(Vector2 origin, Vector2 initialVelocity, float lifeSpan = 10f, float fadeStartTime = 7.5f)
    {
        CoinController newCoin = Instantiate(GameAssets.i.coinPrefab, origin, Quaternion.identity).GetComponent<CoinController>();

        newCoin.rb.velocity = initialVelocity;
        newCoin.startingLifespan = lifeSpan;
        newCoin.remainingLifespan = newCoin.startingLifespan;
        newCoin.coinColor = newCoin.rend.color;
        newCoin.maxSpeedForPickup = 0.5f;
        newCoin.fadeStartTime = lifeSpan - fadeStartTime;

        return newCoin;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (rb.velocity.magnitude > maxSpeedForPickup) return;

        checkForPickup(collision);

    }

    private void checkForPickup (Collider2D collision)
    {
        MinionController m = collision.GetComponent<MinionController>();
        if (m != null)
            if (m.Controller.isPlayer)
            {
                SoundManager.PlaySound(audio.clip, transform.position, 0.8f, 1.2f);
                m.Controller.gainCoins(1);
                Destroy(this.gameObject);
            }
    }

    //controls the fade and eventually deletion of the coin
    private void Update()
    {
        if (rb.velocity.magnitude > maxSpeedForPickup) return;

        if (!stoppedMoving)
        {
            stoppedMoving = true;
            Collider2D[] currentOverlaps = new Collider2D[20];
            col.OverlapCollider(new ContactFilter2D(),currentOverlaps);

            foreach (Collider2D c in currentOverlaps)
            {
                if (c != null)
                {
                    checkForPickup(c);
                }
            }
        }
        remainingLifespan = remainingLifespan - Time.deltaTime;
        if (remainingLifespan <= 0f)
            Destroy(this.gameObject);
        if (remainingLifespan <= fadeStartTime)
            coinColor.a = remainingLifespan / fadeStartTime;
        rend.color = coinColor;
    }


}
