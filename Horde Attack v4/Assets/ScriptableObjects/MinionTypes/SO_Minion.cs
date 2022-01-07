using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Minion", menuName = "Minion")]
public class SO_Minion : ScriptableObject
{
    [Header("Stats")]
    public int health;
    public int moveSpeed;
    public int damage;
    public float attackTime;
    public float reloadTime;
    public float attackRange;
    public float attackPushback;
    public float aggroRange;
    public int coins;

    [Header("Collider")]
    public Vector2 offset;
    public float radius;
    public float pushbackRadius;

    [Header("Weapon")]
    public Sprite weaponSprite;
    public Vector3 weaponPivotOffset;

    [Header("Sounds")]
    public AudioClip deathSound;
    public AudioClip hitSound;


    public AnimatorOverrideController animationController;
}
