using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TomeShopUI : MonoBehaviour
{
    public Transform contentParent;
    public GameObject tomeButtonPrefab;
    public TomeDatabase tomeDB;

    private List<GameObject> spawnedButtons = new List<GameObject>();

    private void OnEnable()
    {
        RefreshShop();
    }

    public void RefreshShop()
    {
        foreach (var btn in spawnedButtons) Destroy(btn);
        spawnedButtons.Clear();

        foreach (var tome in tomeDB.tomes)
        {
            var btnObj = Instantiate(tomeButtonPrefab, contentParent);
            var btn = btnObj.GetComponent<Button>();
            var text = btnObj.GetComponentInChildren<TMP_Text>();

            text.text = $"{tome.tomeName}\nCost: {tome.creditCost} credits";
            btn.onClick.AddListener(() => TryPurchase(tome));

            spawnedButtons.Add(btnObj);
        }
    }

    void TryPurchase(TomeDefinition tome)
    {
        // Simple version – expand with rep checks later
        if (GameDataManager.Instance.currentCharacter.credits >= tome.creditCost)
        {
            GameDataManager.Instance.currentCharacter.credits -= tome.creditCost;
            AbilityPathSystem.Instance.LearnTome(tome);
            NotificationSystem.Instance.Show($"Absorbed {tome.tomeName}!");
            RefreshShop();
        }
        else
        {
            NotificationSystem.Instance.Show("Not enough credits!");
        }
    }
}
