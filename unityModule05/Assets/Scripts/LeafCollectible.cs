using UnityEngine;
using UnityEngine.SceneManagement;

public class LeafCollectible : MonoBehaviour
{
    public string leafId = "Leaf_1";   // à personnaliser dans l'inspector

    private string GetPrefKey()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return $"LeafCollected_{sceneName}_{leafId}";
    }

    private void Start()
    {
        // On cache les feuilles déjà prises UNIQUEMENT quand on fait Resume
        if (GameManager.Instance != null && GameManager.Instance.IsLoadingFromSave())
        {
            string key = GetPrefKey();
            if (PlayerPrefs.GetInt(key, 0) == 1)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddLeaf();
        }

        // On enregistre que cette feuille a été prise
        string key = GetPrefKey();
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();

        Destroy(gameObject);
    }
}