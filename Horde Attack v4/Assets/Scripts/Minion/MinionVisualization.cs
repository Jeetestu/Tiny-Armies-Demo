using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MinionVisualization : MonoBehaviour
{
    private MinionController controller;
    private MinionAggro aggro;

    private SortingGroup sortingGroup;
    private Animator minionAnim;
    private SpriteRenderer rend;
    private bool flippedLastFrame;
    private bool playingDamagedTint;

    [HideInInspector] public bool hasWeapon;
    public Vector3 weaponPivotOffset;
    public Vector3 currentWeaponTargetOffset;

    public Animator weaponAnim;
    public Transform weaponTransform;
    public SpriteRenderer weaponRend;

    private void Awake()
    {

        minionAnim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        controller = GetComponent<MinionController>();
        //setup event subscriptions
        controller.onDeath += deathAnimation;
        controller.onDamaged += damagedAnimation;
        controller.onAttackStart += attackAnimation;
        sortingGroup = GetComponent<SortingGroup>();
    }

    private void Start()
    {
        //setup outline color
        setOutlineColor(controller.Controller.hordeOutlineColor);
        aggro = controller.aggro;
        aggro.onAggroStateChange += aggroAnimation;
        //sets up the animation length based on the minion's stats
        if (hasWeapon)
        {
            weaponAnim.SetFloat("AttackTimeScale", 1f / controller.attackTime);
            weaponAnim.SetFloat("ReloadTimeScale", 1f / controller.reloadTime);
            currentWeaponTargetOffset = weaponTransform.localPosition;
        }
        minionAnim.SetFloat("AttackTimeScale", 1f / controller.attackTime + 1f / controller.reloadTime);


    }

    public void deathAnimation(MinionController minion)
    {
        if (hasWeapon)
            Destroy(weaponAnim.gameObject);
        sortingGroup.sortingLayerName = "Dead Bodies";
        minionAnim.SetTrigger("Death");
        rend.material.SetFloat("_OutlineThickness", 0f);
        SoundManager.PlaySound(controller.minionData.deathSound, transform.position, 0.5f);
    }

    public void damagedAnimation(int amount, HordeControllerParent attacker = null)
    {
        //create damage popup
        DamagePopup.Create(transform.position, amount.ToString(), DamagePopup.PopupType.Damage);
        rend.color = GameAssets.i.damagedColor;
        playingDamagedTint = true;
        SoundManager.PlaySound(controller.minionData.hitSound, transform.position, 0.5f);
    }

    private void Update()
    {
        if (playingDamagedTint)
        {
            rend.color = Vector4.MoveTowards(rend.color, Color.white, 3f * Time.deltaTime);
            if (rend.color == Color.white)
                playingDamagedTint = false;
        }
        if (hasWeapon && !controller.isDead)
            if (weaponTransform.localPosition != currentWeaponTargetOffset)
                weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, currentWeaponTargetOffset, 1f * Time.deltaTime);
        if (controller.isDead)
        {
            if (rend.color != GameAssets.i.damagedColor)
                rend.color = Vector4.MoveTowards(rend.color, GameAssets.i.damagedColor, 3f * Time.deltaTime);
        }

    }

    public void attackAnimation(Transform target)
    {
        //faces towards target
        if (target.transform.position.x < transform.position.x)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        minionAnim.SetTrigger("Attack");
        if (hasWeapon)
            weaponAnim.SetTrigger("Attack");

    }

    public void aggroAnimation(bool isAggro)
    {
        if (hasWeapon)
        {
            weaponAnim.SetBool("Aggrod", isAggro);
            if (isAggro)
                currentWeaponTargetOffset = weaponPivotOffset;
            else
                currentWeaponTargetOffset = Vector3.zero;
        }

               
    }

    public void walkAnimation(Vector2 movement)
    {
        if (!flippedLastFrame && minionAnim.GetBool("Walking"))
        {
            if (movement.x < 0f && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

                flippedLastFrame = true;
            }
            else if (movement.x > 0f && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                flippedLastFrame = true;
            }

        }
        else
            flippedLastFrame = false;


        minionAnim.SetBool("Walking", movement.magnitude > 0.1f);
    }

    public void setOutlineColor(Color c)
    {
        rend.material.SetColor("_OutlineColor", c);
    }
}
