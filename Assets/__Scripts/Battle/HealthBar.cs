using UnityEngine;

public class HealthBar : MonoBehaviour
{
    /// <summary>
    /// Update health bart from a normalized value.
    /// </summary>
    /// <param name="normalizedValue">Float value between 0.0-1.0</param>
    public void SetHP(float normalizedValue)
    {
        transform.transform.localScale = new Vector3(normalizedValue, 1.0f);
    }
}
