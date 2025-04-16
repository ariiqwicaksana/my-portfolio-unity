using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
{
    class ProceduralLimb
    {
        public Transform IKTarget;
        public Vector3 defaultPosition;
        public Vector3 lastPosition;
        public bool moving;
    }

    [Header("Global")]
    [SerializeField] private LayerMask groundLayerMask = default;

    [Header("Steps")]
    [SerializeField] private Transform[] limbTargets;
    [SerializeField] private float stepSize = 1;
    [SerializeField] private float stepHeight = 1;
    [SerializeField] private int smoothness = 1;
    [SerializeField] private float raycastRange = 2;
    [SerializeField] private float feetOffset = 0;

    private int nLimbs;
    private ProceduralLimb[] limbs;

    private Vector3 lastBodyPosition;
    private Vector3 velocity;
    private bool allLimbsResting;

    void Start()
    {
        nLimbs = limbTargets.Length;
        limbs = new ProceduralLimb[nLimbs];
        Transform t;
        for (int i = 0; i < nLimbs; ++i)
        {
            t = limbTargets[i];
            limbs[i] = new ProceduralLimb()
            {
                IKTarget = t,
                defaultPosition = t.localPosition,
                lastPosition = t.position,
                moving = false
            };
        }

        lastBodyPosition = transform.position;
        allLimbsResting = true;
    }

    void FixedUpdate()
    {
        velocity = transform.position - lastBodyPosition;

        if (velocity.magnitude > Mathf.Epsilon)
            _HandleMovement();
        else if (!allLimbsResting)
            _BackToRestPosition();
    }

    private void _HandleMovement()
    {
        lastBodyPosition = transform.position;

        Vector3[] desiredPositions = new Vector3[nLimbs];
        float greatestDistance = stepSize;
        int limbToMove = -1;

        for (int i = 0; i < nLimbs; ++i)
        {
            if (limbs[i].moving) continue; // limb already moving: can't move again!

            desiredPositions[i] = transform.TransformPoint(limbs[i].defaultPosition);
            float dist = (desiredPositions[i] + velocity - limbs[i].lastPosition).magnitude;
            if (dist > greatestDistance)
            {
                greatestDistance = dist;
                limbToMove = i;
            }
        }

        for (int i = 0; i < nLimbs; ++i)
            if (i != limbToMove)
                limbs[i].IKTarget.position = limbs[i].lastPosition;

        if (limbToMove != -1)
        {
            Vector3 targetOffset = desiredPositions[limbToMove] - limbs[limbToMove].IKTarget.position;
            Vector3 targetPoint = desiredPositions[limbToMove] + velocity.magnitude * targetOffset;
            targetPoint = _RaycastToGround(targetPoint, transform.up);
            targetPoint += transform.up * feetOffset;

            allLimbsResting = false;
            StartCoroutine(_Stepping(limbToMove, targetPoint));
        }
    }

    private void _BackToRestPosition()
    {
        Vector3 targetPoint; float dist;
        for (int i = 0; i < nLimbs; ++i)
        {
            if (limbs[i].moving) continue; // limb already moving: can't move again!

            targetPoint = _RaycastToGround(
                transform.TransformPoint(limbs[i].defaultPosition),
                transform.up) + transform.up * feetOffset;
            dist = (targetPoint - limbs[i].lastPosition).magnitude;
            if (dist > 0.005f)
            {
                StartCoroutine(_Stepping(i, targetPoint));
                return;
            }
        }
        allLimbsResting = true;
    }

    private Vector3 _RaycastToGround(Vector3 pos, Vector3 up)
    {
        Vector3 point = pos;

        Ray ray = new Ray(pos + raycastRange * up, -up);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f * raycastRange, groundLayerMask))
            point = hit.point;
        return point;
    }

    private IEnumerator _Stepping(int limbIdx, Vector3 targetPosition)
    {
        limbs[limbIdx].moving = true;
        Vector3 startPosition = limbs[limbIdx].lastPosition;
        float t;
        for (int i = 1; i <= smoothness; ++i)
        {
            t = i / (smoothness + 1f);
            limbs[limbIdx].IKTarget.position =
                Vector3.Lerp(startPosition, targetPosition, t)
                + transform.up * Mathf.Sin(t * Mathf.PI) * stepHeight;
            yield return new WaitForFixedUpdate();
        }
        limbs[limbIdx].IKTarget.position = targetPosition;
        limbs[limbIdx].lastPosition = targetPosition;
        limbs[limbIdx].moving = false;
    }
}
