using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    public static AudioManager Instance;
    public AudioMixer mixer;

    void Awake()
    {
        {
            if (Instance == null)
                Instance = this;
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
    private void Start()
    {
        foreach (Sound s in sounds)
        {
           s.sorce.outputAudioMixerGroup = mixer.FindMatchingGroups(s.AudioType.ToString())[0];
        }
        if (GameManager.Instance.Music)
            mixer.SetFloat("MusicVolume", 0);
        else
            mixer.SetFloat("MusicVolume", -80);

        if (GameManager.Instance.Sound)
            mixer.SetFloat("SfxVolume", 0);
        else
            mixer.SetFloat("SfxVolume", -80);
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.sorce.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.sorce.Stop();
    }

    public void ChangeSounds(string currentSound, string nextSound)
    {
        Sound first = Array.Find(sounds, sound => sound.name == currentSound);
        Sound second = Array.Find(sounds, sound => sound.name == nextSound);
        StartCoroutine(Fade(first, second));
    }

    private IEnumerator Fade(Sound first, Sound second)
    {
        second.sorce.volume = 0;
        second.sorce.Play();
        while (first.sorce.volume != 0 && second.sorce.volume != second.volium)
        {
            first.sorce.volume -= 0.2f * Time.deltaTime;
            second.sorce.volume += 0.2f * Time.deltaTime;
            yield return null;
        }

        first.sorce.volume = first.volium;
        first.sorce.Pause();
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volium;
    [Range(.1f, 3f)] public float pitch;
    public bool loop;
    public bool PlayOnAwake;
    public SuondTypesOptions AudioType; // Music or Sfx
    [HideInInspector] public AudioSource sorce;
}
public enum SuondTypesOptions
{
    Music,
    Sfx
}