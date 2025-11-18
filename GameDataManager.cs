using System.Threading.Tasks;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { get; private set; }
    public CharacterData currentCharacter;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);  // ← Fixed
        }
        else
        {
            Destroy(this.gameObject);  // ← Fixed
        }
    }

    public void LoadCharacter(CharacterData data)
    {
        currentCharacter = data;
        data?.InitializeDefaults();
        Debug.Log($"Loaded: {data?.characterName}");
    }

    public void SetActiveCharacter(CharacterData character)  // ← NEW
    {
        currentCharacter = character;
        Debug.Log($"[GameDataManager] Active character set: {character.characterName}");
    }

    public async Task SaveProgressAsync()
    {
        if (currentCharacter == null) return;
        bool success = await FirebaseCharacterDatabase.Instance.UpdateCharacterProgressAsync(currentCharacter);
        NotificationSystem.Show(success ? "Saved!" : "Failed!", success ? Color.green : Color.red);
    }

    public void SaveProgress() => _ = SaveProgressAsync();
}