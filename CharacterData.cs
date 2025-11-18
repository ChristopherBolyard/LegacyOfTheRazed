using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MVS/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public FactionType faction;
    public BackgroundType background;

    // Appearance (LPC)
    public int skinIndex;
    public int hairIndex;
    public int eyeIndex;

    // Currency & Progress
    public int credits = 1000;
    public int totalXP;
    public int soulFeats = 0;

    // Reputation (MVS Vol. 2 p.89)
    public Dictionary<FactionType, int> factionRep = new Dictionary<FactionType, int>
    {
        { FactionType.StellarGuard, 0 },
        { FactionType.PyreClans, 0 },
        { FactionType.VoltSyndicate, 0 },
        { FactionType.ApexHunters, 0 },
        { FactionType.NexusTradeGuild, 0 },
        { FactionType.FrontierRebels, 0 }
    };

    // Learned Tomes & Skills
    public List<string, bool> learnedTomes = new Dictionary<string, bool>();
    public List<string> activeSkills = new List<string> { "Ember Strike" }; // starter

    public void LearnTome(string tomeID) => learnedTomes[tomeID] = true;
    public bool HasTome(string tomeID) => learnedTomes.ContainsKey(tomeID);
}
