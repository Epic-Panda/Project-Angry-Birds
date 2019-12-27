using UnityEngine;
using UnityEngine.Audio;
using System;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] sounds;

    bool mute = false;

    void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;

            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    // Use this for initialization
    void Start()
    {
        Play("background music");
        DontDestroyOnLoad(gameObject);
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s != null)
            s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);

        if (s != null)
            s.source.Stop();
    }

    public void ToogleSound()
    {
        mute = !mute;

        foreach (Sound s in sounds)
            if (mute)
                s.source.volume = 0;
            else
                s.source.volume = s.volume;
    }
}
