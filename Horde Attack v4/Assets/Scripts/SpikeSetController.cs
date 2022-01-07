using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSetController : MonoBehaviour
{
    private Animator[] spikeTiles;
    private Collider2D col;
    public bool raised;

    private void Awake()
    {
        spikeTiles = GetComponentsInChildren<Animator>();
        col = GetComponent<Collider2D>();
        ToggleSpikes(true);
    }

    public void ToggleSpikes (bool val)
    {
        raised = val;
        col.enabled = val;
        foreach (Animator a in spikeTiles)
            a.SetBool("Raised", val);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            ToggleSpikes(!raised);
    }
}
