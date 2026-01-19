using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Gère l'état du jeu, le score, les vies, le spawn des cibles et l'interface
public class GameManager : MonoBehaviour
{
    private int lives = 3; // Nombre de vies restantes du joueur
    private int score;
    private float spawnRate = 1.0f; // Délai entre chaque spawn de cible (en secondes)

    public GameObject[] hearts; // Tableau des icônes de cœurs (vies) dans l'UI
    public List<GameObject> targets; // Liste des préfabs de cibles à faire apparaître
    public TextMeshProUGUI scoreText; // Texte affichant le score
    public bool isGameActive;
    public GameObject startScreen; // Écran de démarrage
    public GameObject endScreen; // Écran de fin de partie
    public Image redOverlay; // Overlay rouge (effet visuel pour bombe)
    public Image goldOverlay; // Overlay doré (effet visuel pour boîte mystère)

    public AudioSource audioSource; // Source audio pour jouer les sons
    public AudioClip reloadSound; // Son de rechargement au début
    public AudioClip gunshotSound; // Son de tir
    public AudioClip chimeSound; // Son de boîte mystère
    public AudioClip bombSound; // Son de bombe
    public AudioClip gameOverSound; // Son de fin de partie

    private Coroutine spawnCoroutine; // Référence à la coroutine de spawn (pour l'arrêter)

    void Start()
    {
        // Configuration initiale de l'interface
        startScreen.SetActive(true);
        endScreen.SetActive(false);
        scoreText.gameObject.SetActive(false);
        redOverlay.gameObject.SetActive(false);
        goldOverlay.gameObject.SetActive(false);

        // Active tous les cœurs (vies pleines)
        foreach (GameObject heart in hearts)
        {
            heart.SetActive(true);
        }
    }

    // Lance le jeu en mode facile (difficulté 1)
    public void StartEasyGame()
    {
        StartGame(1);
    }

    // Lance le jeu en mode moyen (difficulté 2)
    public void StartMediumGame()
    {
        StartGame(2);
    }

    // Lance le jeu en mode difficile (difficulté 3)
    public void StartHardGame()
    {
        StartGame(3);
    }

    // Initialise le jeu selon la difficulté choisie
    private void StartGame(int difficulty)
    {
        isGameActive = true;
        score = 0;
        lives = 3;
        spawnRate = 1.0f / difficulty; // Plus la difficulté augmente, plus le spawn est rapide

        UpdateScore(0); // Affiche le score initial

        // Configure l'interface pour le jeu actif
        startScreen.SetActive(false);
        endScreen.SetActive(false);
        scoreText.gameObject.SetActive(true);

        // Réactive tous les cœurs
        foreach (GameObject heart in hearts)
        {
            heart.SetActive(true);
        }

        audioSource.PlayOneShot(reloadSound); // Joue le son de démarrage
        spawnCoroutine = StartCoroutine(SpawnTarget()); // Commence à faire apparaître des cibles
    }

    // Enlève une vie au joueur
    public void DecreaseLife()
    {
        lives--;

        // Désactive le cœur correspondant dans l'UI
        if (lives >= 0 && lives < hearts.Length)
        {
            hearts[lives].SetActive(false);
        }

        // Si plus de vies, déclenche le game over
        if (lives <= 0)
        {
            StartCoroutine(GameOver());
        }
    }

    // Gère la fin de partie
    private IEnumerator GameOver()
    {
        isGameActive = false; // Arrête le jeu
        StopCoroutine(spawnCoroutine); // Arrête le spawn de nouvelles cibles

        // Détruit toutes les cibles restantes sur la scène
        Target[] allTargets = FindObjectsOfType<Target>();
        foreach (Target target in allTargets)
        {
            Destroy(target.gameObject);
        }

        yield return new WaitForSeconds(1f); // Petit délai avant d'afficher l'écran final

        endScreen.SetActive(true); // Affiche l'écran de fin
        audioSource.PlayOneShot(gameOverSound); // Joue le son de game over
    }

    // Recharge la scène actuelle (recommence le jeu)
    public void RestartGame()
    {
        Time.timeScale = 1f; // Remet le temps à la normale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Coroutine qui fait apparaître des cibles à intervalles réguliers
    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate); // Attend selon le taux de spawn

            int index = Random.Range(0, targets.Count); // Choisit une cible aléatoire
            Instantiate(targets[index]); // Crée la cible
        }
    }

    // Ajoute des points au score et met à jour l'affichage
    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
    }

    // Déclenche l'effet visuel rouge (pour les bombes)
    public void TriggerRedOverlay()
    {
        StartCoroutine(FadeOverlay(redOverlay));
    }

    // Déclenche l'effet visuel doré (pour les boîtes mystère)
    public void TriggerGoldOverlay()
    {
        StartCoroutine(FadeOverlay(goldOverlay));
    }

    // Fait apparaître puis disparaître progressivement un overlay de couleur
    private IEnumerator FadeOverlay(Image overlayColor)
    {
        overlayColor.gameObject.SetActive(true);
        Color color = overlayColor.color;
        color.a = 0.5f; // Commence à 50% d'opacité
        overlayColor.color = color;

        yield return new WaitForSeconds(0.2f); // Garde l'overlay visible brièvement

        // Diminue progressivement l'opacité jusqu'à 0
        while (overlayColor.color.a > 0)
        {
            color.a -= Time.deltaTime * 2f; // Vitesse de disparition
            overlayColor.color = color;
            yield return null;
        }

        overlayColor.gameObject.SetActive(false); // Désactive l'overlay
    }
}