using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{

    public enum PopupType
    {
        Damage,
        Critical
    }

    [System.Serializable]
    public struct PopupData
    {
        public PopupType popupType;
        public int size;
        public Color textColor;
        public Vector3 moveVector;
        public float timeUntilStartDisappear;
        public float timeToDisappear;
        public bool dynamicScale;
        public float timeToGrow;
        public float timeToShrink;
        public int maxSize;

    }

    private static int sortingOrder;

    [SerializeField] private PopupData[] popupData;

    private TextMeshPro textMesh;

    private int size;
    private Color textColor;
    private Vector3 moveVector;
    private float timeUntilStartDisappear;
    private float timeToDisappear;
    private bool dynamicScale;
    //serialized so OnValidate can give a preview of what it looks like
    [SerializeField] private PopupType popupType;
    private float timeToGrow;
    private float timeToShrink;
    private int maxSize;

    private float disappearSpeed;
    private float growSpeed;
    private float shrinkSpeed;
    private float growTimer;
    public static DamagePopup Create(Vector3 position, string val, PopupType popupType = PopupType.Damage)
    {
        Transform damagePopupTransform = Instantiate(GameAssets.i.damagePopupPrefab, position, Quaternion.identity).transform;
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(val, popupType);

        return damagePopup;
    }

    public void Setup(string val, PopupType popupType)
    {
        textMesh = transform.GetComponent<TextMeshPro>();

        this.popupType = popupType;
        //Sets up data based on the PopupType
        foreach (PopupData data in popupData)
        {
            if (data.popupType == this.popupType)
            {
                size = data.size;
                textColor = data.textColor;
                moveVector = data.moveVector;
                timeUntilStartDisappear = data.timeUntilStartDisappear;
                timeToDisappear = data.timeToDisappear;
                dynamicScale = data.dynamicScale;
                timeToGrow = data.timeToGrow;
                timeToShrink = data.timeToShrink;
                maxSize = data.maxSize;
            }
        }

        textMesh.text = val;
        sortingOrder++;
        textMesh.sortingOrder = sortingOrder;
        //scales the disappear speed so that it reduces by the appropriate amount to set textColor.a to zero after 'timeToDisappear many seconds
        disappearSpeed = timeToDisappear * textColor.a;
        growSpeed = (maxSize - size) / timeToGrow;
        shrinkSpeed = (maxSize - size) / timeToShrink;
        growTimer = 0f;
        textMesh.color = textColor;
        textMesh.fontSize = size;
    }

    private void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        timeUntilStartDisappear = timeUntilStartDisappear - Time.deltaTime;

        //logic for dynamically scaling text
        if (dynamicScale)
        {
            growTimer = growTimer + Time.deltaTime;
            if (growTimer <= timeToGrow)
                textMesh.fontSize += growSpeed * Time.deltaTime;
            else
                textMesh.fontSize -= shrinkSpeed * Time.deltaTime;
        }

        //logic for disappearing text
        if (timeUntilStartDisappear <= 0)
        {
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
                Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        //Sets up data based on the PopupType
        foreach (PopupData data in popupData)
        {
            if (data.popupType == this.popupType)
            {
                textMesh.fontSize = data.size;
                textMesh.color = data.textColor;
            }
        }
    }
}