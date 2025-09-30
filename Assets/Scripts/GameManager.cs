using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private int lives = 3;
    public TextMeshProUGUI livesText;
    public GameObject heart;
    private int score;
    private float spawmRate = 1.0f;
    public List<GameObject> targets;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameOverText;
    public bool isGameActive;
    public Button restartButton;
    public GameObject titleScreen;
    public Image redOverlay;
    public Image goldOverlay;

    public AudioSource audioSource;
    public AudioSource backgroundMusic;
    public AudioClip reloadSound;
    public AudioClip gunshotSound;
    public AudioClip chimeSound;
    public AudioClip bombSound;




    // Start is called before the first frame update
    void Start()
    {
        livesText.text = "" + lives; // Initialize lives UI

    }

    public void StartGame(int difficulty)
    {
        isGameActive = true;
        StartCoroutine(SpawnTarget());
        score = 0;
        lives = 3;
        UpdateScore(0);
        titleScreen.gameObject.SetActive(false);
        redOverlay.gameObject.SetActive(false);
        livesText.gameObject.SetActive(true);
        livesText.text = "" + lives;
        spawmRate /= difficulty;

        audioSource.PlayOneShot(reloadSound);

        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        isGameActive = false;
        restartButton.gameObject.SetActive(true);
        livesText.gameObject.SetActive(false);
    }

    public void DecreaseLife()
    {
        lives--;
        livesText.text = "" + lives;

        if (lives <= 0)
        {
            GameOver();
        }
    }


    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawmRate);
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

    private IEnumerator FadeOverlay(Image OverlayColor)
    {
        OverlayColor.gameObject.SetActive(true); // Show the overlay
        Color color = OverlayColor.color;
        color.a = 0.5f; // Set alpha to 50% visibility
        OverlayColor.color = color;

        // Wait briefly to keep the overlay visible
        yield return new WaitForSeconds(0.2f);

        // Gradually fade out the overlay
        while (OverlayColor.color.a > 0)
        {
            color.a -= Time.deltaTime;
            OverlayColor.color = color;
            yield return null;
        }

        OverlayColor.gameObject.SetActive(false); // Hide the overlay after fading out
    }
}

