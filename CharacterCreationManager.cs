using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class CharacterCreationManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject nameInputPanel;
    public GameObject backgroundSelectionPanel;
    public GameObject abilitySelectionPanel;
    public GameObject appearanceCustomizationPanel;
    public GameObject confirmPanel;

    [Header("Name Input")]
    public TMP_InputField nameInputField;
    public Button nameConfirmButton;

    [Header("Background")]
    public Transform backgroundButtonContainer;
    public GameObject backgroundButtonPrefab;
    public TMP_Text backgroundNameText;
    public TMP_Text backgroundDescriptionText;
    public Button backgroundConfirmButton;

    [Header("Ability Path")]
    public Transform abilityButtonContainer;
    public GameObject abilityButtonPrefab;
    public TMP_Text abilityNameText;
    public TMP_Text abilityDescriptionText;
    public Button abilityConfirmButton;

    [Header("Appearance")]
    public TMP_Dropdown skinToneDropdown;
    public TMP_Dropdown hairStyleDropdown;
    public TMP_Dropdown hairColorDropdown;
    public TMP_Dropdown eyeColorDropdown;
    public Button appearanceConfirmButton;

    [Header("Confirm")]
    public TMP_Text confirmSummaryText;
    public Button createCharacterButton;

    private CharacterData currentCharacterData;
    private int currentStep = 0;

    private void Awake()
    {
        currentCharacterData = new CharacterData();
        currentCharacterData.InitializeDefaults();
    }

    private void Start()
    {
        ShowStep(0);
        SetupAllButtons();
    }

    private void SetupAllButtons()
    {
        // Name
        if (nameConfirmButton != null)
            nameConfirmButton.onClick.AddListener(OnNameConfirm);

        if (nameInputField != null)
            nameInputField.onValueChanged.AddListener(delegate { UpdateNameConfirmButton(); });

        // Background
        if (backgroundConfirmButton != null)
            backgroundConfirmButton.onClick.AddListener(OnBackgroundConfirm);

        // Ability
        if (abilityConfirmButton != null)
            abilityConfirmButton.onClick.AddListener(OnAbilityConfirm);

        // Appearance
        if (appearanceConfirmButton != null)
            appearanceConfirmButton.onClick.AddListener(OnAppearanceConfirm);

        // Confirm
        if (createCharacterButton != null)
            createCharacterButton.onClick.AddListener(OnCreateCharacter);
    }

    public void ShowStep(int step)
    {
        currentStep = step;

        nameInputPanel.SetActive(step == 0);
        backgroundSelectionPanel.SetActive(step == 1);
        abilitySelectionPanel.SetActive(step == 2);
        appearanceCustomizationPanel.SetActive(step == 3);
        confirmPanel.SetActive(step == 4);

        switch (step)
        {
            case 0: SetupNameInput(); break;
            case 1: PopulateBackgroundSelection(); break;
            case 2: PopulatePathSelection(); break;
            case 3: SetupAppearance(); break;
            case 4: ShowConfirmation(); break;
        }
    }

    #region Step 0: Name
    private void SetupNameInput()
    {
        if (nameInputField != null)
            nameInputField.text = "";

        UpdateNameConfirmButton();
    }

    private void UpdateNameConfirmButton()
    {
        bool valid = !string.IsNullOrWhiteSpace(nameInputField?.text);
        if (nameConfirmButton != null)
            nameConfirmButton.interactable = valid;
    }

    private void OnNameConfirm()
    {
        string name = nameInputField?.text.Trim();
        if (string.IsNullOrEmpty(name))
        {
            NotificationSystem.Show("Enter a name!", Color.red);
            return;
        }

        currentCharacterData.characterName = name;
        ShowStep(1);
    }
    #endregion

    #region Step 1: Background
    private void PopulateBackgroundSelection()
    {
        if (backgroundButtonContainer == null || backgroundButtonPrefab == null)
        {
            Debug.LogError("Background container or prefab missing!");
            return;
        }

        foreach (Transform child in backgroundButtonContainer)
            Destroy(child.gameObject);

        var backgrounds = new[]
        {
        BackgroundType.MilitaryFamily,
        BackgroundType.Orphaned,
        BackgroundType.NobleBorn,
        BackgroundType.Scholar,
        BackgroundType.Criminal
    };

        foreach (var bg in backgrounds)
        {
            var def = BackgroundSystem.GetBackground(bg);
            if (def == null) continue;

            GameObject btnObj = Instantiate(backgroundButtonPrefab, backgroundButtonContainer);
            Button btn = btnObj.GetComponent<Button>();
            TMP_Text btnText = btnObj.GetComponentInChildren<TMP_Text>();

            if (btnText != null)
                btnText.text = def.title;

            btn.onClick.AddListener(() =>
            {
                currentCharacterData.background = bg;
                backgroundNameText.text = def.title;
                backgroundDescriptionText.text = def.description;
                backgroundConfirmButton.interactable = true;
            });
        }

        backgroundConfirmButton.interactable = false;
    }

    private void OnBackgroundConfirm()
    {
        BackgroundSystem.ApplyBackgroundEffects(currentCharacterData);
        ShowStep(2);
    }
    #endregion

    #region Step 2: Ability Path
    private void PopulatePathSelection()
    {
        if (abilityButtonContainer == null || abilityButtonPrefab == null)
        {
            Debug.LogError("Ability container or prefab missing!");
            return;
        }

        foreach (Transform child in abilityButtonContainer)
            Destroy(child.gameObject);

        var elements = new[]
        {
            AbilityElement.Fire,
            AbilityElement.Water,
            AbilityElement.Earth,
            AbilityElement.Wind
        };

        foreach (var el in elements)
        {
            string title = el.ToString() + " Strike";
            string desc = "Starter " + el.ToString().ToLower() + " skill.";

            GameObject btnObj = Instantiate(abilityButtonPrefab, abilityButtonContainer);
            Button btn = btnObj.GetComponent<Button>();
            TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
            if (txt != null) txt.text = title;

            btn.onClick.AddListener(() =>
            {
                currentCharacterData.primaryElement = el;
                currentCharacterData.primaryAbility = new AbilityData
                {
                    abilityId = el.ToString().ToLower(),
                    abilityName = title,
                    unlockedSkills = new List<string> { title }
                };
                abilityNameText.text = title;
                abilityDescriptionText.text = desc;
                abilityConfirmButton.interactable = true;
            });
        }

        abilityConfirmButton.interactable = false;
    }

    private void OnAbilityConfirm()
    {
        ShowStep(3);
    }
    #endregion

    #region Step 3: Appearance
    private void SetupAppearance()
    {
        if (skinToneDropdown == null || hairStyleDropdown == null ||
            hairColorDropdown == null || eyeColorDropdown == null)
        {
            Debug.LogError("One or more dropdowns are NULL! Assign in Inspector.");
            return;
        }

        PopulateDropdown(skinToneDropdown, new List<string> { "Light", "Fair", "Tan", "Olive", "Brown", "Dark" });
        PopulateDropdown(hairStyleDropdown, new List<string> { "Short", "Mid", "Long", "Bald", "Ponytail", "Braids", "Mohawk", "Spiky" });
        PopulateDropdown(hairColorDropdown, new List<string> { "Black", "Brown", "Blond", "Red", "Gray", "White", "Green", "Blue", "Purple", "Pink" });
        PopulateDropdown(eyeColorDropdown, new List<string> { "Brown", "Blue", "Green", "Gray", "Hazel", "Red", "Purple", "Amber", "Black", "White" });

        appearanceConfirmButton.interactable = true;
    }

    private void PopulateDropdown(TMP_Dropdown dropdown, List<string> options)
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(options);
        dropdown.value = 0;
        dropdown.RefreshShownValue();
    }

    private void OnAppearanceConfirm()
    {
        currentCharacterData.skinTone = skinToneDropdown.options[skinToneDropdown.value].text;
        currentCharacterData.hairStyle = hairStyleDropdown.options[hairStyleDropdown.value].text;
        currentCharacterData.hairColor = hairColorDropdown.options[hairColorDropdown.value].text;
        currentCharacterData.eyeColor = eyeColorDropdown.options[eyeColorDropdown.value].text;
        ShowStep(4);
    }
    #endregion

    #region Step 4: Confirm
    private void ShowConfirmation()
    {
        confirmSummaryText.text =
            $"<b>Name:</b> {currentCharacterData.characterName}\n" +
            $"<b>Background:</b> {currentCharacterData.background}\n" +
            $"<b>Element:</b> {currentCharacterData.primaryElement}\n" +
            $"<b>Skin:</b> {currentCharacterData.skinTone} | <b>Hair:</b> {currentCharacterData.hairStyle} {currentCharacterData.hairColor} | <b>Eyes:</b> {currentCharacterData.eyeColor}\n" +
            $"<b>Credits:</b> {currentCharacterData.credits}";
    }

    private async void OnCreateCharacter()
    {
        createCharacterButton.interactable = false;
        currentCharacterData.characterId = System.Guid.NewGuid().ToString();

        bool success = await FirebaseCharacterDatabase.Instance.SaveNewCharacterAsync(currentCharacterData);
        if (success)
        {
            GameDataManager.Instance.LoadCharacter(currentCharacterData);
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
        else
        {
            NotificationSystem.Show("Save failed!", Color.red);
            createCharacterButton.interactable = true;
        }
    }
    #endregion
}