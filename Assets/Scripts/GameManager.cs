using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int lives = 3;
    private int score;
    private float spawnRate = 1.0f;

    public GameObject[] hearts;
    public List<GameObject> targets;
    public TextMeshProUGUI scoreText;
    public bool isGameActive;
    public GameObject startScreen;
    public GameObject endScreen;
    public Image redOverlay;
    public Image goldOverlay;

    public AudioSource audioSource;
    public AudioClip reloadSound;
    public AudioClip gunshotSound;
    public AudioClip chimeSound;
    public AudioClip bombSound;
    public AudioClip gameOverSound;

    private Coroutine spawnCoroutine;

    void Start()
    {
        startScreen.SetActive(true);
        endScreen.SetActive(false);
        scoreText.gameObject.SetActive(false);
        redOverlay.gameObject.SetActive(false);
        goldOverlay.gameObject.SetActive(false);

        foreach (GameObject heart in hearts)
        {
            heart.SetActive(true);
        }
    }

    public void StartEasyGame()
    {
        StartGame(1);
    }

    public void StartMediumGame()
    {
        StartGame(2);
    }

    public void StartHardGame()
    {
        StartGame(3);
    }

    private void StartGame(int difficulty)
    {
        isGameActive = true;
        score = 0;
        lives = 3;
        spawnRate = 1.0f / difficulty;

        UpdateScore(0);

        startScreen.SetActive(false);
        endScreen.SetActive(false);
        scoreText.gameObject.SetActive(true);

        foreach (GameObject heart in hearts)
        {
            heart.SetActive(true);
        }

        audioSource.PlayOneShot(reloadSound);
        spawnCoroutine = StartCoroutine(SpawnTarget());
    }

    public void DecreaseLife()
    {
        lives--;

        if (lives >= 0 && lives < hearts.Length)
        {
            hearts[lives].SetActive(false);
        }

        if (lives <= 0)
        {
            StartCoroutine(GameOver());
        }
    }

    private IEnumerator GameOver()
    {
        isGameActive = false;
        StopCoroutine(spawnCoroutine);

        Target[] allTargets = FindObjectsOfType<Target>();
        foreach (Target target in allTargets)
        {
            Destroy(target.gameObject);
        }

        yield return new WaitForSeconds(1f);

        endScreen.SetActive(true);
        audioSource.PlayOneShot(gameOverSound);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);

            int index = Random.Range(0, targets.Count);
            Instantiate(targets[index]);
        }
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    public void TriggerRedOverlay()
    {
        StartCoroutine(FadeOverlay(redOverlay));
    }

    public void TriggerGoldOverlay()
    {
        StartCoroutine(FadeOverlay(goldOverlay));
    }

    private IEnumerator FadeOverlay(Image overlayColor)
    {
        overlayColor.gameObject.SetActive(true);
        Color color = overlayColor.color;
        color.a = 0.5f;
        overlayColor.color = color;

        yield return new WaitForSeconds(0.2f);

        while (overlayColor.color.a > 0)
        {
            color.a -= Time.deltaTime * 2f;
            overlayColor.color = color;
            yield return null;
        }

        overlayColor.gameObject.SetActive(false);
    }
}