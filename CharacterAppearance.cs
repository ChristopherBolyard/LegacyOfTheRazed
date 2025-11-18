using UnityEngine;

[System.Serializable]
public class CharacterAppearance
{
    public string skinTone = "Light";
    public string hairStyle = "Short";
    public string hairColor = "Black";
    public string eyeColor = "Brown";

    // LPC sprite paths (Resources folder)
    public string bodySprite;
    public string hairSprite;
    public string eyesSprite;
    public string torsoSprite;
    public string armsSprite;
    public string legsSprite;
}