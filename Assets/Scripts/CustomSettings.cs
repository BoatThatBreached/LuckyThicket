using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomSettings : MonoBehaviour
{
    // Start is called before the first frame update\
    private Slider SoundVolume;
    private Slider MusicVolume;
    
    public void Start()
    {
        var sliders = GameObject.FindObjectsOfType<Slider>();
        MusicVolume = sliders[0];
        SoundVolume = sliders[1];
        
        SoundVolume.value = Account.SoundsVolume;
        MusicVolume.value = Account.MusicVolume;
        
        AudioStatic.AddMainTheme(AudioStatic.MainTheme, gameObject);
        AudioStatic.AddSoundsToButtons(AudioStatic.Click, gameObject);
    }
    
    public void ChangeSoundVolume()
    {
        Account.SoundsVolume = SoundVolume.value;
    }
    
    public void ChangeMusicVolume()
    {
        AudioStatic.RememberThemeState(gameObject);
        AudioStatic.AddMainTheme(AudioStatic.MainTheme, gameObject);
        Account.MusicVolume = MusicVolume.value;
    }

    public void Return()
    {
        AudioStatic.RememberThemeState(gameObject);
        SceneManager.LoadScene("MenuScene");
    }
}
