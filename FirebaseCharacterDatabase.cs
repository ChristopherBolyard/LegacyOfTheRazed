using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Database;
using UnityEngine;

public class FirebaseCharacterDatabase : MonoBehaviour
{
    public static FirebaseCharacterDatabase Instance { get; private set; }
    private DatabaseReference dbReference;
    private bool isInitialized = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void Initialize()
    {
        if (isInitialized) return;

        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        while (!dependencyTask.IsCompleted) await Task.Yield();

        if (dependencyTask.Exception != null) return;

        if (dependencyTask.Result == DependencyStatus.Available)
        {
            dbReference = FirebaseDatabase.DefaultInstance.RootReference;
            isInitialized = true;
            Debug.Log("Firebase DB ready");
        }
    }

    public async Task<bool> SaveNewCharacterAsync(CharacterData data)
    {
        if (!isInitialized) await Task.Delay(100);
        try
        {
            data.InitializeDefaults();
            data.lastPlayed = DateTime.UtcNow.ToString("o");
            string json = JsonUtility.ToJson(data);
            string path = $"users/{AuthenticationManager.Instance.GetCurrentUserId()}/characters/{data.characterId}";
            await dbReference.Child(path).SetRawJsonValueAsync(json);
            return true;
        }
        catch { return false; }
    }

    public async Task<List<CharacterData>> LoadCharactersForUser(string userId)
    {
        var list = new List<CharacterData>();
        if (!isInitialized) return list;

        try
        {
            string path = $"users/{userId}/characters";
            var snapshot = await dbReference.Child(path).GetValueAsync();
            if (snapshot.Exists)
            {
                foreach (var child in snapshot.Children)
                {
                    var data = JsonUtility.FromJson<CharacterData>(child.GetRawJsonValue());
                    data.InitializeDefaults();
                    list.Add(data);
                }
            }
        }
        catch { }
        return list;
    }

    public async Task<bool> DeleteCharacterAsync(CharacterData character)
    {
        try
        {
            string path = $"users/{AuthenticationManager.Instance.GetCurrentUserId()}/characters/{character.characterId}";
            await dbReference.Child(path).RemoveValueAsync();
            return true;
        }
        catch { return false; }
    }

    public async Task<bool> UpdateCharacterProgressAsync(CharacterData character)
    {
        try
        {
            var updates = new Dictionary<string, object>
            {
                { "currentXP", character.currentXP },
                { "level", character.level },
                { "credits", character.credits },
                { "lastPlayed", DateTime.UtcNow.ToString("o") }
            };
            string path = $"users/{AuthenticationManager.Instance.GetCurrentUserId()}/characters/{character.characterId}";
            await dbReference.Child(path).UpdateChildrenAsync(updates);
            return true;
        }
        catch { return false; }
    }

    public void ClearCache() { }
}