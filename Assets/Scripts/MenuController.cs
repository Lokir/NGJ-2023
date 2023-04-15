using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject menuPanel, gameScreen;
    bool paused;
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)){
            PauseGame(paused);
        }
    }
    private void PauseGame(bool pauseState) {
        paused = !paused;
        menuPanel.SetActive(paused);
        gameObject.SetActive(!paused);
    }

    public static void LoadLevel(int levelBuildIncex) {
        SceneManager.LoadScene(levelBuildIncex);
    }  

    public void StartLevel() {
        menuPanel.SetActive(false);
        gameObject.SetActive(true);
        //Call funciton to start the Quicktime Events
    }

    public void Quit() {
        Application.Quit();
    }

}
