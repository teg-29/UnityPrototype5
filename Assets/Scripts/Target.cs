using UnityEngine;

// Contrôle le comportement des cibles : spawn, physique, interactions et destruction
public class Target : MonoBehaviour
{
    private Rigidbody targetRb; // Composant physique de la cible
    private float minSpeed = 10; // Vitesse minimale de lancement vers le haut
    private float maxSpeed = 14; // Vitesse maximale de lancement vers le haut
    private float maxTorque = 10; // Force de rotation maximale
    private float xRange = 4; // Portée horizontale du spawn (gauche/droite)
    private float zRange = -90; // Portée en profondeur du spawn
    private float ySpawnPos = -2; // Position Y de spawn (en bas de l'écran)
    private GameManager gameManager; // Référence au GameManager
    public int pointValue; // Valeur en points de la cible
    public ParticleSystem explosionParticle; // Effet de particules à la destruction

    void Start()
    {
        targetRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Lance la cible vers le haut avec une force aléatoire
        targetRb.AddForce(RandomForce(), ForceMode.Impulse);

        // Applique une rotation aléatoire sur tous les axes
        targetRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);

        // Place la cible à une position de spawn aléatoire
        transform.position = RandomSpawnPos();
    }

    // Retourne une force verticale aléatoire (pour lancer la cible)
    Vector3 RandomForce()
    {
        return Vector3.up * Random.Range(minSpeed, maxSpeed);
    }

    // Retourne une valeur de rotation aléatoire
    float RandomTorque()
    {
        return Random.Range(-maxTorque, maxTorque);
    }

    // Retourne une position de spawn aléatoire dans les limites définies
    Vector3 RandomSpawnPos()
    {
        return new Vector3(Random.Range(-xRange, xRange), ySpawnPos, Random.Range(0, zRange));
    }

    // Gère le clic sur la cible
    private void OnMouseDown()
    {
        if (gameManager.isGameActive)
        {
            // Si c'est une bombe : perd une vie et effet rouge
            if (CompareTag("Bomb"))
            {
                PlaySound(gameManager.bombSound);
                Destroy(gameObject);
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
                gameManager.TriggerRedOverlay();
                gameManager.DecreaseLife();
            }

            // Si c'est une boîte mystère : détruit toutes les autres cibles et effet doré
            else if (CompareTag("MBox"))
            {
                PlaySound(gameManager.chimeSound);
                ClearMap(); // Détruit toutes les cibles normales
                Destroy(gameObject);
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
                gameManager.TriggerGoldOverlay();
                gameManager.UpdateScore(pointValue);
            }

            // Cible normale : ajoute des points
            else
            {
                PlaySound(gameManager.gunshotSound);
                Destroy(gameObject);
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
                gameManager.UpdateScore(pointValue);
            }
        }
    }

    // Détecte quand la cible sort de l'écran (trigger zone)
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);

        // Si c'est une cible normale ratée, perd une vie
        if (!gameObject.CompareTag("Bomb") && !gameObject.CompareTag("MBox"))
        {
            gameManager.DecreaseLife();
        }
    }

    // Détruit toutes les cibles sur la carte sauf les bombes
    private void ClearMap()
    {
        Target[] allTargets = FindObjectsOfType<Target>();

        foreach (Target target in allTargets)
        {
            if (target.CompareTag("Bomb")) // Ignore les bombes
                continue;
            Destroy(target.gameObject);
            Instantiate(explosionParticle, target.transform.position, explosionParticle.transform.rotation);
            gameManager.UpdateScore(target.pointValue); // Ajoute les points de chaque cible détruite
        }
    }

    // Joue un son via l'AudioSource du GameManager
    private void PlaySound(AudioClip clip)
    {
        gameManager.audioSource.PlayOneShot(clip);
    }
}