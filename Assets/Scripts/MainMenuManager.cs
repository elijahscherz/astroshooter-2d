using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour {

	// Use this for initialization
	public void PlayButtonClick()
    {
        Application.LoadLevel("Game");
    }

    public void QuitButtonClick()
    {
        Application.Quit();
    }
}
