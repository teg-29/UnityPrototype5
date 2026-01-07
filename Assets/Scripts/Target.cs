using UnityEngine;

public class Target : MonoBehaviour

{
    private Rigidbody targetRb;
    private float minSpeed = 10;
    private float maxSpeed = 14;
    private float maxTorque = 10;
    private float xRange = 4;
    private float zRange = -90;
    private float ySpawnPos = -2;
    private GameManager gameManager;
    public int pointValue;
    public ParticleSystem explosionParticle;

    void Start()
    {
        targetRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        targetRb.AddForce(RandomForce(), ForceMode.Impulse);
        targetRb.AddTorque(RandomTorque(), RandomTorque(), RandomTorque(), ForceMode.Impulse);
        transform.position = RandomSpawnPos();
    }

    Vector3 RandomForce()
    {
        return Vector3.up * Random.Range(minSpeed, maxSpeed);
    }

    float RandomTorque()
    {
        return Random.Range(-maxTorque, maxTorque);
    }

    Vector3 RandomSpawnPos()
    {
        return new Vector3(Random.Range(-xRange, xRange), ySpawnPos, Random.Range(0, zRange));
    }

    private void OnMouseDown()
    {
        if (gameManager.isGameActive)
        {
            if (CompareTag("Bomb"))
            {
                PlaySound(gameManager.bombSound);
                Destroy(gameObject);
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
                gameManager.TriggerRedOverlay();
                gameManager.DecreaseLife();
            }

            else if (CompareTag("MBox"))
            {
                PlaySound(gameManager.chimeSound);
                ClearMap();
                Destroy(gameObject);
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
                gameManager.TriggerGoldOverlay();
                gameManager.UpdateScore(pointValue);

            }

            else
            {
                PlaySound(gameManager.gunshotSound);
                Destroy(gameObject);
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
                gameManager.UpdateScore(pointValue);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);

        if (!gameObject.CompareTag("Bomb") && !gameObject.CompareTag("MBox"))
        {
            gameManager.DecreaseLife();
        }
    }

    private void ClearMap()
    {
        Target[] allTargets = FindObjectsOfType<Target>();

        foreach (Target target in allTargets)
        {
            if (target.CompareTag("Bomb"))
                continue;
            Destroy(target.gameObject);
            Instantiate(explosionParticle, target.transform.position, explosionParticle.transform.rotation);
            gameManager.UpdateScore(target.pointValue);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        gameManager.audioSource.PlayOneShot(clip);
    }
}