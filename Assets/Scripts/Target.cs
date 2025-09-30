using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour

{
    private Rigidbody targetRb;
    private float minSpeed = 10;
    private float maxSpeed = 14;
    private float maxTorque = 10;
    private float xRange = 4;
    private float zRange = -48;
    private float ySpawnPos = -2;
    private GameManager gameManager;
    public int pointValue;
    public ParticleSystem explosionParticle;
    void Start()
    {
        targetRb = GetComponent<Rigidbody>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

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
            if (CompareTag("Bad"))
            {
                PlaySound(gameManager.bombSound);
                gameManager.DecreaseLife();
                gameManager.TriggerRedOverlay();
                Destroy(gameObject);
                Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
            }
            else if (CompareTag("Myst"))
            {
                PlaySound(gameManager.chimeSound);
                DestroyMysteryBoxObjects();
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
        if (!gameObject.CompareTag("Bad") && !gameObject.CompareTag("Myst"))
        {
            gameManager.DecreaseLife();
        }
    }
    private void PlaySound(AudioClip clip)
    {
        gameManager.audioSource.PlayOneShot(clip);
    }

    private void DestroyMysteryBoxObjects()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag("Bad") || obj.CompareTag("Myst") || obj == gameObject) 
                continue;

            if (obj.GetComponent<Target>() != null)
            {
                Instantiate(explosionParticle, obj.transform.position, explosionParticle.transform.rotation);
                Destroy(obj);
            }
        }

        Instantiate(explosionParticle, transform.position, explosionParticle.transform.rotation);
        Destroy(gameObject);
    }
}


