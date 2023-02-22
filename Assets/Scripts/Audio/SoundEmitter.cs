using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEmitter : MonoBehaviour
{
    private AudioSource _audioSource;
    [SerializeField] AudioClip[] _clips;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
        }

        _audioSource.clip = _clips[Random.Range(0, _clips.Length)];
        _audioSource.Play();
    }
}
