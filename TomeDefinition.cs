// Assets/Scripts/Data/TomeDefinition.cs
using UnityEngine;

[CreateAssetMenu(menuName = "Legacy of the Razed/Tome Definition")]
public class TomeDefinition : ScriptableObject
{
    public string tomeID;
    public string tomeName;
    public Sprite icon;
    public AbilityPath requiredPath = AbilityPath.None;
    public AbilityElement requiredElement = AbilityElement.None;
    public ExoticPower requiredExotic = ExoticPower.None;
    public SoulPower unlockedSoulPower = SoulPower.None;
    public FactionType requiredFaction = FactionType.None;
    public int requiredFactionRep = 0;
    public int creditCost = 0;
    public int xpGranted = 0;
    public string unlockedSkillName = "";
    [TextArea] public string flavorText = "";
}