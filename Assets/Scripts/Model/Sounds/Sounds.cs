using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;
using Ship;

public static class Sounds {

    public static void PlaySoundGlobal(string path)
    {
        AudioSource audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        audioSource.volume = Options.SfxVolume;
        PlaySound(audioSource, path);
    }

    public static float PlayShipSound(string path, GenericShip ship = null)
    {
        if (ship == null) ship = Selection.ThisShip;
        if (ship == null) return 0;

        AudioSource audioSource = ship.Model.GetComponent<AudioSource>();
        return PlaySound(audioSource, path);
    }

    public static void PlayBombSound(GameObject bombObject, string path)
    {
        AudioSource audioSource = bombObject.transform.GetComponentInChildren<AudioSource>();
        PlaySound(audioSource, path);
    }

    private static float PlaySound(AudioSource audioSource, string path)
    {
        audioSource.volume = Options.SfxVolume;
        AudioClip audioClip = (AudioClip)Resources.Load("Sounds/" + path);
        audioSource.PlayOneShot(audioClip);

        return audioClip.length;
    }

    public static void PlayShots(string path, int times)
    {
        for (int i = 0; i < times; i++)
        {
            AudioSource audio = Selection.AnotherShip.Model.GetComponents<AudioSource>()[i];
            audio.volume = Options.SfxVolume;
            audio.clip = (AudioClip)Resources.Load("Sounds/" + path);
            audio.PlayDelayed(i * 0.5f);
        }
    }

    public static void PlayFly(Ship.GenericShip ship)
    {
        int soundsCount = ship.SoundFlyPaths.Count;
        int selectedIndex = Random.Range(0, soundsCount);

        PlayShipSound(ship.SoundFlyPaths[selectedIndex]);
    }

    public static void PlayFly()
    {
        PlayFly(Selection.ThisShip);
    }
}
