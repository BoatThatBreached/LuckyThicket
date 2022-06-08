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

        AudioStatic.MenuInitSounds(this, gameObject);
        
        SoundVolume.value = Account.SoundsVolume;
        MusicVolume.value = Account.MusicVolume;
    }
    
    public void ChangeSoundVolume()
    {
        Account.SoundsVolume = SoundVolume.value;
        AudioStatic.ChangeVolumes();
    }
    
    public void ChangeMusicVolume()
    {
        Account.MusicVolume = MusicVolume.value;
        AudioStatic.ChangeVolumes();
    }

    public void Return() => SceneManager.LoadScene("MenuScene");
    
}
