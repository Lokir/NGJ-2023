using System;
using Core.MotorTest.Scripts;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private BreadItGameController gameController;
    public GameObject menuPanel, gameScreen, scoreScreen;
    bool paused;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)){
            PauseGame(paused);
        }
    }
    private void PauseGame(bool pauseState) {
        paused = !paused;
        menuPanel.SetActive(paused);
        gameScreen.SetActive(!paused);
    }

    public void LoadLevel(int levelBuildIncex)
    {
        StartLevel();
    }
    
    public void StartLevel() {
        if (gameController.StartGame())
        {
            menuPanel.SetActive(false);
            scoreScreen.SetActive(false);
            gameScreen.SetActive(true);
        }
    }

    public void Quit() {
        Application.Quit();
    }

}
