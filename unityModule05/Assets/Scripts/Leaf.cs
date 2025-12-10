using UnityEngine;

public class Leaf : MonoBehaviour
{
    public float minFallSpeed = 0.5f;
    public float maxFallSpeed = 2f;
    public float horizontalAmplitude = 0.5f;   // amplitude du mouvement gauche/droite
    public float horizontalFrequency = 1f;     // vitesse de l’oscillation
    public float destroyY = -6f;               // position où on détruit la feuille

    private float fallSpeed;
    private float startX;
    private float timeOffset;

    void Start()
    {
        fallSpeed = Random.Range(minFallSpeed, maxFallSpeed);
        startX = transform.position.x;
        timeOffset = Random.Range(0f, 10f);
    }

    void Update()
    {
        float t = Time.time + timeOffset;

        // Mouvement vers le bas
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        // Oscillation gauche / droite
        float x = startX + Mathf.Sin(t * horizontalFrequency) * horizontalAmplitude;
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

        // Destruction quand la feuille sort de l’écran
        if (transform.position.y < destroyY)
        {
            Destroy(gameObject);
        }
    }
}
