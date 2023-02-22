using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float _lifetime = 1;
    private float _lifetimeCounter;
    [SerializeField] SoundEmitter _soundEmitter;

    private void OnEnable()
    {
        if (_soundEmitter != null)
        {
            _soundEmitter.PlaySound();
        }

        _lifetimeCounter = _lifetime;
        GameManager.instance.onGameEnd += SelfDestroy;
    }

    private void OnDisable()
    {
        GameManager.instance.onGameEnd -= SelfDestroy;
    }

    private void Update()
    {
        _lifetimeCounter -= Time.deltaTime;
        if (_lifetimeCounter <= 0)
            gameObject.SetActive(false);
    }

    private void SelfDestroy()
    {
        gameObject.SetActive(false);
    }
}
