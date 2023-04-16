using System;
using Core.MotorTest.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private BreadItGameController gameController;
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
        gameController.StartGame();
    }

    public void Quit() {
        Application.Quit();
    }

}
