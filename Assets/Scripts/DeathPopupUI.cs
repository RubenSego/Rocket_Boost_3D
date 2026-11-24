using UnityEngine;
using UnityEngine.UI;

public class DeathPopupUI : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private GameObject popupPanel;

    public void ShowPopup(bool hasCheckpoint)
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(true);
        }
    }

    public void HidePopup()
    {
        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }
}