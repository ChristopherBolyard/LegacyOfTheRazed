using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Firebase.Auth;
using System.Threading.Tasks;
using System.Linq;
using System;

/// <summary>
/// Character Selection Manager - Displays and manages character slots
/// CLEAN VERSION - No duplicate methods
/// </summary>
public class CharacterSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class CharacterSlotUI
    {
        public GameObject slotRoot;
        public Image characterImage;
        public TextMeshProUGUI characterNameText;
        public TextMeshProUGUI characterLevelText;
        public TextMeshProUGUI characterBackgroundText;
        public Button createNewButton;
        public Button selectCharacterButton;
        public Button deleteCharacterButton;
    }

    [Header("Character Slots")]
    public CharacterSlotUI[] characterSlots = new CharacterSlotUI[5];

    [Header("UI Elements")]
    public GameObject loadingPanel;
    public TextMeshProUGUI loadingText;
    public Button backButton;

    [Header("Confirmation Dialog")]
    public GameObject deleteConfirmationPanel;
    public TextMeshProUGUI deleteConfirmationText;
    public Button confirmDeleteButton;
    public Button cancelDeleteButton;

    private List<CharacterData> loadedCharacters = new List<CharacterData>();
    private CharacterData characterToDelete = null;
    private string currentUserId;

    void Start()
    {
        SetupUI();
        LoadUserCharacters();
    }

    void SetupUI()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButton);
        }

        if (confirmDeleteButton != null)
        {
            confirmDeleteButton.onClick.AddListener(OnConfirmDelete);
        }

        if (cancelDeleteButton != null)
        {
            cancelDeleteButton.onClick.AddListener(OnCancelDelete);
        }

        // Initialize slots and assign listeners
        for (int i = 0; i < characterSlots.Length; i++)
        {
            var slot = characterSlots[i];

            // Clear existing listeners to prevent duplication
            slot.selectCharacterButton.onClick.RemoveAllListeners();
            slot.deleteCharacterButton.onClick.RemoveAllListeners();
            slot.createNewButton.onClick.RemoveAllListeners();

            // Add new listeners using a closure variable for the index
            int index = i;

            slot.selectCharacterButton.onClick.AddListener(() => OnSelectCharacter(index));
            slot.deleteCharacterButton.onClick.AddListener(() => OnDeleteCharacter(index));
            slot.createNewButton.onClick.AddListener(OnCreateNewCharacter);
        }

        // Hide confirmation dialog initially
        if (deleteConfirmationPanel != null)
            deleteConfirmationPanel.SetActive(false);
    }

    async void LoadUserCharacters()
    {
        currentUserId = FirebaseAuth.DefaultInstance.CurrentUser?.UserId;
        if (string.IsNullOrEmpty(currentUserId))
        {
            Debug.LogError("User not logged in or ID is null. Returning to Login.");
            SceneManager.LoadScene("01_LoginScene");
            return;
        }

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            loadingText.text = "Loading characters from the stars...";
        }

        if (FirebaseCharacterDatabase.Instance == null)
        {
            Debug.LogError("FirebaseCharacterDatabase instance is missing!");
            if (loadingPanel != null) loadingPanel.SetActive(false);
            return;
        }

        try
        {
            // Call the database to fetch characters
            var characters = await FirebaseCharacterDatabase.Instance.LoadCharactersForUser(currentUserId);
            DisplayCharacters();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load characters: {ex.Message}");
            loadingText.text = "Error loading characters. Check console for details.";
        }
        finally
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
        }
    }

    void DisplayCharacters()
    {
        int characterCount = loadedCharacters.Count;

        for (int i = 0; i < characterSlots.Length; i++)
        {
            var slot = characterSlots[i];

            if (i < characterCount)
            {
                // Display existing character data
                CharacterData character = loadedCharacters[i];

                slot.slotRoot.SetActive(true);
                slot.createNewButton.gameObject.SetActive(false);

                slot.characterNameText.text = character.characterName;
                slot.characterLevelText.text = $"Level {character.level}";
                slot.characterBackgroundText.text = character.background.ToString();

                slot.selectCharacterButton.gameObject.SetActive(true);
                slot.deleteCharacterButton.gameObject.SetActive(true);
            }
            else if (i == characterCount && characterCount < characterSlots.Length)
            {
                // Show the "Create New" button in the next available slot
                slot.slotRoot.SetActive(true);
                slot.createNewButton.gameObject.SetActive(true);

                // Clear or hide other elements in the create slot
                slot.characterNameText.text = "New Recruit";
                slot.characterLevelText.text = string.Empty;
                slot.characterBackgroundText.text = string.Empty;
                slot.selectCharacterButton.gameObject.SetActive(false);
                slot.deleteCharacterButton.gameObject.SetActive(false);
            }
            else
            {
                // Hide remaining unused slots
                slot.slotRoot.SetActive(false);
            }
        }
    }

    void OnSelectCharacter(int index)
    {
        if (index >= 0 && index < loadedCharacters.Count)
        {
            CharacterData selectedCharacter = loadedCharacters[index];

            // Set the active character in the persistent GameDataManager
            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.SetActiveCharacter(loadedCharacters[index]);
                Debug.Log($"Selected character: {selectedCharacter.characterName}. Loading GameScene.");

                // Load the main game scene
                SceneManager.LoadScene("03_GameScene");
            }
            else
            {
                Debug.LogError("GameDataManager is not initialized!");
            }
        }
        else
        {
            Debug.LogError($"Attempted to select character at invalid index: {index}");
        }
    }

    void OnCreateNewCharacter()
    {
        Debug.Log("Loading Character Creation Scene.");
        SceneManager.LoadScene("02_CharacterCreatorScene");
    }

    void OnDeleteCharacter(int index)
    {
        if (index >= 0 && index < loadedCharacters.Count)
        {
            characterToDelete = loadedCharacters[index];

            if (deleteConfirmationPanel != null)
            {
                deleteConfirmationText.text = $"Are you sure you want to delete {characterToDelete.characterName} (Lvl {characterToDelete.level})?\nThis action cannot be undone.";
                deleteConfirmationPanel.SetActive(true);
            }
        }
    }

    async void OnConfirmDelete()
    {
        if (characterToDelete == null) return;

        if (loadingPanel != null)
        {
            loadingPanel.SetActive(true);
            loadingText.text = $"Deleting {characterToDelete.characterName}...";
        }

        bool success = await FirebaseCharacterDatabase.Instance.DeleteCharacterAsync(characterToDelete);

        if (success)
        {
            Debug.Log($"Successfully deleted character: {characterToDelete.characterName}");
            // Clear the cache and reload the list to refresh the UI
            FirebaseCharacterDatabase.Instance.ClearCache();
            characterToDelete = null;
            LoadUserCharacters();
        }
        else
        {
            Debug.LogError("Failed to delete character!");
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
        }

        if (deleteConfirmationPanel != null)
            deleteConfirmationPanel.SetActive(false);
    }

    void OnCancelDelete()
    {
        characterToDelete = null;

        if (deleteConfirmationPanel != null)
            deleteConfirmationPanel.SetActive(false);
    }

    void OnBackButton()
    {
        // Return to login screen
        SceneManager.LoadScene("01_LoginScene");
    }
}

