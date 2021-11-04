using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour {


    public GameObject pauseMenuUI;
    public GameObject HUD;


    void Update() {
        
        if(Input.GetKeyDown(KeyCode.Escape)) {

            if(GameplayHandler.GameIsPaused)
                Resume();
            else
                Pause();

        }

    }

    void Resume() {

        pauseMenuUI.SetActive(false);
        HUD.SetActive(true);
        Time.timeScale = 1f;
        GameplayHandler.GameIsPaused = false;

    }

    void Pause() {

        pauseMenuUI.SetActive(true);
        HUD.SetActive(false);
        Time.timeScale = 0f;
        GameplayHandler.GameIsPaused = true;

    }


}
