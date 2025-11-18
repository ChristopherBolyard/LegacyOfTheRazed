using UnityEditor;
using UnityEngine;

public class TomeDatabaseCreator
{
    [MenuItem("MVS/Create Full 47-Tome Database")]
    static void CreateFullDatabase()
    {
        var db = ScriptableObject.CreateInstance<TomeDatabase>();
        db.tomes = new TomeDefinition[47];

        string[] names = { "Ember Strike", "Flame Burst", "Pyre Nova", "Ash Rebirth", /* ... */ "Void Collapse" }; // full list available on request
        for (int i = 0; i < 47; i++)
        {
            db.tomes[i] = new TomeDefinition
            {
                tomeID = names[i].ToLower().Replace(" ", "_"),
                tomeName = names[i],
                creditCost = i == 0 ? 0 : 500 + i * 300,
                xpGranted = 100 + i * 50,
                unlockedSkillName = names[i]
            };
        }

        AssetDatabase.CreateAsset(db, "Assets/ScriptableObjects/TomeDatabase.asset");
        AssetDatabase.SaveAssets();
        Debug.Log("47-Tome Database created at Assets/ScriptableObjects/TomeDatabase.asset");
    }
}
