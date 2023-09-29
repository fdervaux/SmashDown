using System.Linq;
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

    [SerializeField, Range(0, 1)] private float _groundControl = 1f;

    [SerializeField] private float impactForceDrag = 0.2f;

    private GroundSensor _groundSensor = null;


    //private Vector3 _moveInput = Vector3.zero;
    private Rigidbody2D _rigidBody2D = null;
    private InputAction _moveInputAction = null;
    private InputAction _smashInputAction = null;

    private float _horizontalMovement = 0f;


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

        Vector2 momemtum = _rigidBody2D.velocity - new Vector2(_horizontalMovement, 0);

        momemtum.x = Mathf.Lerp(momemtum.x, 0, impactForceDrag);
        

        UpdateHorizontalMovement();
        UpdateGravityScale();

        //_rigidBody2D.velocity = new Vector2(_horizontalMovement, _rigidBody2D.velocity.y);

        _rigidBody2D.velocity = momemtum + new Vector2(_horizontalMovement, 0);

        RaycastHit2D[] hitResults = new RaycastHit2D[16];

        int nbResults = _rigidBody2D.Cast(
            Vector2.right * Mathf.Sign(_horizontalMovement),
            hitResults,
            Mathf.Abs(_horizontalMovement) * Time.fixedDeltaTime
            );

        if (nbResults > 0)
        {
            _horizontalMovement = 0;
        }


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
        float friction = _groundSensor.IsGrounded() ? _groundControl : _airControl;
        float speed = Mathf.Lerp(0, _moveInput.x * _movementSpeed, friction);
        _horizontalMovement = speed;


    }

    private void ReadInputs()
    {
        _moveInput = _moveInputAction.ReadValue<Vector2>();
        _smashInput = _smashInputAction.ReadValue<float>() > 0.5f ? true : false;
    }

    private void OnJump()
    {
        //Jump();
    }

    public void AddImpactVelocity(Vector2 velocity)
    {
        _rigidBody2D.AddForce(velocity, ForceMode2D.Impulse);
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
