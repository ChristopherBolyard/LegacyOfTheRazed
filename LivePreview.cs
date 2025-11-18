using UnityEngine;
using UnityEngine.UI;

public class LivePreview : MonoBehaviour
{
    public Image body;
    public Image hair;
    public Image eyes;
    public Image outfit;

    public Sprite[] skinSprites;
    public Sprite[] hairSprites;
    public Sprite[] eyeSprites;
    public Sprite[] outfitSprites; // index = faction enum

    private CharacterData Data => GameDataManager.Instance.currentCharacter;

    private void OnEnable() => RefreshPreview();
    public void RefreshPreview()
    {
        if (Data == null) return;

        body.sprite = skinSprites[Data.skinIndex];
        hair.sprite = hairSprites[Data.hairIndex];
        eyes.sprite = eyeSprites[Data.eyeIndex];
        outfit.sprite = outfitSprites[(int)Data.faction];
    }

    // Called by dropdowns
    public void UpdateSkin(int i) { Data.skinIndex = i; RefreshPreview(); }
    public void UpdateHair(int i) { Data.hairIndex = i; RefreshPreview(); }
    public void UpdateEyes(int i) { Data.eyeIndex = i; RefreshPreview(); }
}
