using UnityEngine;
using UnityEngine.UI;

public class SimpleHPBar : MonoBehaviour
{
    public Image hpFillImage;

    // Call this whenever HP changes
    public void Start()
    {
        hpFillImage = GetComponent<Image>();
    }
    public float fillPercent = 1.0f; 
    void Update()
    {
        hpFillImage.fillAmount = Mathf.Clamp01(fillPercent);
    }
    public void SetHPPercent(float percent)
    {
        hpFillImage.fillAmount = Mathf.Clamp01(percent);
    }
}
