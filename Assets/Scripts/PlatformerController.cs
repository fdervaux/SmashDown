using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformerController : MonoBehaviour
{

    [SerializeField, Range(0, 10)] private float _speed = 5.0f;
    [SerializeField, Range(0, 100)] private float _jumpForce = 5.0f;

    [SerializeField, Range(0, 3)] private float _gravityScaleNormal = 2f;
    [SerializeField, Range(0, 3)] private float _gravityScaleJumpUp = 1f;
    [SerializeField, Range(0, 3)] private float _gravityScaleJumpRelease = 1.5f;

    [SerializeField] private float _groundDetectionOffset = 1;
    [SerializeField] private LayerMask _groundDetectionLayerMask = 0;

    [SerializeField, Range(0,1)] private float _airControl = 0.5f;


    //private Vector3 _moveInput = Vector3.zero;
    private Rigidbody2D _rigidBody2D = null;

    private InputAction _moveInputAction = null;
    private InputAction _smashInputAction = null;

    private bool _grounded = false;


    // Start is called before the first frame update
    void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        PlayerInput playerInput = GetComponent<PlayerInput>();
        _moveInputAction = playerInput.actions.FindAction("Move");
        _smashInputAction = playerInput.actions.FindAction("Smash");
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector3 moveInput = _moveInputAction.ReadValue<Vector2>();

        bool smashInput = _smashInputAction.ReadValue<float>() > 0.5f ? true : false;


        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position + Vector3.up * _groundDetectionOffset, 
            new Vector2(1f, 0.2f), 
            0f, 
            Vector2.down, 
            _groundDetectionOffset, 
            _groundDetectionLayerMask
            );


        _grounded = false;

        if(hit.collider != null && _rigidBody2D.velocity.y <= 0)
        {
            _grounded = true;
        }
        

        if(_grounded)
            _rigidBody2D.velocity = new Vector2(moveInput.x * _speed, _rigidBody2D.velocity.y);
        else
        {
            float speed = Mathf.Lerp(_rigidBody2D.velocity.x, moveInput.x * _speed, _airControl);
            _rigidBody2D.velocity = new Vector2( speed , _rigidBody2D.velocity.y);
        }


        if (_rigidBody2D.velocity.y < 0)
        {
            _rigidBody2D.gravityScale = _gravityScaleNormal;
        }

        if (_rigidBody2D.velocity.y > 0 && !smashInput)
        {
            _rigidBody2D.gravityScale = _gravityScaleJumpRelease;
        }
    }

    private void OnSmash(InputValue value)
    {
        Debug.Log("Smash! " + value.Get<float>());

        if(!_grounded)
        {
            return;
        }

        _rigidBody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        _rigidBody2D.gravityScale = _gravityScaleJumpUp;
    }
}
