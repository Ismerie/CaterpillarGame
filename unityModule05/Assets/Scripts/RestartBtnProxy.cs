using UnityEngine;

public class ButtonProxy : MonoBehaviour
{
    public void OnRestartButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartCurrentStage();
        }
        else
        {
            Debug.LogWarning("No GameManager.Instance found when trying to restart stage.");
        }
    }

    public void OnReturnToMenuClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnToMainMenu();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
