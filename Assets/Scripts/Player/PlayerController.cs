using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    //movement stuff
    public float walkingSpeed = 7.5f;
    public float overHeatSpeed = 3.0f;
    public float jumpSpeed = 8.0f;
    public float grappleSpeed = 20.0f;
    public float gravity = 9.0f;
    public int defaultJumps = 2;
    public int jumps;
    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;

    //camera stuff
    public Transform cameraTarget; // Where the camera looks at (usually slightly above player)
    public Camera playerCamera;
    public float cameraDistance = 5f;
    public float cameraHeight = 2f;
    public float cameraSensitivity = 2.0f;
    public float cameraVerticalLimit = 80f;
    private float cameraYaw = 0f;
    private float cameraPitch = 20f;

    //player rotation
    public float rotationSpeed = 10f;

    //parry stuff
    public GameObject parryHitboxPrefab;
    public Transform parrySpawnPoint;

    //combat stuff
    public float maxHealth = 100f;
    public float currentHealth;
    public float damage = 10f;
    public float range = 100f;
    public float health = 100f;

    // dash stuff
    public float dashTime;
    public float dashSpeed;
    public float dashCooldown;
    public bool canDash;

    // parry stuff
    public bool canParry;
    public float parryPrevent = 0.5f;
    public float parryBounceVelocity;

    //grapple stuff
    public GrappleRope grappleRope;
    public Transform grappleRopeStartPoint;
    public float grappleRange = 100f;
    public float grappleCooldown = 1f;
    public float grappleAccelerationFactor = 10f;
    public float maxGrappleSpeed = 30f;
    public float grappleCancelBoost = 5f;
    private float currentGrappleSpeed = 0f;
    private float baseGrappleSpeed = 20f;

    //state stuff
    public enum PlayerState
    {
        Normal,
        Stunned
    }
    public PlayerState state;
    private Vector3 grappleTarget;

    //shooting
    public WeaponManagement weaponManagement;

    //heat stuff
    public HeatManager heatManager;
    public float dashHeatCost = 10f;

    [HideInInspector]
    public bool canMove = true;
    public bool isGrappling = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        heatManager = GetComponent<HeatManager>();
        weaponManagement = GetComponent<WeaponManagement>();
        
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        canParry = true;
        canDash = true;
        jumps = defaultJumps;
        state = PlayerState.Normal;
        currentHealth = maxHealth;

        // Setup camera target if not assigned
        if (cameraTarget == null)
        {
            GameObject targetObj = new GameObject("CameraTarget");
            cameraTarget = targetObj.transform;
            cameraTarget.parent = transform;
            cameraTarget.localPosition = new Vector3(0, 1.5f, 0);
        }
    }

    void Update()
    {
        switch (state)
        {
            default:
            case PlayerState.Normal:
                if (!isGrappling)
                {
                HandleMovement();
                }
                HandleGrappleInput();
                HandleGrappling();
                HandleShooting();
                break;
            case PlayerState.Stunned:
                // Handle stunned state if needed
                break;
        }
    }

    void LateUpdate()
    {
        HandleCamera();
    }

    private void HandleCamera()
    {
        // Get mouse input
        cameraYaw += Input.GetAxis("Mouse X") * cameraSensitivity;
        cameraPitch -= Input.GetAxis("Mouse Y") * cameraSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -cameraVerticalLimit, cameraVerticalLimit);

        // Calculate camera position
        Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
        Vector3 offset = rotation * new Vector3(0, cameraHeight, -cameraDistance);
        
        playerCamera.transform.position = cameraTarget.position + offset;
        playerCamera.transform.LookAt(cameraTarget.position);
    }

    private void HandleGrappleInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, range))
            {
                grappleTarget = hit.point;
                StartGrapple();
            }
        }
    }

    private void StartGrapple()
    {
        currentGrappleSpeed = baseGrappleSpeed;
        isGrappling = true;
        HandleGrappling();
        grappleRope.StartGrapple(grappleRopeStartPoint, grappleTarget);
    }

    private void HandleMovement()
    {
        // Get input relative to camera direction
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (canMove)
        {
            // Calculate movement direction relative to camera
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            
            // Flatten camera directions (ignore Y component)
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // Calculate desired move direction
            Vector3 desiredMoveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;

            // Store Y velocity before overwriting moveDirection
            float movementDirectionY = moveDirection.y;
            
            // Apply movement
            moveDirection = desiredMoveDirection * walkingSpeed;
            moveDirection.y = movementDirectionY;

            // Rotate player to face movement direction
            if (desiredMoveDirection.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(desiredMoveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        if (characterController.isGrounded)
        {
            jumps = defaultJumps; // Reset jumps when grounded
        }

        if (Input.GetButtonDown("Jump") && canMove && jumps > 0)
        {
            moveDirection.y = jumpSpeed;
            jumps--; // Consume a jump
        }

        //parry input
        if (Input.GetKeyDown(KeyCode.C) && canParry == true)
        {
            Instantiate(parryHitboxPrefab, parrySpawnPoint.position, parrySpawnPoint.rotation);
            canParry = false;
            StartCoroutine(ParryCoolDown());
        }

        // Gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        //dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
            canDash = false;
            StartCoroutine(DashCoolDown());
        }
    }

    public void HandleShooting()
    {
        var w = weaponManagement?.CurrentWeapon;
        if (Input.GetButtonDown("Fire1") && w != null)
        {
            w.Fire();
            w.StartSustainedFire();
        }

        if (Input.GetButtonUp("Fire1") && weaponManagement.CurrentWeapon != null)
        {
            w.StopSustainedFire();
        }
    }

  private void HandleGrappling()
{
    isGrappling = Input.GetMouseButton(1);
    canMove = !isGrappling;

    if (isGrappling)
    {
        float distanceToTarget = Vector3.Distance(transform.position, grappleTarget);

        // Still requires that the target is valid distance
        if (distanceToTarget > 3f && distanceToTarget <= grappleRange)
        {
            Vector3 direction = (grappleTarget - transform.position).normalized;

            // ACCELERATION LOGIC
            currentGrappleSpeed += grappleAccelerationFactor * Time.deltaTime;
            currentGrappleSpeed = Mathf.Min(currentGrappleSpeed, maxGrappleSpeed);

            // Move with accelerated speed
            characterController.Move(direction * currentGrappleSpeed * Time.deltaTime);

            // Rotate toward target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            grappleRope.UpdateGrappleTarget(grappleTarget);
        }
        else
        {
            EndGrapple();
        }

        
    }
    else
    {
        // RMB released this frame → stop accelerating
        if (currentGrappleSpeed > 0f)
            EndGrapple();
    }
}


   public void EndGrapple()
{
    isGrappling = false;
    grappleRope.EndGrapple();
    canMove = true;
    currentGrappleSpeed = 0f;  // IMPORTANT

    if (grappleRope != null)
        grappleRope.EndGrapple();
}
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Overheat()
    {
        walkingSpeed = overHeatSpeed;
    }

    void Die()
    {
        gameObject.SetActive(false);
    }

    IEnumerator Dash()
    {
        float startTime = Time.time;
        
        // Dash in the direction the player is facing
        Vector3 dashDirection = transform.forward;
        
        // If there's movement input, dash in that direction instead
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        if (horizontal != 0 || vertical != 0)
        {
            Vector3 cameraForward = playerCamera.transform.forward;
            Vector3 cameraRight = playerCamera.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();
            
            dashDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;
        }
        
        heatManager.AddHeat(dashHeatCost);

        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void ParryPogo()
    {
        moveDirection.y += parryBounceVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
        jumps = defaultJumps;
    }

    IEnumerator ParryCoolDown()
    {
        yield return new WaitForSeconds(parryPrevent);
        canParry = true;
    }

    IEnumerator DashCoolDown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void OnDestroy()
    {
    }
}