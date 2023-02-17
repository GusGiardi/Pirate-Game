using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager instance => _instance;

    [SerializeField] Transform _playerTransform;

    public Transform playerTransform => _playerTransform;

    private void Awake()
    {
        _instance = this;
    }

}
