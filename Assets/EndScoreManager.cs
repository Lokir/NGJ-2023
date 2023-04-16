using UnityEngine;
using UnityEngine.UI;

public class EndScoreManager : MonoBehaviour
{
    [SerializeField] GameObject[] scoreImages;
    [SerializeField] GameObject restartButton;
    [SerializeField] int numberOfGames = 8;
    private int score = 0;
    int gameNumber = 0;
    // Start is called before the first frame update
    public EndScoreManager Factory(Transform parent) {
        var instance = Instantiate(this, parent);
        return instance;
    }
    void Start()
    {
        score = 0;
        foreach (GameObject image in scoreImages) {
            image.SetActive(false);
            restartButton.SetActive(false);
        }
    }

    public void UpdateScore(int value) {
        score += value;
        
        /*
        gameNumber++;
        if (gameNumber == numberOfGames) {
            GameOver(score / 2);
        }
        */
    }

    public int GetScore() {
        return score;
    }

    public void GameOver(int score) {
        if (score >= scoreImages.Length) {
            score = scoreImages.Length - 1;
        }
        if (score < 0 ) {
            score = 0;
        }
        scoreImages[score].SetActive(true);
        scoreImages[score].GetComponent<AudioSource>()?.Play();
        restartButton.SetActive(true);
    }
}
