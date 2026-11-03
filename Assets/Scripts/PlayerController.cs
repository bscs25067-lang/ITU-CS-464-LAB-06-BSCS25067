using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;

    [Header("Boost Settings")]
    [SerializeField] private float boostMultipler = 1;
    [SerializeField] private float boostDuration = 5;

    private bool _isBoostActive;
    private Coroutine _activeBoost;
    private Rigidbody _rb;
    private Vector2 _input;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        _input = Vector2.ClampMagnitude(_input, 1f);
    }

    private void FixedUpdate()
    {
        Vector3 direction = new Vector3(_input.x, 0, _input.y);

        float multiplier = _isBoostActive ? boostMultipler : 1;
        _rb.AddForce(direction * speed * multiplier, ForceMode.Force);
    }

    public void BoostSpeed()
    {
        if (_activeBoost != null) StopCoroutine(_activeBoost);

        _activeBoost = StartCoroutine(ApplySpeedMultipler());
    }

    IEnumerator ApplySpeedMultipler()
    {
        _isBoostActive = true;
        yield return new WaitForSeconds(boostDuration);
        _isBoostActive = false;
    }
}
