using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public void PlayTestingFacility() {
        SceneManager.LoadScene("TestingFacility");
    }

    public void QuitGame() {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void Itchio() {
        Application.OpenURL("https://sens1tiv.itch.io/");
        Debug.Log("is this working?");
    }

}
