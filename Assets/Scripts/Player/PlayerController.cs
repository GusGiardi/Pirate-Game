using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PirateShip))]
public class PlayerController : MonoBehaviour
{
    private PirateShip _myPirateShip;

    private void Awake()
    {
        _myPirateShip = GetComponent<PirateShip>();
    }

    private void Update()
    {
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
}
