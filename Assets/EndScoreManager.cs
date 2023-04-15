using UnityEngine;
using UnityEngine.UI;

public class EndScoreManager : MonoBehaviour
{
    [SerializeField] GameObject[] scoreImages;
    [SerializeField] GameObject restartButton;
    // Start is called before the first frame update
    public EndScoreManager Factory(Transform parent) {
        var instance = Instantiate(this, parent);
        return instance;
    }
    void Start()
    {
        foreach (GameObject image in scoreImages) {
            image.SetActive(false);
            restartButton.SetActive(false);
        }
    }
    public void GameOver(int score) {
        if (score >= scoreImages.Length) {
            score = scoreImages.Length - 1;
        }
        if (score < 0 ) {
            score = 0;
        }
        scoreImages[score].SetActive(true);
        restartButton.SetActive(true);
    }

}
