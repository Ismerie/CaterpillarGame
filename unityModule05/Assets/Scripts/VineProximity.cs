using UnityEngine;

public class VineProximity : MonoBehaviour
{
    public string playerTag = "Player";
    public string awakeBoolName = "IsAwake";

    private Animator anim;

    private void Awake()
    {
        // On va chercher l'Animator sur le parent "Vine"
        anim = GetComponentInParent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (anim != null && !string.IsNullOrEmpty(awakeBoolName))
        {
            anim.SetBool(awakeBoolName, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        if (anim != null && !string.IsNullOrEmpty(awakeBoolName))
        {
            anim.SetBool(awakeBoolName, false);
        }
    }
}
