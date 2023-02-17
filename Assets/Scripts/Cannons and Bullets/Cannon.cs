using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _shootPoint;

    public void Fire()
    {
        ObjectPoolManager.instance.InstantiateInPool(_bulletPrefab, _shootPoint.position, _shootPoint.rotation);
    }
}
