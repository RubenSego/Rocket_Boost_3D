using System.Collections.Generic;
using UnityEngine;

public class HealthHUD : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private GameObject eggPrefab;

    private List<EggUI> eggsList = new List<EggUI>();

    public void SetupHUD(int maxHealth)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        eggsList.Clear();

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject newEgg = Instantiate(eggPrefab, transform);
            EggUI eggUI = newEgg.GetComponent<EggUI>();
            if (eggUI != null)
            {
                eggsList.Add(eggUI);
            }
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        for (int i = 0; i < eggsList.Count; i++)
        {
            if (i >= currentHealth)
            {
                eggsList[i].BreakEgg();
            }
            else
            {
                eggsList[i].RestoreEgg();
            }
        }
    }
}