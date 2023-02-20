using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PirateShip))]
public class EnemyController : MonoBehaviour
{
    protected Transform _myTransform;
    protected PirateShip _myPirateShip;

    [Header("Navigation")]
    [SerializeField] NavMeshAgent _navMeshAgent;
    [SerializeField] float _navigationStopDistance = 1;
    [SerializeField] float _minAngleToRotate = 10;

    [Header("Deactivation")]
    [SerializeField] float _deactivationTime = 2;
    protected float _deactivationTimeCounter;

    [Header("Score")]
    [SerializeField] int _killScore = 50;

    protected virtual void Awake()
    {
        _myTransform = transform;
        _myPirateShip = GetComponent<PirateShip>();
    }

    protected virtual void OnEnable() 
    {
        _deactivationTimeCounter = _deactivationTime;
        AdjustNavMeshAgentPosition();
        _navMeshAgent.gameObject.SetActive(true);
    }

    protected virtual void OnDisable() 
    {
        _navMeshAgent.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if (!_myPirateShip.alive) 
        {
            _deactivationTimeCounter -= Time.deltaTime;
            if (_deactivationTimeCounter <= 0) 
            {
                EnemySpawner.instance.RemoveEnemy(gameObject);
            }
        }

        if (_navMeshAgent.hasPath) 
        {
            for(int i = 0; i < _navMeshAgent.path.corners.Length - 1; i++) 
            {
                Debug.DrawLine(_navMeshAgent.path.corners[i], _navMeshAgent.path.corners[i + 1], Color.white);
            }
        }
    }

    protected void AdjustNavMeshAgentPosition()
    {
        Vector3 myPositionInNavMesh = new Vector3(_myTransform.position.x, 0, _myTransform.position.y);

        bool pointFound = NavMesh.SamplePosition(myPositionInNavMesh, out NavMeshHit hit, 10, NavMesh.AllAreas);
        if (pointFound)
        {
            _navMeshAgent.Warp(hit.position);
        }
    }

    protected bool CreatePath(Vector2 destinationPoint)
    {
        AdjustNavMeshAgentPosition();

        if (!_navMeshAgent.isOnNavMesh)
            return false;

        Vector3 destinationInNavMesh = new Vector3(destinationPoint.x, 0, destinationPoint.y);

        bool pointFound = NavMesh.SamplePosition(destinationInNavMesh, out NavMeshHit hit, 10, NavMesh.AllAreas);
        if (!pointFound)
            return false;

        Vector3 destination = hit.position;
        NavMeshPath path = new NavMeshPath();
        bool hasPath = _navMeshAgent.CalculatePath(destination, path);
        if (!hasPath)
            return false;

        _navMeshAgent.SetPath(path);
        return true;
    }

    protected void FollowCurrentPath()
    {
        if (_navMeshAgent.hasPath)
        {
            if (_navMeshAgent.path.corners.Length >= 2)
            {
                Vector2 pointToGo = new Vector2(
                    _navMeshAgent.path.corners[1].x, 
                    _navMeshAgent.path.corners[1].z
                    );
                MoveToPoint(pointToGo);
            }
            if (_navMeshAgent.remainingDistance <= _navigationStopDistance)
            {
                CancelCurrentPath();
            }
        }
    }

    protected void CancelCurrentPath()
    {
        if (!_navMeshAgent.isOnNavMesh)
            return;
        _navMeshAgent.ResetPath();
    }

    protected void MoveToPoint(Vector2 point)
    {
        Vector2 currentPosition = _myTransform.position;
        Vector2 directionToGo = (point - currentPosition).normalized;
        float directionAngle = Vector2.SignedAngle(-_myTransform.up, directionToGo);
        if (Mathf.Abs(directionAngle) > _minAngleToRotate)
        {
            _myPirateShip.Rotate(Mathf.Sign(directionAngle) < 0);
        }

        _myPirateShip.Accelerate();
    }

    public void AddScore()
    {
        GameManager.instance.AddScore(_killScore);
    }
}
