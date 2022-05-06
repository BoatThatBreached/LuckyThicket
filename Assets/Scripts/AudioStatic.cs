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
    private static AudioSource SoundHandler;
    private static AudioSource MusicHandler;

    public class PointerEventsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            PlayAudio(Resources.Load<AudioClip>(Guidance), Account.SoundsVolume * 0.2F);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        
        }
    }

    public static void SetNewHandlers(GameObject source)
    {
        SoundHandler = source.GetComponents<AudioSource>()[0];
        
        MusicHandler = source.GetComponents<AudioSource>()[1];
        MusicHandler.volume = Account.MusicVolume;
        MusicHandler.loop = true;
    }

    public static void ChangeVolumes()
    {
        MusicHandler.volume = Account.MusicVolume;
        SoundHandler.volume = Account.SoundsVolume;
    }

    public static IEnumerator UpdateThemeTime()
    {
        while (true)
        {
            _mainThemeTime = MusicHandler.time;
            Thread.Sleep(5);
            yield return null;
        }
    }

    public static void AddMainTheme(MonoBehaviour scene, string soundPath)
    {
        MusicHandler.clip = Resources.Load<AudioClip>(soundPath);
        MusicHandler.Play();
        MusicHandler.time = _mainThemeTime;
        
        scene.StartCoroutine(UpdateThemeTime());
    }
    
    private static void RefreshSoundtrack()
    {
        ind = 0;
        soundtracks = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Assets\\Resources\\Sounds\\Soundtracks")
            .GetFiles()
            .Select(x => x.Name)
            .Where(x => !x.Contains("meta"))
            .OrderBy(emp => Guid.NewGuid())
            .ToList();
    }

    private static  void StartSoundtrack(MonoBehaviour scene) => scene.StartCoroutine(TracksCoroutine()); 
    public static IEnumerator TracksCoroutine()
    {
        RefreshSoundtrack();
        while (true)
        {
            MusicHandler.clip = Resources.Load<AudioClip>("Sounds\\Soundtracks\\" + soundtracks[ind]
                .Substring(0, soundtracks[ind].Length - 4));
            MusicHandler.Play();

            while (MusicHandler.isPlaying)
                yield return null;

            ind = (ind + 1) % soundtracks.Count;
        }
    }

    public static void AddSoundsToButtons(string soundPath, GameObject source)
    {
        var buttons = FindObjectsOfType<Button>(true);

        foreach (var button in buttons)
        {
            button.onClick.AddListener(() => PlayAudio(Resources.Load<AudioClip>(soundPath), Account.SoundsVolume));
            button.gameObject.AddComponent<PointerEventsController>();
        }
    }
    
    private static void PlayAudio(AudioClip clip, float volume)
    {
        SoundHandler.PlayOneShot(clip, volume);
        //Thread.Sleep((int)Math.Round(clip.length*1000));
    }

    public static void MenuInitSounds(MonoBehaviour scene, GameObject source)
    {
        SetNewHandlers(source);
        AddMainTheme(scene, MainTheme);
        AddSoundsToButtons(Click, source);
    }
    public static void GameInitSounds(MonoBehaviour scene, GameObject source)
    {
        SetNewHandlers(source);
        StartSoundtrack(scene);
        AddSoundsToButtons(Click, source);
    }
}