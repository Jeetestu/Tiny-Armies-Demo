using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    private static GameObject soundGameObject;
    /// <summary>
    /// Creates a 'sound object' to play the provided clip
    /// </summary>
    /// <param name="clip"></param>
    public static void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Sound Manager called with a null value clip");
            return;
        }
        if (soundGameObject == null)
            soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(clip);
    }

    public static void PlaySound(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null)
        {
            Debug.LogWarning("Sound Manager called with a null value clip");
            return;
        }
        GameObject oneShotSoundObject = new GameObject("Sound");
        oneShotSoundObject.transform.position = position;
        AudioSource audioSource = oneShotSoundObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        AudioSource.PlayClipAtPoint(clip, position);
        Destroy(oneShotSoundObject, audioSource.clip.length);
    }
}
