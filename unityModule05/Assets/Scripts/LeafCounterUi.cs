using UnityEngine;
using TMPro;

public class LeafCounterUI : MonoBehaviour
{
    public TextMeshProUGUI leafText;

    private int lastCount = -1;

    void Update()
    {
        if (GameManager.Instance == null) return;

        int current = GameManager.Instance.leavesCollectedThisStage;

        if (current != lastCount)
        {
            leafText.text = "Leaves: " + current;
            lastCount = current;
        }
    }
}
