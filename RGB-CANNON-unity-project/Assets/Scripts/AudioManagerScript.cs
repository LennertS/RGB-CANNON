using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManagerScript : MonoBehaviour {
    public Sound[] sounds;
    public static AudioManagerScript instance;

    [System.Serializable]
    public class Sound
    {
        public string name;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;
        [Range(0.1f, 3f)]
        public float pitch;

        public bool loop;

        [HideInInspector]
        public AudioSource source;
    }
	// Use this for initialization
	void Awake () {
        if (instance == null) //if no audiomanager was initialized,
        {
            instance = this;    //set current audiomanager to this instance.
        }
        else    //if audiomanager was already initialized by previous scene,
        {
            Destroy(gameObject);    //destroy the newly initialized audiomanager.
            return;
        }

        //DontDestroyOnLoad(gameObject);    //prevents audio cuts when transitioning scenes;

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        //Play("Theme");
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s==null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        //Debug.Log("Started playing " + name);
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        //Debug.Log("Stopped playing " + name);
        s.source.Stop();
    }

    public void PlayDelayed(string delayedSound, string currentClip)
    {
        Sound s = Array.Find(sounds, sound => sound.name == delayedSound);
        Sound currentSound = Array.Find(sounds, sound => sound.name == currentClip);

        if (s == null)
        {
            Debug.LogWarning("Sound: " + delayedSound + " not found!");
            return;
        }

        if (currentSound == null)
        {
            Debug.LogWarning("Sound: " + currentClip + " not found!");
            return;
        }

        s.source.PlayDelayed(currentSound.clip.length);

    }
}
