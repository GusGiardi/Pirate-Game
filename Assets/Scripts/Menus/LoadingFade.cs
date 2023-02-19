using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingFade : MonoBehaviour
{
    [SerializeField] Image _fadeImage;
    private Material _fadeMaterial;

    [SerializeField] float _animationVelocity = 1;
    private bool _open = false;
    private float _currentOpenValue = 0;

    private void Awake()
    {
        _fadeMaterial = _fadeImage.material;
        _fadeMaterial.SetFloat("_ScreenWidth", Screen.width);
        _fadeMaterial.SetFloat("_ScreenHeight", Screen.height);
    }

    private void Update()
    {
        if (_open) 
        {
            if (_currentOpenValue < 1)
            {
                _currentOpenValue += Time.deltaTime * _animationVelocity;
            }
        }
        else 
        {
            if (_currentOpenValue > 0)
            {
                _currentOpenValue -= Time.deltaTime * _animationVelocity;
            }
        }

        _fadeMaterial.SetFloat("_Open", _currentOpenValue);
    }

    public void Open() 
    {
        _open = true;
    }

    public void Close()
    {
        _open = false;
    }
}
