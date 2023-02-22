using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _shootPoint;
    [SerializeField] SoundEmitter _soundEmitter;
    [SerializeField] GameObject _fireEffect;

    public void Fire()
    {
        ObjectPoolManager.instance.InstantiateInPool(_bulletPrefab, _shootPoint.position, _shootPoint.rotation);

        GameObject fireEffect = ObjectPoolManager.instance.InstantiateInPool(_fireEffect, _shootPoint.position, _shootPoint.rotation);
        fireEffect.transform.SetParent(_shootPoint, true);

        if (_soundEmitter != null)
        {
            _soundEmitter.PlaySound();
        }
    }
}
