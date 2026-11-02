using UnityEngine;
using TMPro;

public class AmmoHUD : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI ammoText;

    public void UpdateAmmo(int currentAmmo)
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentAmmo}";
        }
    }
}