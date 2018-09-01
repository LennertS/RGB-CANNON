using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.PostProcessing;
using UnityEngine.Audio;

public class MainMenuScript : MonoBehaviour {

    public Slider volumeSlider;
    public GameObject highLight, qwertyBtn, azertyBtn;

    public AudioListener audioListener;
    
    void Start()
    {
        audioListener = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioListener>();
        SetVolume(PlayerStats.Volume);
        if (PlayerStats.UseQwerty)
        {
            SelectQwerty();
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }
    
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void SelectQwerty()
    {
        highLight.transform.position = qwertyBtn.transform.position;
        PlayerStats.UseQwerty = true;
    }

    public void SelectAzerty()
    {
        highLight.transform.position = azertyBtn.transform.position;
        PlayerStats.UseQwerty = false;
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerStats.Volume = volume;
        volumeSlider.value = volume;
    }
}
