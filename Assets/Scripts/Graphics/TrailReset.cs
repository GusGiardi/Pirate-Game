using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class TrailReset : MonoBehaviour
{
    private TrailRenderer _myTrail;

    private void Awake()
    {
        _myTrail = GetComponent<TrailRenderer>();
    }

    private void OnDisable()
    {
        _myTrail.Clear();
    }
}
