using UnityEngine;

public class EndPoint : MonoBehaviour
{
    [Header("UI Message si pas assez de points")]
    public GameObject notEnoughPointsPanel; // un panel UI avec un Text ou TMP
    
    private void Start()
    {
        if (notEnoughPointsPanel != null)
            notEnoughPointsPanel.SetActive(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (GameManager.Instance == null)
            return;

        if (GameManager.Instance.CanPassEndPoint())
        {
            // On cache le message si jamais il était déjà affiché
            if (notEnoughPointsPanel != null)
                notEnoughPointsPanel.SetActive(false);

            GameManager.Instance.GoToNextStage();
        }
        else
        {
            if (notEnoughPointsPanel != null)
                notEnoughPointsPanel.SetActive(true);
        }
    }
}
