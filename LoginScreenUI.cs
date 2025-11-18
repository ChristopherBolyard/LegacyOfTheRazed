using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class LoginScreenUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField emailField;
    public TMP_InputField passwordField;
    public Button loginButton;
    public Button registerButton;
    public Button quitButton;

    [Header("Optional Feedback")]
    public TextMeshProUGUI statusText;

    private void Start()
    {
        // Setup button listeners
        if (loginButton != null)
            loginButton.onClick.AddListener(OnLoginClicked);

        if (registerButton != null)
            registerButton.onClick.AddListener(OnRegisterClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(Application.Quit);

        // Optional: Clear status
        if (statusText != null)
            statusText.text = "";
    }

    private async void OnLoginClicked()
    {
        string email = emailField?.text ?? "";
        string password = passwordField?.text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowStatus("Please enter email and password.", Color.red);
            return;
        }

        ShowStatus("Logging in...", Color.yellow);

        // Initialize Firebase only when needed
        AuthenticationManager.Instance.InitializeFirebase();
        await Task.Delay(150); // Give Firebase time to init

        bool success = await AuthenticationManager.Instance.LoginAsync(email, password);

        if (success)
        {
            ShowStatus("Login successful!", Color.green);
            // Scene loads inside LoginAsync
        }
        else
        {
            ShowStatus("Login failed. Check credentials.", Color.red);
        }
    }

    private async void OnRegisterClicked()
    {
        string email = emailField?.text ?? "";
        string password = passwordField?.text ?? "";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            ShowStatus("Please enter email and password.", Color.red);
            return;
        }

        if (password.Length < 6)
        {
            ShowStatus("Password must be 6+ characters.", Color.red);
            return;
        }

        ShowStatus("Registering...", Color.yellow);

        AuthenticationManager.Instance.InitializeFirebase();
        await Task.Delay(150);

        bool success = await AuthenticationManager.Instance.RegisterAsync(email, password);

        if (success)
        {
            ShowStatus("Registered! Now log in.", Color.green);
        }
        else
        {
            ShowStatus("Registration failed.", Color.red);
        }
    }

    private void ShowStatus(string message, Color color)
    {
        if (statusText != null)
        {
            statusText.text = message;
            statusText.color = color;
        }

        // Also use NotificationSystem if available
        NotificationSystem.Show(message, color);
    }

    private void OnDestroy()
    {
        // Clean up listeners
        if (loginButton != null)
            loginButton.onClick.RemoveAllListeners();
        if (registerButton != null)
            registerButton.onClick.RemoveAllListeners();
        if (quitButton != null)
            quitButton.onClick.RemoveAllListeners();
    }
}