using UnityEngine;
using UnityEngine.InputSystem;


public class PlatformerController : MonoBehaviour
{

    [SerializeField, Range(0, 10)] private float _movementSpeed = 5.0f;
    [SerializeField, Range(0, 100)] private float _jumpForce = 5.0f;

    [SerializeField, Range(0, 3)] private float _gravityScaleNormal = 2f;
    [SerializeField, Range(0, 3)] private float _gravityScaleJumpUp = 1f;
    [SerializeField, Range(0, 3)] private float _gravityScaleJumpRelease = 1.5f;
    [SerializeField, Range(0, 1)] private float _airControl = 0.5f;

    private GroundSensor _groundSensor = null;


    //private Vector3 _moveInput = Vector3.zero;
    private Rigidbody2D _rigidBody2D = null;
    private InputAction _moveInputAction = null;
    private InputAction _smashInputAction = null;


    private Vector2 _moveInput = Vector2.zero;
    private bool _smashInput = false;


    // Start is called before the first frame update
    void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        PlayerInput playerInput = GetComponent<PlayerInput>();
        _moveInputAction = playerInput.actions.FindAction("Move");
        _smashInputAction = playerInput.actions.FindAction("Smash");
        _groundSensor = GetComponent<GroundSensor>();
    }

    private void FixedUpdate()
    {
        ReadInputs();
        UpdateHorizontalMovement();
        UpdateGravityScale();
    }

    private void UpdateGravityScale()
    {
        if (_rigidBody2D.velocity.y < 0)
        {
            _rigidBody2D.gravityScale = _gravityScaleNormal;
        }

        if (_rigidBody2D.velocity.y > 0 && !_smashInput)
        {
            _rigidBody2D.gravityScale = _gravityScaleJumpRelease;
        }
    }

    private void UpdateHorizontalMovement()
    {
        if (_groundSensor.IsGrounded())
            _rigidBody2D.velocity = new Vector2(_moveInput.x * _movementSpeed, _rigidBody2D.velocity.y);
        else
        {
            float speed = Mathf.Lerp(_rigidBody2D.velocity.x, _moveInput.x * _movementSpeed, _airControl);
            _rigidBody2D.velocity = new Vector2(speed, _rigidBody2D.velocity.y);
        }
    }

    private void ReadInputs()
    {
        _moveInput = _moveInputAction.ReadValue<Vector2>();
        _smashInput = _smashInputAction.ReadValue<float>() > 0.5f ? true : false;
    }

    private void OnSmash(InputValue value)
    {
        Jump();
    }

    private void Jump()
    {
        if (!_groundSensor.IsGrounded())
        {
            return;
        }

        _rigidBody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        _rigidBody2D.gravityScale = _gravityScaleJumpUp;
    }
}
