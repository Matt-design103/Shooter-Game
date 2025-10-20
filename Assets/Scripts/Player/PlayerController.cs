//chat lwk we are cooked lwk
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
    float rotationX = 0;

    //mouse look stuff
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    //parry stuff
    public GameObject parryHitboxPrefab;
    public Transform parrySpawnPoint;

    //combat stuff
    public float maxHealth = 100f;
    float currentHealth;
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

    //grapple visuals
    public GrappleRope grappleRope;
    public Transform grappleRopeStartPoint;

    //state stuff
    public enum PlayerState
    {
        Normal,
        Grappling,
        Stunned
    }
    public PlayerState state;
    private Vector3 grappleTarget;
    private bool isGrappling = false;

    //shooting
    public WeaponManagement weaponManagement;

    //heat stuff
    public HeatManager heatManager;
    public float dashHeatCost = 10f;

    [HideInInspector]
    public bool canMove = true;

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
    }

    void Update()
    {
       switch (state)
    {
        default:
        case PlayerState.Normal:
            HandleMovement();
            HandleGrappleInput();
            HandleShooting();
            break;
        case PlayerState.Grappling:
            HandleGrappling();
            break;
        case PlayerState.Stunned:
            // Handle stunned state if needed
            break;
    }
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
        state = PlayerState.Grappling;
        isGrappling = true;
        canMove = false;

             if (grappleRope != null)
        {
            Transform startPoint = grappleRopeStartPoint != null ? grappleRopeStartPoint : playerCamera.transform;
            grappleRope.StartGrapple(startPoint, grappleTarget);
        }
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        float curSpeedX = canMove ? (walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (characterController.isGrounded)
        {
            jumps = defaultJumps; // Reset jumps when grounded

        }

        if (Input.GetButtonDown("Jump") && canMove && jumps > 0)
        {
            moveDirection.y = jumpSpeed;
            jumps--; // Consume a jump
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }
        //parry input
        if (Input.GetKeyDown(KeyCode.C) && canParry == true)
        {

            Instantiate(parryHitboxPrefab, parrySpawnPoint.position, parrySpawnPoint.rotation);
            canParry = false;
            StartCoroutine(ParryCoolDown());
            // You can add parry animation or effects here
        }
        // Gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }


        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);

        //dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {

            StartCoroutine(Dash());
            canDash = false;
            StartCoroutine(DashCoolDown());
        }
    }
    
    public void HandleShooting()
    {  if (Input.GetButtonDown("Fire1") && weaponManagement.CurrentWeapon != null)
        {
            weaponManagement.CurrentWeapon.Fire();
            //get rotation of muzzle flash spawn pos
        }
    }

    private void HandleGrappling()
{
    // Check if we've reached the grapple point (or close enough)
    float distanceToTarget = Vector3.Distance(transform.position, grappleTarget);
        if (distanceToTarget > 3f) // Within 1 unit of target
        {
            Vector3 direction = (grappleTarget - transform.position).normalized;
            // Move towards grapple point
            characterController.Move(direction * grappleSpeed * Time.deltaTime);
        }
    else
    {
        EndGrapple();
    }
    
    //jump off grapple
    if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space))
    {
        EndGrapple();
    }
    
    // Still allow camera look during grapple
    if (canMove)
    {
        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
    }
}

    public void EndGrapple()
    {
        state = PlayerState.Normal;
        isGrappling = false;
        // Give the player some momentum when exiting grapple
        Vector3 direction = (grappleTarget - transform.position).normalized;
        moveDirection = direction * (grappleSpeed * 0.5f);
        canMove = true;
    
       if (grappleRope != null)
    {
        grappleRope.EndGrapple();
    }
}

    //damage and death logic
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
        // Implement overheat effects here (e.g., disable shooting, play sound, etc.)
        // For now, just reduce movement speed
        walkingSpeed = overHeatSpeed;
        // You can add a coroutine to recover from overheat after some time
    }

    void Die()
    {
        // Handle player death (e.g., reload scene, show game over screen)
        // For now, just disable the player
        gameObject.SetActive(false);
    }

    //dash logic

    IEnumerator Dash()
    {
        float startTime = Time.time;
        Vector3 dashDirection = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;
            heatManager.AddHeat(dashHeatCost);

        if (dashDirection.magnitude == 0)
        {
            dashDirection = transform.forward;
        }

        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }
    }

    //parry pogo logic
      public void ParryPogo()
    {
        moveDirection.y += parryBounceVelocity;
        characterController.Move(moveDirection * Time.deltaTime);
        jumps = defaultJumps; // Reset jumps when parrying
    }
   
    //spam prevention.
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