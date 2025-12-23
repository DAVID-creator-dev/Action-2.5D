using UnityEngine.InputSystem;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.HighDefinition;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    #region Script Parameters
    [Header("Movement Parameters")]
    [SerializeField] private float speedWithGenerator;
    [SerializeField] private float speedWithoutGenerator;
    [SerializeField] private float fallMultiplierWithGenerator = 2.5f;
    [SerializeField] private float fallMultiplierWithoutGenerator = 2.5f;
    [SerializeField] private float jumpHeightWithGenerator;
    [SerializeField] private float jumpHeightWithoutGenerator;
    [SerializeField] private float AccelerationTime = 0.5f; 
    [SerializeField] private CharacterController Player;

    [Header("Object References")]
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    public GameObject generator;
    public GameObject generatorEmplacement;
    [SerializeField] private HDAdditionalLightData leftEye;
    [SerializeField] private HDAdditionalLightData rightEye;
    [SerializeField] private Light PointLight;

    [Header("Player Parameters")]
    [SerializeField] private float grabHeight;
    [SerializeField] private float rotationVelocity = 5f;
    [SerializeField] private float gainFrequency;
    [SerializeField] private float depleteFrequency;
    [SerializeField] public bool moveOnZAxis;
    [SerializeField] public float GroundedRadius = 0.28f;

    [Header("Health Parameters")]
    public int MaxHealth = 100;
    public int health;

    //Speed Paramateters 
    private float accelerationTimer = 0f;
    private float topSpeed;
    private float currentSpeed = 0f;
    private float targetSpeed = 0f;

    //Gravity Paramaters
    private float terminalVelocity = 51.0f;
    private float verticalVelocity;
    private float Gravity = -9.81f;

    //Collison Box Parameters 
    private float boxDistanceTorso;
    private float boxDistanceLeg;

    // Player Parameters
    private bool isGrounded;
    private bool isPushing;
    private int jumpLeft;
    private Vector2 inputMove;
    private Vector2 outputMove;
    private Vector3 direction;
    private Vector3 targetOffset;
    private Vector3 initialOffset;
    private CinemachineTransposer offset;
    private Animator playerAnimator;
    private GameObject box;
    private float timer;
    private float maxIntensity;
    [HideInInspector] public Vector3 externalDirectionInfluence; 
    [HideInInspector] public bool haveGenerator = true;
    [HideInInspector] public Vector3 lastDirection = Vector3.right;
    [HideInInspector] public Vector3 respawnPos;
    [HideInInspector] public Rigidbody rigidbodyGen;

    //MachineState 
    private bool isLanding;
    private bool isJumping;
    private bool isWalking;
    private bool isFalling;
    private bool hasTouchTheGround; 
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (leftEye)
            maxIntensity = leftEye.intensity;

        rigidbodyGen = generator.GetComponent<Rigidbody>();
        offset = playerCamera.GetCinemachineComponent<CinemachineTransposer>();

        topSpeed = speedWithGenerator;

        respawnPos = transform.position;
        targetOffset = offset.m_FollowOffset;
        initialOffset = offset.m_FollowOffset;

        playerAnimator = GetComponentInChildren<Animator>();

        boxDistanceTorso = 0.95f;
        boxDistanceLeg = 0.4f; 
    }
    #endregion

    #region Methods
    private void Update()
    {
        UpdatePlayerMovement();
        UpdatePlayerAnimations();
        ApplyGravity();
        IsGrounded();

        HealthPlayer();
        HealthEye();

        if (!GameManager.Get.isInCinematic)
        {
            ChangeAxis();

            Player.Move(direction * (currentSpeed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime + externalDirectionInfluence);

            isWalking = direction != Vector3.zero;
        }
    }
    private void UpdatePlayerMovement()
    {
        Move();
        Rotate();
        CameraMove();
    }
    private void ChangeAxis()
    {
        if (moveOnZAxis)
            direction = new Vector3(0f, 0f, -outputMove.x);
        else
            direction = new Vector3(outputMove.x, 0f, 0f);

        if (direction != Vector3.zero)
        {
            lastDirection = direction;  
        }
    }

    public void Camera()
    {
        float zOffset = initialOffset.z;
        float xOffset = initialOffset.x;

        if (moveOnZAxis)
            zOffset = lastDirection.z < 0 ? -initialOffset.z : initialOffset.z;
        else
            xOffset = lastDirection.x < 0 ? -initialOffset.x : initialOffset.x;

        targetOffset = new Vector3(xOffset, initialOffset.y, zOffset);
    }
    private void CheckObstacles(Vector3 direction)
    {
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, lastDirection, out hit, 1.0f))
        {
            if (hit.transform.gameObject == null || hit.transform.gameObject.CompareTag("BackGround"))
            {
                DropGenerator(direction);
            }
        }
        else
        {
            DropGenerator(direction);
        }
    }
    public void DropGenerator(Vector3 direction)
    {
        Vector3 targetGeneratorPos = transform.position + (direction * 0.8f) + (Vector3.up * 1.5f);

        generator.transform.position = targetGeneratorPos;
        generator.transform.rotation = Quaternion.identity;

        haveGenerator = false;
        generator.transform.parent = null;
        rigidbodyGen.isKinematic = false;

        topSpeed = speedWithoutGenerator;

        boxDistanceTorso = 0.4f;
        boxDistanceLeg = 0.4f;

        rigidbodyGen.AddForce(Vector3.down * 10, ForceMode.VelocityChange);
    }

    private void GrabObject()
    {
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(transform.position.x, transform.position.y - grabHeight, transform.position.z), lastDirection, 1.0f);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.tag == "Generator" && !haveGenerator && isGrounded && !isPushing)
            {
                PlaySound("Recharge_Energy");
                PlaySound("Take_Generator");

                generator = hit.transform.gameObject;
                rigidbodyGen = generator.GetComponent<Rigidbody>();

                rigidbodyGen.isKinematic = true;
                haveGenerator = true;
                generator.transform.parent = transform;

                generator.transform.position = generatorEmplacement.transform.position;
                generator.transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);

                topSpeed = speedWithGenerator;

                boxDistanceTorso = 0.7f;
                boxDistanceLeg = 0.4f;
            }
        }
    }

    private void IsGrounded()
    {
        Vector3 spherePosition = transform.position;
        Collider[] colliders = Physics.OverlapSphere(spherePosition, GroundedRadius);

        isGrounded = false;
        foreach (Collider collider in colliders)
        {
            if (!collider.CompareTag("Player") && !collider.CompareTag("BackGround"))
            {
                isGrounded = true;
                 break;
            }
        }

        if (isGrounded && !hasTouchTheGround)
        {
            PlaySound("Fall_On_Ground");
        }

        if (!haveGenerator)
        {
            isFalling = !isGrounded;
        }

        if (!haveGenerator && isGrounded && !hasTouchTheGround)
        {
            isLanding = true;
            StartCoroutine(CoroPostLanded()); 
        }
        hasTouchTheGround = isGrounded; 
    }

    private IEnumerator CoroPostLanded()
    {
        yield return new WaitForSeconds(0.1f);
        isLanding = false;
    }

    private void Push()
    {
        RaycastHit hit;
        if (!isPushing)
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y - grabHeight, transform.position.z), lastDirection, out hit, 1.0f))
            {
                if (hit.transform.gameObject.CompareTag("Generator") && isGrounded)
                {
                    if (box == null || box != hit.collider.gameObject)
                    {
                        if (box != null)
                        {
                            box.transform.parent = null;
                        }
                        box = hit.collider.gameObject;
                        box.transform.SetParent(transform);
                    }
                    isPushing = true;

                    PlaySound("Push_Generator");

                    boxDistanceTorso = 1.5f;
                    boxDistanceLeg = 1.5f;
                }
            }
        }
        else
        {
            isPushing = false;
            if (box != null)
            {
                box.transform.parent = null;
                box = null;
            }

            boxDistanceTorso = 0.4f;
            boxDistanceLeg = 0.4f;
        }

    }

    private void Jump()
    {
        float jumpHeight;
        jumpHeight = haveGenerator ? jumpHeightWithGenerator : jumpHeightWithoutGenerator;

        verticalVelocity = Mathf.Sqrt(jumpHeight * -2 * Gravity);

        StartCoroutine(CoroPostJump());
    }

    private IEnumerator CoroPostJump()
    {
        if (!haveGenerator)
        {
            isJumping = true;
            yield return new WaitForSeconds(0.3f);
            isJumping = false;
        }
    }

    private void ApplyGravity()
    {
        if (isGrounded)
        {
            if (verticalVelocity < 0.0f)
                verticalVelocity = -2f;
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * (haveGenerator ? fallMultiplierWithGenerator : fallMultiplierWithoutGenerator) * Time.deltaTime; 
        }
    }

    private void Move()
    {
        outputMove = inputMove;

        if (outputMove != Vector2.zero) 
        {
            targetSpeed = topSpeed;

            accelerationTimer += Time.deltaTime;
            if (accelerationTimer >= AccelerationTime)
                accelerationTimer = AccelerationTime;
        }
        else
        {
            targetSpeed = 0f;
            accelerationTimer = 0f; 
        }
        currentSpeed = Mathf.Lerp(0f, targetSpeed, accelerationTimer / AccelerationTime);

        HandlePushingAndClipping();
        direction.x = outputMove.x;
    }

    private void CameraMove()
    {
        float horizontalInput = inputMove.x;

        if (horizontalInput != 0 && !GameManager.Get.isInCinematic)
        {
            if (moveOnZAxis)
                targetOffset.z = Mathf.Abs(targetOffset.z) * Mathf.Sign(horizontalInput);
            else
                targetOffset.x = Mathf.Abs(targetOffset.x) * Mathf.Sign(-horizontalInput);
        }

        offset.m_FollowOffset = Vector3.Lerp(offset.m_FollowOffset, targetOffset, 1.0f * Time.deltaTime);
    }

    private void Rotate()
    {
        if (!isPushing)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lastDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationVelocity);
        }
    }

    private void HealthPlayer()
    {
        if (haveGenerator && health < MaxHealth)
            UpdateHealth(1);
        else if (!haveGenerator)
            UpdateHealth(-1);
    }
    private void UpdateHealth(int factor)
    {
        timer += Time.deltaTime;
        if (timer > (haveGenerator ? gainFrequency : depleteFrequency))
        {
            health += factor;
            GameManager.Get.OnHealthUpdate.Invoke(health);

            timer = 0;
        }
    }

    private void HealthEye()
    {
        ColorEyes();
        if (leftEye && rightEye)
        {
            leftEye.intensity = UpdateEyeIntensity();
            rightEye.intensity = UpdateEyeIntensity();
        }
        if (PointLight)
        {
            PointLight.intensity = 20 * (float)health / (float)MaxHealth;
        }
    }
    private void ColorEyes()
    {
        if (health < MaxHealth * 0.7)
            ChangeColor(Color.white, Color.yellow, 1.5f);
        if (health < MaxHealth * 0.5)
            ChangeColor(Color.yellow, Color.red, 1.5f);
        else
            ChangeColor(Color.white, Color.white, 1.5f);
    }
    private void ChangeColor(Color targetColor, Color previousColor, float lerpSpeed)
    {
        leftEye.color = Color.Lerp(targetColor, previousColor, lerpSpeed);
        rightEye.color = Color.Lerp(targetColor, previousColor, lerpSpeed);
        PointLight.color = Color.Lerp(targetColor, previousColor, lerpSpeed);
    }
    private float UpdateEyeIntensity()
    {
        return maxIntensity * (float)health / (float)MaxHealth;
    }
    private void HandlePushingAndClipping()
    {
        if (isPushing && Mathf.Sign(lastDirection.x) * Mathf.Sign(outputMove.x) < 0)
            outputMove.x = 0;

        float moveInput = moveOnZAxis ? -outputMove.x : outputMove.x;

        Collider[] legColliders = OverlapBox(0.5f, boxDistanceLeg); 
        bool legDetection = false;

        Collider[] torsoColliders = OverlapBox(1.5f, boxDistanceTorso);
        bool torsoDetection = false;

        legDetection = DetectCollision(legColliders, new string[] { "BackGround", "Player" });

        string[] torsoIgnoreTags = haveGenerator ? new string[] { "BackGround", "Generator", "Player" } : new string[] { "BackGround", "Player" };
        torsoDetection = DetectCollision(torsoColliders, torsoIgnoreTags);


        if (legDetection || torsoDetection)
        {
            if (Mathf.Sign(lastDirection.x) * Mathf.Sign(moveInput) > 0)
                outputMove.x = 0;
        }
    }

    private bool DetectCollision(Collider[] colliders, string[] ignoreTags)
    {
        foreach (Collider collider in colliders)
        {
            bool isIgnored = false;
            foreach (string tag in ignoreTags)
            {
                if (collider.CompareTag(tag))
                {
                    isIgnored = true;
                    break;
                }
            }
            if (!isIgnored)
            {
                return true; 
            }
        }
        return false; 
    }

    Collider[] OverlapBox(float height, float boxDistance)
    {
        Vector3 boxSize = new Vector3(0.1f, 0.1f, 0.05f);
        Vector3 boxCenter = transform.position + (Vector3.up * height) + (moveOnZAxis ? boxDistance * Mathf.Sign(lastDirection.z) * Vector3.forward : boxDistance * Mathf.Sign(lastDirection.x) * Vector3.right);
        Quaternion boxRotation = Quaternion.LookRotation(lastDirection);

        return Physics.OverlapBox(boxCenter, boxSize, boxRotation);
    }

    //PlayeSfxSoundForPlayer
    private void PlaySound(string soundName)
    {
        GameManager.Get.OnSFXPlayerUpdate.Invoke(soundName);
    }

    //MachineState
    private void UpdatePlayerAnimations()
    {
        playerAnimator.SetBool("Jump", false);
        playerAnimator.SetBool("Walking", false);
        playerAnimator.SetBool("isFalling", false);
        playerAnimator.SetBool("Landed", false);

        if(haveGenerator) 
            playerAnimator.SetBool("HaveGenerator", true);
        else
            playerAnimator.SetBool("HaveGenerator", false);

        if (isJumping)
        {
            //Debug.Log("Jump");
            playerAnimator.SetBool("Jump", true);
        }
        else if (isFalling)
        {
            //Debug.Log("Falling");
            playerAnimator.SetBool("isFalling", true);
        }
        else if (isLanding)
        {
            //Debug.Log("Landing");
            playerAnimator.SetBool("Landed", true);

        }
        else if (isWalking)
        {
            //Debug.Log("Walking");
            playerAnimator.SetBool("Walking", true);

        }
    }

    //Reset
    public void ResetParameters()
    {
        health = MaxHealth;
        haveGenerator = true; 
        transform.position = respawnPos;
        topSpeed = speedWithGenerator;
    }

    //Input
    public void MoveInput(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
    }
    public void GrabOrDropGenerator(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (!haveGenerator)
                GrabObject();
            else
                CheckObstacles(lastDirection);
        }
    }
    public void PauseInput(InputAction.CallbackContext context)
    {
        if (context.started)
            GameManager.Get.uiManager.Pause();
    }
    public void ResetInput(InputAction.CallbackContext context)
    {
        if (context.started)
            GameManager.Get.RunRetry();

    }
    public void PushInput(InputAction.CallbackContext context)
    {
        if (context.started)
            Push();
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.started && !isPushing)
        {
            if (isGrounded)
            {
                PlaySound("Jump"); 
                jumpLeft = 2;
                Jump();
            }
            else if (jumpLeft > 1)
            {
                PlaySound("Double_Jump");
                if (jumpLeft == 2 && haveGenerator)
                {
                    DropGenerator(-lastDirection);
                }
                --jumpLeft;
                Jump();
            }
        }
    }
    #endregion
}
