

using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class AudioStatic : MonoBehaviour
{
    public static void AddSoundsToButtons(string soundPath, GameObject source)
    {
        var buttons = FindObjectsOfType<Button>(true);

        foreach (var button in buttons)
            button.onClick.AddListener(() => PlayAudio(Resources.Load<AudioClip>(soundPath), source));
    }
    
    private static void PlayAudio(AudioClip clip, GameObject source)
    {
        source.GetComponent<AudioSource>().PlayOneShot(clip, Account.SoundsVolume);
        Thread.Sleep((int)Math.Round(clip.length*1000));
    }
}
