using UnityEngine;
using UnityEngine.UI;

public class LivePreview : MonoBehaviour
{
    [Header("Images (assign in inspector)")]
    public Image bodyImg;
    public Image hairImg;
    public Image eyesImg;
    public Image clothesImg;

    [Header("Sprites (drag folders)")]
    public Sprite[] skins;
    public Sprite[] hairs;
    public Sprite[] eyes;
    public Sprite[] clothes;

    private CharacterData data => GameDataManager.Instance.currentCharacter;

    void OnEnable() => Refresh();

    public void Refresh()
    {
        if (data == null) return;

        bodyImg.sprite = skins[data.skinIndex % skins.Length];
        hairImg.sprite = hairs[data.hairIndex % hairs.Length];
        eyesImg.sprite = eyes % eyes.Length;
        clothesImg.sprite = clothes[(int)data.faction];
    }

    public void SetSkin(int index) { data.skinIndex = index; Refresh(); }
    public void SetHair(int index) { data.hairIndex = index; Refresh(); }
    public void SetEyes(int index) { data.eyeIndex = index; Refresh(); }
}