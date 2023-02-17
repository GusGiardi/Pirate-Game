using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCanvas : MonoBehaviour
{
    private Transform _myTransform;

    private void Awake()
    {
        _myTransform = transform;
    }

    private void Update()
    {
        _myTransform.rotation = Quaternion.identity;
    }
}
