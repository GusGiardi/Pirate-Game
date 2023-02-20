using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    private Transform _myTransform;
    private Rigidbody2D _myRigidbody;

    [SerializeField] float _velocity;
    [SerializeField] float _lifetime;
    private float _lifetimeCounter;

    [SerializeField] float _power;

    private void Awake()
    {
        _myTransform = transform;
        _myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _lifetimeCounter = _lifetime;
        _myRigidbody.velocity = _myTransform.up * _velocity;
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
        {
            SelfDestroy();
        }
    }

    private void FixedUpdate()
    {
        _myRigidbody.velocity = _myTransform.up * _velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PirateShip ship = collision.collider.GetComponent<PirateShip>();
        if (ship != null)
        {
            ship.TakeDamage(_power);
        }
        SelfDestroy();
    }

    private void SelfDestroy()
    {
        gameObject.SetActive(false);
    }
}
