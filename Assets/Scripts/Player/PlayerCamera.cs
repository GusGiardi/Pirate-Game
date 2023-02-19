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

    private void Awake()
    {
        _myTransform = transform;
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
}
