using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AudioStatic : MonoBehaviour
{
    public const string MainTheme = "Sounds/main_theme";
    public static int ind = -1;
    private static List<string> soundtracks = new List<string>(); 
    private static float _mainThemeTime;
    public const string Click = "Sounds/click";
    private const string Guidance = "Sounds/guidance";
    private static GameObject _source;
    
    public class PointerEventsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            PlayAudio(Resources.Load<AudioClip>(Guidance), _source, Account.SoundsVolume * 0.2F);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        
        }
    }
    
    public static void RememberThemeState(GameObject source) {
        _mainThemeTime = source.GetComponents<AudioSource>()[1].time;
    }

    public static void AddMainTheme(string soundPath, GameObject source)
    {
        var sound = source.GetComponents<AudioSource>()[1];
        sound.loop = true;
        sound.clip = Resources.Load<AudioClip>(soundPath);
        sound.time = _mainThemeTime;
        sound.volume = Account.MusicVolume;
        sound.Play();
    }
    
    public static void RefreshSoundtrack()
    {
        ind = 0;
        soundtracks = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Assets\\Resources\\Sounds\\Soundtracks")
            .GetFiles()
            .Select(x => x.Name)
            .Where(x => !x.Contains("meta"))
            .OrderBy(emp => Guid.NewGuid())
            .ToList();
    }


    public static IEnumerator StartTracksCoroutine(GameObject source)
    {
        while (true)
        {
            var sound = source.GetComponents<AudioSource>()[1];
            sound.clip = Resources.Load<AudioClip>("Sounds\\Soundtracks\\" + soundtracks[ind]
                .Substring(0, soundtracks[ind].Length - 4));
            sound.volume = Account.MusicVolume;
            sound.Play();

            while (sound.isPlaying)
                yield return null;

            ind = (ind + 1) % soundtracks.Count;
        }
    }

    public static void AddSoundsToButtons(string soundPath, GameObject source)
    {
        var buttons = FindObjectsOfType<Button>(true);
        _source = source; 
        
        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => PlayAudio(Resources.Load<AudioClip>(soundPath), source, Account.SoundsVolume));
            button.gameObject.AddComponent<PointerEventsController>();
        }
    }
    
    private static void PlayAudio(AudioClip clip, GameObject source, float volume)
    {
        source.GetComponent<AudioSource>().PlayOneShot(clip, volume);
        Thread.Sleep((int)Math.Round(clip.length*1000));
    }
}