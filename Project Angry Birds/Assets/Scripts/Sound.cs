﻿using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip audioClip;

    [Range(0, 1)]
    public float volume = 1;
    public bool loop = false;

    [HideInInspector]
    public AudioSource source;
}
