using UnityEngine;

public class BoxGroundSensor :  GroundSensor
{
    [SerializeField] private LayerMask _groundLayerMask = 0;
    [SerializeField] private float _groundDetectionOffset = 1;

    private RaycastHit2D _hit;

    private void Start()
    {
    }

    public override float GetGroundDistance()
    {
        if (_hit.collider != null)
        {
            return _hit.distance - _groundDetectionOffset;
        }

        return Mathf.Infinity;
    }

    public override bool IsGrounded()
    {
        if (_hit.collider != null)
        {
            return true;
        }

        return false;
    }

    public override void UpdateGroundDetection()
    {
        _hit = Physics2D.BoxCast(
            transform.position + Vector3.up * _groundDetectionOffset,
            new Vector2(1f, 0.2f),
            0f,
            Vector2.down,
            _groundDetectionOffset,
            _groundLayerMask
            );

    }
}
