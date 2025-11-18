using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterData
{
    // ────── BASIC INFO ──────
    public string characterId;
    public string characterName;
    public BackgroundType background;

    // ────── ABILITY PATH ──────
    public AbilityPath primaryPath = AbilityPath.None;
    public AbilityElement primaryElement = AbilityElement.None;
    public ExoticPower primaryExotic = ExoticPower.None;
    public SoulPower unlockedSoulFeat = SoulPower.None;
    public List<SoulPower> learnedSoulFeats = new();

    // ────── PROGRESS ──────
    public FactionType currentFaction = FactionType.None;
    public int level = 1;
    public int credits = 1000;
    public int currentXP = 0;                    // ← NEW
    public CharacterStats stats = new();         // ← NEW

    // ────── ABILITIES ──────
    public AbilityData primaryAbility;
    public List<AbilityData> learnedAbilities = new();

    // ────── APPEARANCE ──────
    public string skinTone;
    public string hairStyle;
    public string hairColor;
    public string eyeColor;

    // ────── INVENTORY & QUESTS ──────
    public List<ItemData> inventory = new();
    public List<string> equippedItems = new();
    public Dictionary<string, int> factionReputation = new();
    public List<string> activeQuests = new();
    public List<string> completedQuests = new();
    public string lastPlayed;  // ISO 8601 string

    public void InitializeDefaults()
    {
        inventory ??= new List<ItemData>();
        equippedItems ??= new List<string>();
        factionReputation ??= new Dictionary<string, int>();
        activeQuests ??= new List<string>();
        completedQuests ??= new List<string>();
        lastPlayed ??= DateTime.UtcNow.ToString("o");
        stats ??= new CharacterStats();
        currentXP = 0;
    }

    // Add these inside CharacterData class
    public int skinIndex = 0;
    public int hairIndex = 0;
    public int eyeIndex = 0;
    public Color hairColor = Color.white; // optional tint
}

[Serializable]
public class AbilityData
{
    public string abilityId;
    public string abilityName;
    public int level = 1;
    public List<string> unlockedSkills = new();
}

[Serializable]
public class ItemData
{
    public string itemId;
    public string itemName;
    public int quantity = 1;
}

// ────── STATS CLASS (NEW) ──────
[Serializable]
public class CharacterStats
{
    public int strength = 10;
    public int agility = 10;
    public int intelligence = 10;
    public int vitality = 10;
    public int energy = 100;
    public int maxEnergy = 100;
}
// In CharacterData.cs
public Dictionary<FactionType, int> factionRep = new Dictionary<FactionType, int>
{
    { FactionType.StellarGuard, 0 },
    { FactionType.PyreClans, 0 },
    { FactionType.VoltSyndicate, 0 },
    { FactionType.ApexHunters, 0 },
    { FactionType.NexusTradeGuild, 0 },
    { FactionType.FrontierRebels, 0 }
};
