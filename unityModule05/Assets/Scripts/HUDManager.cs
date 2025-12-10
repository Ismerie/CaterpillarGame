using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI leafPointsText;

    private Canvas canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        canvas = GetComponent<Canvas>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ❗ HUD CACHÉ dans MainMenu ET Diary
        if (scene.name == "MainMenu" || scene.name == "Diary")
        {
            if (canvas != null)
                canvas.enabled = false;
        }
        else
        {
            if (canvas != null)
                canvas.enabled = true;
        }
    }

    private void LateUpdate()
    {
        if (canvas == null || !canvas.enabled)
            return;

        UpdateHP();
        UpdateLeafPoints();
    }

    private void UpdateHP()
    {
        var player = Object.FindFirstObjectByType<PlayerController>();
        if (player != null)
            hpText.text = $"HP: {player.CurrentHP}/{player.maxHP}";
        else
            hpText.text = "HP: --";
    }

    private void UpdateLeafPoints()
    {
        if (GameManager.Instance != null)
            leafPointsText.text = $"Leaf points: {GameManager.Instance.currentScore}";
        else
            leafPointsText.text = "Leaf points: 0";
    }
}
