using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MultiAudioSource : MonoBehaviour
{
    private AudioSource source;
    public AudioClip audioClip;
    public AudioMixerGroup output;
    public bool playOnAwake = false;
    public bool loop = false;
    [Range(0,1)] public float volume = 1;
    public float maxDistance = 500f;

    private float closestRange;

    private void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();

        source.spatialBlend = 0;
        source.clip = audioClip;
        source.outputAudioMixerGroup = output;
        source.playOnAwake = playOnAwake;
        source.loop = loop;
    }

    private void Update()
    {
        if (source.isPlaying)
        {
            closestRange = DistanceTo(PlayerController.AllPlayers[0].transform.position);
            foreach(PlayerController player in PlayerController.AllPlayers)
            {
                float newDist = DistanceTo(player.transform.position);
                if (newDist < closestRange)
                    closestRange = newDist;
            }
            source.volume = Mathf.Clamp01((maxDistance - closestRange) / maxDistance) * volume;
        }
    }

    public void PlayOneShot(AudioClip clip)
    {
        source.PlayOneShot(clip);
    }

    public void PlayOneShot(AudioClip clip, float volume)
    {
        this.volume = volume;
        source.PlayOneShot(clip);
    }

    private float DistanceTo(Vector3 player)
    {
        return Vector3.Distance(transform.position, player);
    }


}
