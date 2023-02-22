using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PirateShip))]
public class PlayerController : MonoBehaviour
{
    private PirateShip _myPirateShip;

    [SerializeField] float _minDistanceToMapBorder = 1;

    private void Awake()
    {
        _myPirateShip = GetComponent<PirateShip>();
    }

    private void OnEnable()
    {
        MiniMapManager.instance.TrackTransform(transform, true);
    }

    private void OnDisable()
    {
        MiniMapManager.instance.RemoveTrackedTransform(transform);
    }

    private void Update()
    {
        StayInScreenLimits();

        if (!_myPirateShip.alive)
            return;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _myPirateShip.Rotate(false);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            _myPirateShip.Rotate(true);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            _myPirateShip.Accelerate();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            _myPirateShip.Fire(0);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            _myPirateShip.Fire(1);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            _myPirateShip.Fire(2);
        }
    }

    private void StayInScreenLimits() 
    {
        float xPos = Mathf.Clamp(
            transform.position.x,
            GameManager.instance.mapLimitsBorderSize + _minDistanceToMapBorder,
            GameManager.instance.mapGenerator.mapSize - GameManager.instance.mapLimitsBorderSize - _minDistanceToMapBorder);
        float yPos = Mathf.Clamp(
            transform.position.y,
            GameManager.instance.mapLimitsBorderSize + _minDistanceToMapBorder,
            GameManager.instance.mapGenerator.mapSize - GameManager.instance.mapLimitsBorderSize - _minDistanceToMapBorder);

        transform.position = new Vector2(xPos, yPos);
    }
}
