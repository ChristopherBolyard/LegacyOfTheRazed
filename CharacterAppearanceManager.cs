using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterAppearanceManager : MonoBehaviour
{
    [Header("Preview Images (assign in Inspector)")]
    public Image bodyImage;
    public Image hairImage;
    public Image eyesImage;
    public Image torsoImage;
    public Image armsImage;
    public Image legsImage;

    private CharacterAppearance appearance = new();

    // Call from UI dropdowns
    public void SetSkinTone(string tone) => appearance.skinTone = tone;
    public void SetHairStyle(string style) => appearance.hairStyle = style;
    public void SetHairColor(string color) => appearance.hairColor = color;
    public void SetEyeColor(string color) => appearance.eyeColor = color;

    // Call from CharacterCreationManager when confirming appearance
    public void ApplyToCharacter(CharacterData data)
    {
        data.skinTone = appearance.skinTone;
        data.hairStyle = appearance.hairStyle;
        data.hairColor = appearance.hairColor;
        data.eyeColor = appearance.eyeColor;

        // Build sprite paths (LPC format)
        appearance.bodySprite = $"Characters/body/body_human_male_{appearance.skinTone.ToLower()}";
        appearance.hairSprite = $"Characters/hair/hair_{appearance.hairStyle.ToLower()}_male_{appearance.hairColor.ToLower()}";
        appearance.eyesSprite = $"Characters/eyes/eyes_human_{appearance.eyeColor.ToLower()}";
        // torso/arms/legs can be added later
    }

    // Optional: live preview
    public void UpdatePreview()
    {
        if (bodyImage) bodyImage.sprite = Resources.Load<Sprite>(appearance.bodySprite);
        if (hairImage) hairImage.sprite = Resources.Load<Sprite>(appearance.hairSprite);
        if (eyesImage) eyesImage.sprite = Resources.Load<Sprite>(appearance.eyesSprite);
    }
}