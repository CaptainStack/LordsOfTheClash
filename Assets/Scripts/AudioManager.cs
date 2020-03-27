using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public string sceneMusic;
    [SerializeField]
    public Sound[] sounds;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.loop = sound.loop;
            sound.source.outputAudioMixerGroup = sound.mixerGroup;
        }
    }

    void Start()
    {
        PlaySound(sceneMusic);
    }

    public void PlaySound(string _name)
    {
        Sound sound = Array.Find(sounds, s => s.name == _name);
        sound.PlaySound();
    }

    public void ChangeMusic(string _name)
    {
        foreach (Sound sound in sounds)
        {
            if (sound.mixerGroup.name == "Music") 
            {
                sound.StopSound();
            }
        }
        PlaySound(_name);
    }

     public static AudioManager GetInstance() 
     {
        return instance;
     }
}
