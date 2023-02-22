using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager instance => _instance;

    private AudioSource _audioSource;
    private AudioClip _currentClip;

    [SerializeField] AnimationCurve _fadeOutCurve;
    private float _fadeTimeCounter = 0;
    private bool _fadeOut = false;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        UpdateMusicChange();
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == _currentClip)
            return;

        _currentClip = musicClip;
        if (_audioSource.clip == null)
        {
            _audioSource.clip = _currentClip;
            _audioSource.Play();
            return;
        }
        _fadeOut = true;
    }

    private void UpdateMusicChange()
    {
        float fadeCurveTime = _fadeOutCurve.keys[_fadeOutCurve.length - 1].time;
        if (_fadeOut)
        {
            _fadeTimeCounter += Time.unscaledDeltaTime;
            _audioSource.volume = _fadeOutCurve.Evaluate(Mathf.Clamp(_fadeTimeCounter, 0, fadeCurveTime));
            if (_fadeTimeCounter >= fadeCurveTime)
            {
                _fadeOut = false;
                _audioSource.clip = _currentClip;
                if (_currentClip != null)
                {
                    _audioSource.Play();
                }
                else
                {
                    _audioSource.Stop();
                }
            }
        }
        else if (_fadeTimeCounter > 0)
        {
            _fadeTimeCounter -= Time.unscaledDeltaTime;
            _audioSource.volume = _fadeOutCurve.Evaluate(Mathf.Clamp(_fadeTimeCounter, 0, fadeCurveTime));
        }
    }
}
