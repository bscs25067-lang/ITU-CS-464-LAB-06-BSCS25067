using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;


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
        _rb.AddForce(direction * speed, ForceMode.Force);
    }
}
