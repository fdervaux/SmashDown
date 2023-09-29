using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class SmashController : MonoBehaviour
{
    [SerializeField] private Transform _smashTransform;
    [SerializeField] private float _smashForce = 10f;
    [SerializeField] private LayerMask _smashLayerMask = 0;
    private VisualEffect _smashVFX = null;

    private InputAction _MoveInputAction = null;

    private Vector2 _lastInputMovement = Vector2.zero;

    private PlatformerController _platformerController = null;


    private void FixedUpdate()
    {

        Vector2 newInputMovement = _MoveInputAction.ReadValue<Vector2>();
        if (newInputMovement.magnitude > 0.2f)
        {
            _lastInputMovement = newInputMovement;
        }

        _smashTransform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(_lastInputMovement.y, _lastInputMovement.x) * Mathf.Rad2Deg);
    }

    private void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        _MoveInputAction = playerInput.actions.FindAction("Look");
        _smashVFX = _smashTransform.GetComponentInChildren<VisualEffect>();
        _platformerController = GetComponent<PlatformerController>();
    }

    private void OnSmash(InputValue value)
    {
        Smash();
    }

    private void Smash()
    {
        RaycastHit2D hit = Physics2D.CircleCast(_smashTransform.position, 0.5f, _lastInputMovement.normalized, 1f, _smashLayerMask);

        if (hit.collider != null)
        {
           
            _platformerController.AddImpactVelocity(-_lastInputMovement.normalized * _smashForce);
        }
        
        _smashVFX.Play();

    }


}
