using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DiaryUI : MonoBehaviour
{
    public TextMeshProUGUI totalLeafPointsText;
    public TextMeshProUGUI deathCountText;
    public TextMeshProUGUI stageListText;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            var gm = GameManager.Instance;

            totalLeafPointsText.text = $"Total leaf points: {gm.totalLeafPoints}";
            deathCountText.text = $"Total death: {gm.DeathCount}";
            stageListText.text = BuildStageList(gm);
        }
        else
        {
            totalLeafPointsText.text = "Total leaf points: 0";
            deathCountText.text = "Total death: 0";
            stageListText.text = "No GameManager found.";
        }
    }

    private string BuildStageList(GameManager gm)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int i = 0; i < gm.stages.Length; i++)
        {
            bool unlocked = i <= gm.HighestStageUnlocked;
            string status = unlocked ? "Unlocked" : "Locked";

            sb.AppendLine($"{gm.stages[i]} : {status}");
        }

        return sb.ToString();
    }

    public void OnBackToMenuClicked()
    {
        Debug.Log("DiaryUI: Back button clicked, loading MainMenu...");
        SceneManager.LoadScene("MainMenu");
    }
}
