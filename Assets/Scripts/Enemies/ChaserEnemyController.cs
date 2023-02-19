using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserEnemyController : EnemyController
{
    [Header("Path Update")]
    [SerializeField] int _updatePathRate = 10;

    [Header("Collision")]
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] float _collisionPower;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();

        if (!_myPirateShip.alive)
        {
            return;
        }

        if (Time.frameCount % _updatePathRate == 0)
        {
            CreatePath(GameManager.instance.playerTransform.position);
        }
        FollowCurrentPath();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_myPirateShip.alive)
            return;

        if ((_playerLayer & 1 << collision.gameObject.layer) == 0)
            return;

        PirateShip ship = collision.collider.GetComponent<PirateShip>();
        if (ship != null)
        {
            ship.TakeDamage(_collisionPower);
        }
        SelfDestroy();
    }

    private void SelfDestroy()
    {
        _myPirateShip.TakeDamage(_myPirateShip.maxHealth);
    }
}
