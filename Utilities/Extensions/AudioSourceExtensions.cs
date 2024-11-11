using UnityEngine;

namespace CoolTools.Utilities
{
    public static class AudioSourceExtensions
    {
        public static void Play(this AudioSource audioSource, AudioClip clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        
        // public static void InstantiateAndPlay(this AudioSource audioSource, AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, float ttl = 1f)
        // {
        //     var audio = Object.Instantiate(audioSource, position, Quaternion.identity);
        //     audio.Play(clip, volume, pitch);
        // }
    }
}