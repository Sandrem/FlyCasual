using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Players;

public static class Sounds {

    public static void PlaySoundOnce(string path)
    {
        AudioSource audio = Selection.ThisShip.Model.GetComponent<AudioSource>();
        audio.volume = Options.SfxVolume * 1f / 5f;
        audio.PlayOneShot((AudioClip)Resources.Load("Sounds/" + path));
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

        PlaySoundOnce(Selection.ThisShip.SoundFlyPaths[selectedIndex]);
    }

}
