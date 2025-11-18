using UnityEngine;
using System.Collections.Generic;

public static class AbilityPathSystem
{
    // === TOME DATABASE ===
    private static readonly Dictionary<string, TomeDefinition> tomeDatabase = new()
    {
        // FIRE PATH
        { "fire_lv1", new TomeDefinition { id = "fire_lv1", name = "Ember Strike", path = AbilityPath.Elemental, element = AbilityElement.Fire, cost = 500, xp = 100, skill = "Ember Strike" } },
        { "fire_lv2", new TomeDefinition { id = "fire_lv2", name = "Flame Burst", path = AbilityPath.Elemental, element = AbilityElement.Fire, cost = 1200, xp = 300, skill = "Flame Burst", reqLevel = 5 } },

        // WATER PATH
        { "water_lv1", new TomeDefinition { id = "water_lv1", name = "Aqua Jet", path = AbilityPath.Elemental, element = AbilityElement.Water, cost = 500, xp = 100, skill = "Aqua Jet" } },

        // EARTH PATH
        { "earth_lv1", new TomeDefinition { id = "earth_lv1", name = "Stone Shield", path = AbilityPath.Elemental, element = AbilityElement.Earth, cost = 500, xp = 100, skill = "Stone Shield" } },

        // WIND PATH
        { "wind_lv1", new TomeDefinition { id = "wind_lv1", name = "Gust Slash", path = AbilityPath.Elemental, element = AbilityElement.Wind, cost = 500, xp = 100, skill = "Gust Slash" } },

        // EXOTIC PATH
        { "telekinesis_lv1", new TomeDefinition { id = "telekinesis_lv1", name = "Mind Lift", path = AbilityPath.Exotic, exotic = ExoticPower.Telekinesis, cost = 800, xp = 200, skill = "Mind Lift" } },

        // SOUL PATH
        { "soul_bind", new TomeDefinition { id = "soul_bind", name = "Soul Bind", path = AbilityPath.Soul, soul = SoulPower.SoulBind, cost = 1000, xp = 400, skill = "Soul Bind", factionReq = FactionType.FrontierRebels } },
        { "soul_drain", new TomeDefinition { id = "soul_drain", name = "Soul Drain", path = AbilityPath.Soul, soul = SoulPower.SoulDrain, cost = 1500, xp = 600, skill = "Soul Drain", reqLevel = 10 } },

        // FACTION LOCKED
        { "apex_hunter_contract", new TomeDefinition { id = "apex_hunter_contract", name = "Elite Hunt Contract", cost = 2000, xp = 500, factionReq = FactionType.ApexHunters, repBonus = 100 } }
    };

    // === LEARN TOME ===
    public static bool LearnBook(CharacterData data, string bookId)
    {
        if (!tomeDatabase.TryGetValue(bookId, out var tome))
        {
            NotificationSystem.Show("Tome not found!", Color.red);
            return false;
        }

        // Check level
        if (tome.reqLevel > 0 && data.level < tome.reqLevel)
        {
            NotificationSystem.Show($"Need level {tome.reqLevel}!", Color.red);
            return false;
        }

        // Check path
        if (tome.path != AbilityPath.None && data.primaryPath != tome.path)
        {
            NotificationSystem.Show("Wrong path!", Color.red);
            return false;
        }

        // Check element
        if (tome.element != AbilityElement.None && data.primaryElement != tome.element)
        {
            NotificationSystem.Show("Wrong element!", Color.red);
            return false;
        }

        // Check exotic
        if (tome.exotic != ExoticPower.None && data.exoticPower != tome.exotic)
        {
            NotificationSystem.Show("Wrong exotic power!", Color.red);
            return false;
        }

        // Check soul
        if (tome.soul != SoulPower.None && !data.unlockedSoulPowers.Contains(tome.soul))
        {
            NotificationSystem.Show("Soul power not unlocked!", Color.red);
            return false;
        }

        // Check faction
        if (tome.factionReq != FactionType.None && data.faction != tome.factionReq)
        {
            NotificationSystem.Show($"Requires {tome.factionReq} faction!", Color.red);
            return false;
        }

        // Check credits
        if (data.credits < tome.cost)
        {
            NotificationSystem.Show("Not enough credits!", Color.red);
            return false;
        }

        // Check already learned
        if (data.primaryAbility.unlockedSkills.Contains(tome.skill))
        {
            NotificationSystem.Show("Already learned!", Color.gray);
            return false;
        }

        // === SUCCESS ===
        data.credits -= tome.cost;
        data.currentXP += tome.xp;

        if (!string.IsNullOrEmpty(tome.skill))
            data.primaryAbility.unlockedSkills.Add(tome.skill);

        if (tome.repBonus > 0)
            data.factionRep[tome.factionReq] = data.factionRep.GetValueOrDefault(tome.factionReq) + tome.repBonus;

        NotificationSystem.Show($"Learned: {tome.name}!", Color.cyan);
        GameDataManager.Instance.SaveProgress();
        return true;
    }

    // === GET TOME INFO (for UI) ===
    public static TomeDefinition GetTome(string id)
    {
        tomeDatabase.TryGetValue(id, out var tome);
        return tome;
    }
}

// === TOME DATA ===
[System.Serializable]
public class TomeDefinition
{
    public string id;
    public string name;
    public AbilityPath path;
    public AbilityElement element;
    public ExoticPower exotic;
    public SoulPower soul;
    public FactionType factionReq;
    public int cost;
    public int xp;
    public int reqLevel;
    public int repBonus;
    public string skill;
}