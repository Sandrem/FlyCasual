using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;

public static class Sounds {

    public static void PlayShipSound(string path)
    {
        AudioSource audioSource = Selection.ThisShip.Model.GetComponent<AudioSource>();
        PlaySound(audioSource, path);
    }

    public static void PlayBombSound(GameObject bombObject, string path)
    {
        AudioSource audioSource = bombObject.transform.GetComponentInChildren<AudioSource>();
        PlaySound(audioSource, path);
    }

    private static void PlaySound(AudioSource audioSource, string path)
    {
        audioSource.volume = Options.SfxVolume * 1f / 5f;
        audioSource.PlayOneShot((AudioClip)Resources.Load("Sounds/" + path));
    }

    public static void PlayShots(string path, int times)
    {
        for (int i = 0; i < times; i++)
        {
            AudioSource audio = Selection.AnotherShip.Model.GetComponents<AudioSource>()[i];
            audio.volume = Options.SfxVolume * 1f / 5f;
            audio.clip = (AudioClip)Resources.Load("Sounds/" + path);
            audio.PlayDelayed(i * 0.5f);
        }
    }

    public static void PlayFly()
    {
        int soundsCount = Selection.ThisShip.SoundFlyPaths.Count;
        int selectedIndex = Random.Range(0, soundsCount);

        PlayShipSound(Selection.ThisShip.SoundFlyPaths[selectedIndex]);
    }

}
