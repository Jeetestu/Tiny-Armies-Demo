using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHelper : MonoBehaviour
{
    public MinionAttack attack;

    public void attackHit() { attack.attackHit(); }

    public void attackComplete() { attack.attackComplete(); }
}
