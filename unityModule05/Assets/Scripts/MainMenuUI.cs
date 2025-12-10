using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnResumeClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
    }

    public void OnNewGameClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.NewGame();
    }

    public void OnDiaryClicked()
    {
        SceneManager.LoadScene("Diary");
    }
}
