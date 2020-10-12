using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void PlayGame(){
        SceneManager.LoadScene("Main");
    }

    public void QuitGame(){
        Debug.Log("You Quit the game");
        Application.Quit();
    }

}
