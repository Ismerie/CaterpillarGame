using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // La chenille
    public float smoothTime = 0.2f;
    public float fixedY = 0f;     // Y fixe pour la caméra

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Position actuelle de la caméra
        Vector3 currentPos = transform.position;

        // Position cible : on suit en X mais pas en Y
        Vector3 targetPos = new Vector3(
            target.position.x,   // suivre en X
            fixedY,             // Y fixe
            -10f                // Z fixe pour caméra 2D
        );

        // Mouvement doux
        Vector3 newPos = Vector3.SmoothDamp(currentPos, targetPos, ref velocity, smoothTime);

        transform.position = newPos;
    }
}
