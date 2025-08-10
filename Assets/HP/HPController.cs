using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviour
{
    [Range(0, 1)]
    public float fillAmount = 1f;
    public Material hPBarMaterial;
    void Start()
    {
        RawImage img = GetComponent<RawImage>();
        hPBarMaterial = Instantiate(img.material);
        img.material = hPBarMaterial;
    }
    void Update()
    {
        hPBarMaterial.SetFloat("_FillAmount", fillAmount);
    }

    public void SetHPPercent(float percent)
    {
        fillAmount = Mathf.Clamp01(percent);
    }
}
