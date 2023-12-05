using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager inctece;
    void Awake()
    {

        {
            if (inctece == null)
                inctece = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound s in sounds)
        {
            s.sorce = gameObject.AddComponent<AudioSource>();
            s.sorce.clip = s.clip;

            s.sorce.volume = s.volium;
            s.sorce.pitch = s.pitch;
            s.sorce.loop = s.loop;
            s.sorce.playOnAwake = s.PlayOnAwake;
            if (s.sorce.playOnAwake)
                s.sorce.Play();
        }
    }
    public void play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.sorce.Play();
    }
    public void stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.sorce.Stop();
    }
    public void ChangeSounds(string CurrentSound,string NextSound )
    {
        Sound first = Array.Find(sounds, sound => sound.name == CurrentSound);
        Sound second = Array.Find(sounds, sound => sound.name == NextSound);
        StartCoroutine(Fade(first, second));
    }
    private IEnumerator Fade(Sound First, Sound Second)
    {
        Second.sorce.volume = 0;
        Second.sorce.Play();
        while (First.sorce.volume != 0 && Second.sorce.volume != Second.volium)
        {
            First.sorce.volume -= 0.2f * Time.deltaTime;
            Second.sorce.volume += 0.2f * Time.deltaTime;
            yield return null;
        }
        First.sorce.volume = First.volium;
        First.sorce.Pause();
    }
}

[System.Serializable]
public class Sound
{

    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]

    public float volium;
    [Range(.1f, 3f)]

    public float pitch;
    public bool loop;
    public bool PlayOnAwake;
    [HideInInspector]
    public AudioSource sorce;
}
