using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Threading.Tasks;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance { get; private set; }

    private FirebaseAuth auth;
    private FirebaseUser user;
    private bool isFirebaseReady = false;

    void Awake()
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

    // CALL THIS FROM LOGIN BUTTON ONLY
    public async void InitializeFirebase()
    {
        if (isFirebaseReady) return;

        Debug.Log("Initializing Firebase...");

        var dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        while (!dependencyTask.IsCompleted)
        {
            await Task.Yield(); // Prevent main thread freeze
        }

        if (dependencyTask.Exception != null)
        {
            Debug.LogError($"Firebase init failed: {dependencyTask.Exception}");
            return;
        }

        var status = dependencyTask.Result;
        if (status == DependencyStatus.Available)
        {
            auth = FirebaseAuth.DefaultInstance;
            auth.StateChanged += AuthStateChanged;
            AuthStateChanged(this, null);
            isFirebaseReady = true;
            Debug.Log("Firebase ready!");
        }
        else
        {
            Debug.LogError($"Firebase unavailable: {status}");
        }
    }

    void AuthStateChanged(object sender, System.EventArgs e)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            user = auth.CurrentUser;
            if (signedIn) Debug.Log("Signed in: " + user.UserId);
        }
    }

    public string GetCurrentUserId()
    {
        return isFirebaseReady && auth?.CurrentUser != null ? auth.CurrentUser.UserId : "offline_user";
    }

    public async Task<bool> RegisterAsync(string email, string password)
    {
        if (!isFirebaseReady) { InitializeFirebase(); await Task.Delay(100); }
        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            return true;
        }
        catch { return false; }
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        if (!isFirebaseReady) { InitializeFirebase(); await Task.Delay(100); }
        try
        {
            await auth.SignInWithEmailAndPasswordAsync(email, password);
            UnityEngine.SceneManagement.SceneManager.LoadScene("02_CharacterSelectionManager");
            return true;
        }
        catch { return false; }
    }

    void OnDestroy()
    {
        if (auth != null) auth.StateChanged -= AuthStateChanged;
    }
}