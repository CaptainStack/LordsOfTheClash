using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound 
{
    public string name;
    public AudioClip clip;

    [HideInInspector]
    public AudioSource source;
    public bool loop = false;
    public AudioMixerGroup mixerGroup;

    public void SetSource(AudioSource source)
    {
        this.source = source;
        this.source.clip = clip;
    }

    public void PlaySound()
    {
        source.Play();
    }
    public void StopSound()
    {
        source.Stop();
    }
}
