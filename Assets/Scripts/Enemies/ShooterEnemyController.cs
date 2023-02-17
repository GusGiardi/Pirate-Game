using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemyController : EnemyController
{
    private enum Behaviour
    {
        FollowPlayer,
        Attack,
        Retreat
    }
    private Behaviour _currentBehaviour;

    [Header("Follow Player Behaviour Properties")]
    [SerializeField] float _minDistanceToFollowPlayer = 3f;
    [SerializeField] int _updatePathRate = 10;

    [Header("Attack Behaviour Properties")]
    [SerializeField] float _minDistanceToRetreat = 1.5f;
    [SerializeField] float _maxDistanceToFollowPlayer = 6f;
    [SerializeField] float _minDirectionDotProdToAccelerate = 0.3f;
    [SerializeField] float _maxAngleToShoot = 10f;
    [SerializeField] CannonRaycast[] _cannonRaycasts;
    [System.Serializable]
    private class CannonRaycast
    {
        [SerializeField] Cannon _cannon;
        [SerializeField] int _cannonGroupIndex;

        public int cannonGroupIndex => _cannonGroupIndex;

        public float GetCannonAngleToPlayer(Vector2 playerPosition)
        {
            Vector2 cannonPosition = _cannon.transform.position;
            return Vector2.SignedAngle(
                _cannon.transform.right,
                playerPosition - cannonPosition);
        }
    }

    [Header("Retreat Behaviour Properties")]
    [SerializeField] float _distanceToAttack = 3f;
    [SerializeField] float _retreatDistance = 3f;

    private void OnEnable()
    {
        _currentBehaviour = Behaviour.FollowPlayer;
    }

    protected override void Update()
    {
        if (!_myPirateShip.alive)
        {
            return;
        }

        switch (_currentBehaviour)
        {
            case Behaviour.FollowPlayer:
                FollowPlayerUpdate();
                break;
            case Behaviour.Attack:
                AttackUpdate();
                break;
            case Behaviour.Retreat:
                RetreatUpdate();
                break;
        }
    }

    private void FollowPlayerUpdate()
    {
        Vector2 playerPosition = GameManager.instance.playerTransform.position;
        if (Time.frameCount % _updatePathRate == 0)
        {
            CreatePath(playerPosition);
        }
        FollowCurrentPath();

        if (Vector2.Distance(_myTransform.position, playerPosition) <= _minDistanceToFollowPlayer)
        {
            _currentBehaviour = Behaviour.Attack;
        }
    }

    private void AttackUpdate()
    {
        Vector2 playerPosition = GameManager.instance.playerTransform.position;

        // Aim Player
        float angleToAim = GetMinimumTotationToAimPlayer(playerPosition, out int cannonGroupIndex);
        if (Mathf.Abs(angleToAim) <= _maxAngleToShoot)
        {
            _myPirateShip.Fire(cannonGroupIndex);
        }
        else
        {
            _myPirateShip.Rotate(Mathf.Sign(angleToAim) < 0);
        }

        // Move
        Vector2 myPosition = _myTransform.position;
        Vector2 playerDir = (playerPosition - myPosition).normalized;
        float playerDirectionDotProd = Vector2.Dot(-_myTransform.up, playerDir);
        if (playerDirectionDotProd < _minDirectionDotProdToAccelerate)
        {
            _myPirateShip.Accelerate();
        }

        // Change Behaviour
        float distanceToPlayer = Vector2.Distance(myPosition, playerPosition);
        if (distanceToPlayer <= _minDistanceToRetreat)
        {
            _currentBehaviour = Behaviour.Retreat;
        }
        else if (distanceToPlayer <= _maxDistanceToFollowPlayer)
        {
            _currentBehaviour = Behaviour.FollowPlayer;
        }
    }

    private float GetMinimumTotationToAimPlayer(Vector2 playerPosition, out int cannonGroupIndex)
    {
        cannonGroupIndex = 0;
        float minimumAngleToAimPlayer = Mathf.Infinity;
        for (int i = 0; i < _cannonRaycasts.Length; i++)
        {
            float angleToAim = _cannonRaycasts[i].GetCannonAngleToPlayer(playerPosition);
            if (Mathf.Abs(angleToAim) < Mathf.Abs(minimumAngleToAimPlayer))
            {
                minimumAngleToAimPlayer = angleToAim;
                cannonGroupIndex = _cannonRaycasts[i].cannonGroupIndex;
            }
        }

        return minimumAngleToAimPlayer;
    }

    private void RetreatUpdate()
    {
        Vector2 playerPosition = GameManager.instance.playerTransform.position;
        if (Time.frameCount % _updatePathRate == 0)
        {
            Vector2 myPosition = _myTransform.position;
            Vector2 playerDir = (playerPosition - myPosition).normalized;
            CreatePath(myPosition - playerDir * _retreatDistance);
        }
        FollowCurrentPath();

        if (Vector2.Distance(_myTransform.position, playerPosition) >= _distanceToAttack)
        {
            _currentBehaviour = Behaviour.Attack;
        }
    }
}
