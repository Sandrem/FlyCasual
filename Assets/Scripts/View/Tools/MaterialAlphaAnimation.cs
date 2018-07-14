using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialAlphaAnimation : MonoBehaviour
{
    private static readonly float alphaLowest = 0f;
    private static readonly float alphaHighest = 100f/256f;
    private static readonly float animationTime = 1f;

    private static int animationMode = 0;

    private Material material;
    private Color originalColor;

    private float animationMultiplier;

    public void OnEnable()
    {
        material = GetComponentInChildren<Renderer>().material;

        originalColor = material.color;
        material.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        animationMultiplier = (alphaHighest - alphaLowest) / (animationTime / 2);

        animationMode = 1;
    }

    public void Update()
    {
        switch (animationMode)
        {
            case 1:
                AlphaIncrease();
                break;
            case -1:
                AlphaDecrease();
                break;
            case 0:
                // No animation
                break;
        }
    }

    private void AlphaIncrease()
    {
        float currentTransparency = material.color.a;
        currentTransparency = Mathf.Min(currentTransparency + (animationMultiplier * Time.deltaTime), alphaHighest);

        material.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentTransparency);

        if (currentTransparency == alphaHighest) animationMode = -1;
    }

    private void AlphaDecrease()
    {
        float currentTransparency = material.color.a;
        currentTransparency = Mathf.Max(currentTransparency - (animationMultiplier * Time.deltaTime), alphaLowest);

        material.color = new Color(originalColor.r, originalColor.g, originalColor.b, currentTransparency);

        if (currentTransparency == alphaLowest)
        {
            gameObject.SetActive(false);
            animationMode = 0;
        }
    }
}
