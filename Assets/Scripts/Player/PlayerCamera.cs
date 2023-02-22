using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    private Transform _myTransform;
    [SerializeField] Camera _camera;
    public new Camera camera => _camera;

    private Vector2 _currentVelocity = Vector2.zero;
    [SerializeField] float _smoothTime;

    [SerializeField] AnimationCurve _screenShakeCurve;
    private float _screenShakeTime = 0;
    private float _screenShakeIntensity = 0;
    private float _currentScreenShakeIntensity = 0;

    private void Awake()
    {
        _myTransform = transform;
    }

    private void OnEnable()
    {
        GameManager.instance.onGameEnd += StopScreenShake;
    }

    private void OnDisable()
    {
        GameManager.instance.onGameEnd -= StopScreenShake;
    }

    private void FixedUpdate()
    {
        _myTransform.position = Vector2.SmoothDamp(
            _myTransform.position,
            GameManager.instance.playerTransform.position,
            ref _currentVelocity,
            _smoothTime);

        float cameraHeight = _camera.orthographicSize;
        float cameraWidth = (_camera.orthographicSize * Screen.width) / Screen.height ;

        float xPos = Mathf.Clamp(
            _myTransform.position.x,
            cameraWidth + GameManager.instance.mapLimitsBorderSize,
            GameManager.instance.mapGenerator.mapSize - GameManager.instance.mapLimitsBorderSize - cameraWidth);
        float yPos = Mathf.Clamp(
            _myTransform.position.y,
            cameraHeight + GameManager.instance.mapLimitsBorderSize,
            GameManager.instance.mapGenerator.mapSize - GameManager.instance.mapLimitsBorderSize - cameraHeight);
        _myTransform.position = new Vector2(xPos, yPos);
    }

    private void Update()
    {
        UpdateScreenShake();
    }

    public void ScreenShake(float time, float intensity)
    {
        _screenShakeTime = Mathf.Max(_screenShakeTime, time);
        _screenShakeIntensity = Mathf.Max(_currentScreenShakeIntensity, intensity);
    }

    private void UpdateScreenShake()
    {
        if (_screenShakeTime <= 0)
            return;

        _screenShakeTime -= Time.deltaTime;
        float graphTime = _screenShakeCurve.keys[_screenShakeCurve.length - 1].time;
        float currentTime = Mathf.Clamp(graphTime - _screenShakeTime, 0, graphTime);
        _currentScreenShakeIntensity = _screenShakeCurve.Evaluate(currentTime) * _screenShakeIntensity;

        _camera.transform.localPosition = new Vector3(Random.value * _currentScreenShakeIntensity, Random.value * _currentScreenShakeIntensity, _camera.transform.localPosition.z);
    }

    public void StopScreenShake()
    {
        _screenShakeTime = 0;
        _screenShakeIntensity = 0;
        _camera.transform.localPosition = new Vector3(0, 0, _camera.transform.localPosition.z);
    }
}
