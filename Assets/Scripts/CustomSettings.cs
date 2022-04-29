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
        
        AudioStatic.AddSoundsToButtons(AudioStatic.Button, gameObject);
    }
    
    public void ChangeSoundVolume()
    {
        Account.SoundsVolume = SoundVolume.value;
    }
    
    public void ChangeMusicVolume()
    {
        Account.MusicVolume = SoundVolume.value;
    }
    
    public void Return()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
