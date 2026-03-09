using UnityEngine;
using UnityEngine.AI;

public class TurretEnemy : EnemyBase
{
    [Header("Aim Joint Settings")]
    [Tooltip("Drag the neck/turret bone Transform here from the model hierarchy")]
    public Transform aimJoint;

    public float rotationOffset = 70f;


    [Tooltip("How fast the joint rotates toward the player")]
    public float aimSpeed = 5f;

    [Tooltip("Local axis that points 'forward' out of the joint (usually Vector3.forward)")]
    public Vector3 aimAxis = Vector3.forward;

    [Tooltip("Clamp how far the joint can rotate left/right in degrees")]
    public float maxYawAngle = 90f;

    [Tooltip("Clamp how far the joint can rotate up/down in degrees")]
    public float maxPitchAngle = 45f;

    [Tooltip("Offset to apply to the yaw angle for model alignment")]
    public float yawOffset = 0f;

    
    // Store the joint's initial local rotation as our neutral pose
    private Quaternion _jointNeutralLocalRot;

    protected override void Awake()
    {
        base.Awake();

        if (aimJoint != null)
            _jointNeutralLocalRot = aimJoint.localRotation;

        // Disable agent for turret since it shouldn't move
        if (agent != null)
        {
            agent.enabled = false;
        }
    }

    // LateUpdate runs AFTER the Animator updates the skeleton each frame.
    // This lets us override just the joint rotation without fighting animations.
    private void LateUpdate()
    {
        if (!isActivated || aimJoint == null || playerPos == null) return;

        AimJointAtPlayer();
    }

    private void AimJointAtPlayer()
    {
        // Direction from the joint to the player in world space
        Vector3 worldDir = (playerPos.position - aimJoint.position).normalized;

        // Convert that direction into the joint's parent local space
        // so our clamping is relative to the body, not the world
        Vector3 localDir = aimJoint.parent.InverseTransformDirection(worldDir);

        // --- Clamp yaw (left/right) ---
        float yaw = Mathf.Atan2(localDir.x, localDir.z) * Mathf.Rad2Deg;
        yaw = Mathf.Clamp(yaw, -maxYawAngle, maxYawAngle);
        yaw += yawOffset;

        // --- Clamp pitch (up/down) ---
        float pitch = Mathf.Atan2(-localDir.y, 
            new Vector2(localDir.x, localDir.z).magnitude) * Mathf.Rad2Deg;
        pitch = Mathf.Clamp(pitch, -maxPitchAngle, maxPitchAngle);

        // Build the target local rotation from clamped angles
        Quaternion targetLocalRot = Quaternion.Euler(pitch, yaw, 0f);

        // Smoothly rotate toward target
        aimJoint.localRotation = Quaternion.Slerp(
            aimJoint.localRotation,
            targetLocalRot,
            Time.deltaTime * aimSpeed
        );
    }

    protected override void ExecuteBehavior()
    {
       if (playerInAttackRange)
        {
            FacePlayer(); // Body still faces player loosely
            PerformRangedAttack();
        }
    }
}