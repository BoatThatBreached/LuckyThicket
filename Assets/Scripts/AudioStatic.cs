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
    public const string Click = "Sounds/wood_click";
    private const string Guidance = "Sounds/guidance";
    private static AudioSource SoundHandler;
    private static AudioSource MusicHandler;

    public static Dictionary<Basis, Dictionary<Tribes, Action>> sounds =
        new Dictionary<Basis, Dictionary<Tribes, Action>>
        {
            {
                Basis.Build, new Dictionary<Tribes, Action>
                {
                    { Tribes.Beaver, () => PlayAudio("Sounds/cell_build") },
                    { Tribes.Magpie, () => PlayAudio("Sounds/cell_build") }
                }
            },
            {
                Basis.Destroy, new Dictionary<Tribes, Action>
                {
                    { Tribes.Beaver, () => PlayAudio("Sounds/cell_destroy") },
                    { Tribes.Magpie, () => PlayAudio("Sounds/cell_destroy") }
                }
            },
            {
                Basis.Kill, new Dictionary<Tribes, Action>
                {
                    { Tribes.Beaver, () => PlayAudio("Sounds/beaver_kill") },
                    { Tribes.Magpie, () => PlayAudio("Sounds/magpie_kill") },
                    { Tribes.Obstacle, () => PlayAudio("Sounds/rustle") },
                }
            },
            {
                Basis.Spawn, new Dictionary<Tribes, Action>
                {
                    { Tribes.Beaver, () => PlayAudio("Sounds/click") },
                    { Tribes.Magpie, () => PlayAudio("Sounds/click") },
                    { Tribes.Obstacle, () => PlayAudio("Sounds/rustle") }
                }
            },
            {
                Basis.Pull, new Dictionary<Tribes, Action>
                {
                    { Tribes.Beaver, () => PlayAudio("Sounds/meat_hook") },
                    { Tribes.Magpie, () => PlayAudio("Sounds/meat_hook") },
                }
            },
            {
                Basis.Push, new Dictionary<Tribes, Action>
                {
                    { Tribes.Beaver, () => PlayAudio("Sounds/punch") },
                    { Tribes.Magpie, () => PlayAudio("Sounds/punch") },
                }
            },
            /*{
                Basis.Select, new Dictionary<Tribes, Action>
                {
                    { Tribes.Beaver, () => PlayAudio("Sounds/click") },
                    { Tribes.Magpie, () => PlayAudio("Sounds/click") }
                }
            },*/
        };

        
    public static void PlaySound(Basis basis, Tribes tribe)
    {
        if (sounds.ContainsKey(basis))
            if (sounds[basis].ContainsKey(tribe))
                sounds[basis][tribe]();
    }


    public class PointerEventsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            PlayAudio(Guidance);
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
        MusicHandler.loop = false;
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
            MusicHandler.Play();

            while (MusicHandler.isPlaying)
            {
                _mainThemeTime = MusicHandler.time;
                yield return null;
            }

            MusicHandler.time = 0;
        }
    }

    public static void AddMainTheme(MonoBehaviour scene, string soundPath)
    {
        MusicHandler.clip = Resources.Load<AudioClip>(soundPath);
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
            button.onClick.AddListener(() => PlayAudio(soundPath));
            button.gameObject.AddComponent<PointerEventsController>();
        }
    }

    public static void PlayAudio(string path)
    {
        var clip = Resources.Load<AudioClip>(path);
        SoundHandler.PlayOneShot(clip, Account.SoundsVolume);
        Thread.Sleep((int)Math.Round(clip.length*500));
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