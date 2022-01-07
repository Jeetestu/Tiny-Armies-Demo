using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCoinController : MonoBehaviour
{
    private Vector2 destination;
    public static BasicCoinController Create(Vector2 origin, Vector2 destination, bool animated = true, float scale = 1f)
    {
        BasicCoinController coin = Instantiate(GameAssets.i.basicCoinPrefab, origin, Quaternion.identity).GetComponent<BasicCoinController>();
        coin.destination = destination;
        coin.transform.localScale = new Vector3(scale, scale, scale);
        if (!animated)
            coin.GetComponent<Animator>().enabled = false;

        return coin;
    }

    private void Update()
    {
        transform.position = Vector2.Lerp(transform.position, destination, 1f * Time.deltaTime);
        if (new Vector2 (transform.position.x, transform.position.y) == destination)
        {
            Destroy(this);
        }
    }
}
