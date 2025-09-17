//chat lwk we are cooked lwk
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 9.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    public GameObject parryHitboxPrefab;
    public Transform parrySpawnPoint;

    public float damage = 10f;
	public float range = 100f;
	public float health = 100f;
	public int defaultJumps = 2;
	public int jumps;

	// dash stuff
 	public float dashTime;
    public float dashSpeed;

    // parry stuff
    public bool canParry;
    public float parryPrevent = 0.5f;


    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        canParry = true;
    }

    void Update()
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
    if (Input.GetMouseButtonDown(1) && canParry == true)
    {
            Debug.Log("Parry Enabled");
            Instantiate(parryHitboxPrefab, parrySpawnPoint.position, parrySpawnPoint.rotation);
        
            //canParry = false;
            //ParryCoolDown();
            // You can add parry animation or effects here
        }
        // Gravity
    if (!characterController.isGrounded)
    {
        moveDirection.y -= gravity * Time.deltaTime;
    }


        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }




	if (Input.GetKeyDown(KeyCode.LeftShift))
	{
            Debug.Log("dash");
 		StartCoroutine(Dash());
   		
 	}
    }
	
 	IEnumerator Dash()
  	{
		float startTime = Time.time;
        Vector3 dashDirection = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;

		  if (dashDirection.magnitude == 0)
        {
            dashDirection = transform.forward;
        }
    
        while(Time.time < startTime + dashTime)
        {
            characterController.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }
   	}

    void ParryCoolDown()
    {
        //spam prevention
        float timer = parryPrevent;
        float elapsed = 0f;
        if (timer > 0f)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= timer)
            {
                canParry = true;
                Debug.Log("Parry ReEnabled");

            }
        }
    }

}
