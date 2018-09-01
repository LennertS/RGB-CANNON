using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class StatsLoggerScript : MonoBehaviour {

    public TextMeshProUGUI xpText, xpTextOnDead;
    int xpPoints;
    public bool gameIsPlaying, firstSecondarySpawned, startedSpawning, overrideCanonControl;
    bool gameIsOver = false;
    bool helpIsActive;


    public GameObject GameOverMenu;
    public GameObject PausedMenu, keySettings, help1, help2, activeHelp;

    AudioManagerScript audioManager;

    // Use this for initialization
    void Start () {
        xpText.text = "0 lumen";
        gameIsPlaying = true;
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManagerScript>();
        startedSpawning = false;
        ToggleHelp(help1);
    }
	
	// Update is called once per frame
	void Update () {
        if ((Input.GetButtonDown("Cancel") || Input.GetButtonDown("Pause")) & !gameIsOver)
        {
            TogglePauseGame();
        }

        if (Input.GetKey(KeyCode.F) & helpIsActive)
        {
            ToggleHelp(activeHelp);
            if (!startedSpawning)
            {
                startedSpawning = true;
            }
        }

        if (Input.GetButton("Cancel") & gameIsOver)
        {
            Debug.Log("QUITTING GAME");
            SceneManager.LoadScene(0);
        }

        if (firstSecondarySpawned)
        {
            ToggleHelp(help2);
            firstSecondarySpawned = false;
        }
    }

    public void AddXP(int value)
    {
        xpPoints += value;
        xpText.text = xpPoints + " lumen";
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER ...");
        gameIsPlaying = false;
        gameIsOver = true;
        GameOverMenu.SetActive(true);
        keySettings.SetActive(true);
        audioManager.Stop("LSSustain");
        xpTextOnDead.text = "You collected " + xpPoints + " LUMEN";

    }

    public void TogglePauseGame()
    {
        if (gameIsPlaying)
        {
            Debug.Log("Pausing Game...");
            gameIsPlaying = false;
            PausedMenu.SetActive(true);
            keySettings.SetActive(true);
        }
        else
        {
            Debug.Log("Unpausing Game...");
            gameIsPlaying = true;
            PausedMenu.SetActive(false);
            keySettings.SetActive(false);
        }
    }

    public void ToggleHelp(GameObject helpPrompt)
    {
        if (gameIsPlaying)
        {
            Debug.Log("Showing helpprompt");
            gameIsPlaying = false;
            overrideCanonControl = true;
            helpIsActive = true;
            activeHelp = helpPrompt;
            helpPrompt.SetActive(true);
        }
        else
        {
            Debug.Log("Hiding helpprompt");
            gameIsPlaying = true;
            overrideCanonControl = false;
            helpIsActive = false;
            helpPrompt.SetActive(false);
            activeHelp = null;
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadNewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerStats.Volume = volume;
    }
}
