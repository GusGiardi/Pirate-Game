using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]

public class PirateShip : MonoBehaviour
{
    private Transform _myTransform;
    private Rigidbody2D _myRigidbody;

    [Header("Movement")]
    [SerializeField] float _movementVelocity = 1.5f;
    [SerializeField] float _rotationVelocity = 0.2f;

    [Header("Health")]
    [SerializeField] float _maxHealth = 100;
    [SerializeField] float _currentHealth = 100;
    [SerializeField] Transform _healthBarImage;
    [SerializeField] ShipPart[] _shipParts;
    public bool alive => _currentHealth > 0;
    public float maxHealth => _maxHealth;

    [System.Serializable]
    private class ShipPart
    {
        [SerializeField] SpriteRenderer _renderer;
        [SerializeField] Sprite[] _damageSprites;

        public void UpdateSprite(float healthPercentage)
        {
            int spriteIndex = Mathf.CeilToInt(healthPercentage * _damageSprites.Length);
            _renderer.sprite = _damageSprites[Mathf.Min(spriteIndex, _damageSprites.Length - 1)];
        }
    }

    [Header("Attack")]
    [SerializeField] CannonGroup[] _cannonGroups;

    [System.Serializable]
    private class CannonGroup
    {
        [SerializeField] string _groupName;
        [SerializeField] Cannon[] _cannons;
        [SerializeField] float _cooldown;
        private float _cooldownCounter;

        public void Fire()
        {
            if (_cooldownCounter > 0)
                return;

            foreach (Cannon c in _cannons)
            {
                c.Fire();
            }
            _cooldownCounter = _cooldown;
        }

        public void UpdateCooldown()
        {
            if (_cooldownCounter <= 0)
                return;

            _cooldownCounter -= Time.deltaTime;
        }
    }

    private void Awake()
    {
        _myTransform = transform;
        _myRigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _currentHealth = _maxHealth;
        UpdateShipParts();
    }

    private void Update()
    {
        foreach (CannonGroup cannonGroup in _cannonGroups)
        {
            cannonGroup.UpdateCooldown();
        }
    }

    public void Accelerate()
    {
        _myRigidbody.AddForce(-_myTransform.up * _movementVelocity * Time.deltaTime);
    }

    public void Rotate(bool clockwise)
    {
        float rotationVel = _rotationVelocity * (clockwise ? -1 : 1);
        _myRigidbody.AddTorque(rotationVel * Time.deltaTime);
    }

    public void Fire(int cannonGroupIndex)
    {
        if (cannonGroupIndex >= _cannonGroups.Length)
            return;
        _cannonGroups[cannonGroupIndex].Fire();
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxHealth);

        UpdateShipParts();
    }

    private void UpdateShipParts()
    {
        float healthPercentage = _currentHealth / _maxHealth;
        _healthBarImage.localScale = new Vector3(
            healthPercentage,
            1, 1);
        foreach (ShipPart part in _shipParts)
        {
            part.UpdateSprite(healthPercentage);
        }
    }
}
