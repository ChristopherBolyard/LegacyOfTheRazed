using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NotificationSystem : MonoBehaviour
{
    public static NotificationSystem Instance { get; private set; }

    [Header("Prefab & Parent")]
    public GameObject notificationPrefab;           // ← MUST ASSIGN IN INSPECTOR
    public Transform parentPanel;                   // ← Canvas or UI Panel

    [Header("Pool Settings")]
    public int poolSize = 5;
    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        if (notificationPrefab == null)
        {
            Debug.LogError("[NotificationSystem] Prefab is NULL! Assign in Inspector.");
            return;
        }

        if (parentPanel == null)
        {
            parentPanel = GameObject.Find("Canvas")?.transform;
            if (parentPanel == null)
            {
                Debug.LogError("[NotificationSystem] No Canvas found! Create one.");
                return;
            }
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(notificationPrefab, parentPanel);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public static void Show(string message, Color color)
    {
        if (Instance == null) return;

        GameObject notification = Instance.GetPooledObject();
        if (notification != null)
        {
            Instance.SetupNotification(notification, message, color);
        }
    }

    private GameObject GetPooledObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        // Expand pool if needed
        if (notificationPrefab != null)
        {
            GameObject obj = Instantiate(notificationPrefab, parentPanel);
            return obj;
        }

        return null;
    }

    private void SetupNotification(GameObject obj, string message, Color color)
    {
        var text = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = message;
            text.color = color;
        }

        var bg = obj.GetComponent<Image>();
        if (bg != null)
        {
            bg.color = new Color(color.r, color.g, color.b, 0.9f);
        }

        StartCoroutine(FadeOut(obj));
    }

    private IEnumerator FadeOut(GameObject obj)
    {
        CanvasGroup cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        cg.alpha = 1f;
        float duration = 2f;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null;
        }

        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}