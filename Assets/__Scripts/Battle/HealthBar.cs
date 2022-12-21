using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class HealthBar : MonoBehaviour
{
    private Image image;

    public Color BarColor
    {
        get
        {
            var localScale = transform.localScale.x;
            if (localScale < 0.15f)
            {
                return new Color(193/255f, 45/255f, 45/255f);
            } else if (localScale < 0.5f)
            {
                return new Color(211/255f, 212/255f, 29/255f);
            }
            else
            {
                return new Color(98/255f, 178/255f, 61/255f);
            }
        }
    }

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    

    /// <summary>
    /// Update health bart from a normalized value.
    /// </summary>
    /// <param name="normalizedValue">Float value between 0.0-1.0</param>
    public void SetHP(float normalizedValue)
    {
        StartCoroutine(SetSmoothHP(normalizedValue));
    }

    public IEnumerator SetSmoothHP(float normalizedValue)
    {
        float currentScale = transform.transform.localScale.x;
        float updateQuantity = currentScale - normalizedValue;
        while (currentScale - normalizedValue > Mathf.Epsilon)
        {
            currentScale -= updateQuantity * Time.deltaTime;
            transform.transform.localScale = new Vector3(currentScale, 1.0f);
            image.color = BarColor;
            yield return null;
        }
        
        transform.transform.localScale = new Vector3(normalizedValue, 1.0f);
    }
}
