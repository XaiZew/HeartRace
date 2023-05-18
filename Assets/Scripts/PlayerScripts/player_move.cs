using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player_move : MonoBehaviour
{
    // PRESS SPACE TO VAULT SUBTITLE LIKE HYPERCHARGE AND REFERENCE IT IN DEVLOG

    float horizontal;
    float vertical;


    // Camera variables
    public Transform camHolder;
    public Transform cam;
    public FOVChanger fovChanger;
    public TiltCamera tiltCamera;

    Rigidbody rb;
    [SerializeField] Transform orientation;
    Vector3 moveDir;
    
    [Header("Grounded Stuff")]
    bool oldGrounded;
    bool isGrounded;
    RaycastHit groundHit;
    [SerializeField] LayerMask groundLayer;

    enum states {
        walking,
        running,
        sliding,
        crouching,
        wallrunning
    }

    states state;

    [SerializeField] AudioSource vaultSource;
    [SerializeField] AudioSource runSource;
    [SerializeField] AudioSource jumpSource;
    [SerializeField] AudioSource slideSource;
    [SerializeField] AudioSource grappleSource;
    [SerializeField] AudioSource airblowingSource;

    [Header("Settings")]
    float        tSpeed;
    public float speed;
    public float walkingSpeed;
    public float runningSpeed;
    public float crouchingSpeed;
    public float groundDrag;
    public float airDrag;
    public float playerRadius;
    public float playerHeight;
    public float speedChangeDuration;
    public bool  useCheckpoints;
    public bool  godMode;

    [Header("Jump Settings")]
    public float jumpForce = 12;
    public float jumpMultiplier = 2f;
    public float fallMultiplier = 2.5f;

    [Header("Sliding Settings")]
    public float slidingMin;
    public float sliderTimerLength;
    public float slideForce;

    float        sliderTimer = .8f;
    bool         slideGrounded = false;

    // Vaulting
    [Header("Vault Settings")]
    public float maxVaultHeight;
    bool         canVault;
    RaycastHit   vaultHit;
    RaycastHit   vaultHit2;
    public float vaultDistance;
    public float vaultDuration;

    // Grappleing
    [Header("Grapple Settings")]
    public float grappleDistance, minGrappleVel;
    public float springJointSpring, springJointDamper, springJointMassScale;
    Transform    grapplePos;
    public LineRenderer grappleLR;
    Vector3      grappleLRPoint;
    RaycastHit   grappleHit;
    SpringJoint  springJoint;

    Coroutine    currentSpeedRoutine;

    bool         grappling;
    [SerializeField] float grapplingDrag;

    [Header("WallRun Settings")]
    public LayerMask wallLayer;
    public float wallJumpForce;
    public float wallRunMinVel;
    public float wallForceConst;
    public float wallRunPush;

    DeathManager deathManager;
    GameManager  gameManager;
    public WeaponHolder weaponHolder;

    public Vector3 lastCheckPoint;

    // Start is called before the first frame update
    void Start()
    {
        deathManager = GameObject.FindObjectOfType<DeathManager>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody>();
        fovChanger = cam.GetComponent<FOVChanger>();
        tiltCamera = cam.GetComponent<TiltCamera>();

        if (deathManager.lastCheckPoint != null && SceneManager.GetActiveScene().buildIndex == deathManager.checkpointSceneIndex && useCheckpoints && PlayerPrefs.GetInt("SpawnAtCheckpoint") == 1) {
            transform.position = deathManager.lastCheckPoint;
        }
        PlayerPrefs.SetInt("SpawnAtCheckpoint", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) {
            if (Time.timeScale == 0)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;
        }


        if (!gameManager.isActive) return;
        dragControl();
        jumping();
        stateControl();
        get_input();
        Slope();
        GrappleLR();
        WallRunDetection();
    }

    private void FixedUpdate() {
        if (!gameManager.isActive) return;
        movePlayer();
    }

    void get_input() {

        // DEBUB KEYBINDS
        if (Input.GetKeyDown(KeyCode.Backspace)) {
            deathManager.StartCoroutine(deathManager.Death());
        }


        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift) && state != states.sliding && state != states.crouching && state != states.wallrunning) {
            state = states.running;
            if (tSpeed != runningSpeed) {
                ChangeSpeed(runningSpeed);
            }
        }
        // If start running then change speed to running speed.
        if (Input.GetKeyDown(KeyCode.LeftShift) && state != states.sliding && state != states.crouching && state != states.wallrunning) {
            state = states.running;
            ChangeSpeed(runningSpeed);
            fovChanger.StartCoroutine(fovChanger.ChangeFOV(85, .4f));
        }

        // Sliding
        if (Input.GetKeyDown(KeyCode.LeftControl) && rb.velocity.magnitude > slidingMin) {
            state = states.sliding;
            slideSource.Play();
            ChangeSpeed(crouchingSpeed);
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl) && rb.velocity.magnitude < slidingMin && state != states.sliding) {
            state = states.crouching;
            ChangeSpeed(crouchingSpeed);
        }

        // Vaulting
        if (Input.GetKeyDown(KeyCode.Space) && canVault) {
            Vault();
        }

        // Grappleing
        if (Input.GetKeyDown(KeyCode.E)) {
            StartGrapple();
        }
        if (Input.GetKeyUp(KeyCode.E)) {
            StopGrapple();
        }
    }

    void movePlayer() {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (state != states.wallrunning)
            moveDir = orientation.forward * vertical + orientation.right * horizontal;
        
        moveDir = moveDir.normalized;
        rb.AddForce(moveDir * speed, ForceMode.Force);
    }

    void dragControl() {
        oldGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight + 0.5f, groundLayer);

        if (oldGrounded != isGrounded) {
            jumpSource.Play();
        }

        if (isGrounded) {
            rb.drag = groundDrag;
        }
        else {
            if (grappling)
                rb.drag = grapplingDrag;
            else
                rb.drag = airDrag;
        }
    }

    void jumping() {
        if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            jumpSource.Play();
        }
        if (OnSlope()) return;
        if (state == states.wallrunning) return;
        if (rb.velocity.y < 0 || grappling || (weaponHolder.currWeapon.GetComponent<GunScript>()?.gunData.gunType == GunData.gunTypes.laser && weaponHolder.currWeapon.GetComponent<GunScript>()?.shootTimer >= weaponHolder.currWeapon.GetComponent<GunScript>()?.gunData.chargeTime && weaponHolder.currWeapon.GetComponent<GunScript>()?.laserDelayTimer >= weaponHolder.currWeapon.GetComponent<GunScript>()?.gunData.laserInterval)) {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space) && state != states.wallrunning) {
            rb.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * Time.deltaTime;
        }
    }

    void sprinting() {
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            ChangeSpeed(walkingSpeed);
            fovChanger.StartCoroutine(fovChanger.ChangeFOV(80, .4f));
            state = states.walking;
        }
    }

    void stateControl() {
        if (state == states.walking) {
            transform.localScale = new Vector3(1, 1, 1);
            canVault = true;
            runSource.pitch = .65f;
            airblowingSource.volume = .4f;
            if (new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude > 1 && isGrounded) {
                if (!runSource.isPlaying)
                    runSource.Play();
                if (!airblowingSource.isPlaying)
                    airblowingSource.Play();
            }
            if (new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude <= 1) {
                if (runSource.isPlaying)
                    runSource.Stop();
                if (airblowingSource.isPlaying)
                    airblowingSource.Stop();
            }
            if (!isGrounded && runSource.isPlaying)
                runSource.Stop();
        }
        if (state == states.running) {
            sprinting();
            transform.localScale = new Vector3(1, 1, 1);
            canVault = true;
            runSource.pitch = 1f;
            airblowingSource.volume = 1f;
            if (!runSource.isPlaying && isGrounded) {
                runSource.Play();
            }
            if (!isGrounded) {
                runSource.Stop();
            }
            if (!airblowingSource.isPlaying)
                airblowingSource.Play();
        }
        if (state == states.sliding) {
            sliding();
        }
        if (state == states.crouching) {
            crouching();
            canVault = false;
        }
    }

    void sliding() {
        if (isGrounded) {
            slideGrounded = true;
        }
        if (sliderTimer <= 0 || !Input.GetKey(KeyCode.LeftControl)) {
            sliderTimer = sliderTimerLength;
            slideGrounded = false;
            state = states.crouching;
            ChangeSpeed(crouchingSpeed);
        }
        if (slideGrounded) {
            sliderTimer -= 1 * Time.deltaTime;
        }
        if (isGrounded)
            rb.AddForce(moveDir * slideForce * sliderTimer, ForceMode.Force);
        else
            rb.AddForce(moveDir * (slideForce / 4) * sliderTimer, ForceMode.Force);
        transform.localScale = new Vector3(.5f, .5f, .5f);
    }

    void crouching() {
        transform.localScale = new Vector3(.5f, .5f, .5f);
        if (Input.GetKeyUp(KeyCode.LeftControl)) {
            ChangeSpeed(walkingSpeed);
            state = states.walking;
        }
    }

    void Vault() {
        // Check if wall is there
        if (!Physics.Raycast((camHolder.position - (Vector3.up * .5f)), orientation.forward, out vaultHit, vaultDistance, groundLayer)) return;
        // if (vaultHit.transform.CompareTag("Grappleable")) return;
        // Check if there is room to vault
        if (Physics.Raycast(vaultHit.point + (orientation.forward * playerRadius) + (Vector3.up * playerHeight * maxVaultHeight), Vector3.down, out vaultHit2)) {
            if (vaultHit.transform != vaultHit2.transform) return;
            if (state == states.wallrunning) {
                state = states.walking;
            }
            StartCoroutine(VaultToPos(vaultHit2.point));
        }
    }

    IEnumerator VaultToPos(Vector3 targetPos) {
        vaultSource.Play();
        targetPos = targetPos + (Vector3.up * playerHeight);
        float t = 0;
        Vector3 startPos = transform.position;

        while (t < vaultDuration) {
            transform.position = Vector3.Lerp(startPos, targetPos, t / vaultDuration);
            t += 1 * Time.deltaTime;
            yield return null;
        }
        transform.position = targetPos;
        yield return null;
    }

    IEnumerator ChangeSpeedRoutine(float targetSpeed) {
        float t = 0;
        float startSpeed = speed;
        tSpeed = targetSpeed;
        while (t < speedChangeDuration) {
            speed = Mathf.Lerp(startSpeed, targetSpeed, t / speedChangeDuration);
            t += 1 * Time.deltaTime;
            yield return null;
        }
        speed = targetSpeed;
    }

    void ChangeSpeed(float targetSpeed) {
        if (currentSpeedRoutine != null)
            StopCoroutine(currentSpeedRoutine);
        currentSpeedRoutine = StartCoroutine(ChangeSpeedRoutine(targetSpeed));
    }

    RaycastHit slopeHit;
    public float maxSlopeAngle;
    bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight + 0.2f, groundLayer)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    Vector3 GetSlopeMoveDir() {
        return Vector3.ProjectOnPlane(moveDir, slopeHit.normal).normalized;
    }

    void Slope() {
        if (OnSlope()) {
            rb.AddForce(GetSlopeMoveDir() * speed / 4, ForceMode.Force);
        }
        if (state != states.wallrunning) {
            rb.useGravity = !OnSlope();
        }
    }

    void StartGrapple() {
        grapplePos = grappleLR.transform;
        if (Physics.Raycast(camHolder.position, camHolder.forward, out grappleHit, grappleDistance, groundLayer)) {
            if (!grappleHit.transform.CompareTag("Grappleable")) return;
            grappleSource.Play();
            grappling = true;
            // if (!grappleHit.transform.CompareTag("Grappleable")) return;
            grappleLR.positionCount = 2;
            grappleLRPoint = grappleHit.point;

            springJoint = gameObject.AddComponent<SpringJoint>();
            springJoint.autoConfigureConnectedAnchor = false;
            springJoint.connectedAnchor = grappleLRPoint + Vector3.up * 8;

            float distance = Vector3.Distance(transform.position, grappleLRPoint);

            springJoint.maxDistance = distance * 0.8f;
            springJoint.minDistance = distance * 0.25f;

            springJoint.spring = springJointSpring;
            springJoint.damper = springJointDamper;
            springJoint.massScale = springJointMassScale;

        }
    }

    void GrappleLR() {
        if (!springJoint) return;
        if (grappleLR.positionCount < 1) return;
        // Conditions while wall running
        grappleLR.SetPosition(0, grapplePos.position);
        grappleLR.SetPosition(1, grappleLRPoint);
        
        if (rb.velocity.magnitude < minGrappleVel || Vector3.Distance(transform.position, grappleLRPoint) < 0)
            StopGrapple();
    }

    void StopGrapple() {
        grappling = false;
        Destroy(springJoint);
        grappleLR.positionCount = 0;
    }

    
    RaycastHit wallRunHitLeft;
    RaycastHit wallRunHitRight;
    Transform currWall;
    bool leftOrRightWall = false;
    Vector3 wallRunPushDirection;
    

    void WallRunDetection() {
        float currMag = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;

        if (state == states.wallrunning) {

            if (currMag < wallRunMinVel) {
                StopWallRun("vel");
            }

            Vector3 wallForward;
            if (leftOrRightWall)
                wallForward = Vector3.Cross(wallRunHitLeft.normal * wallForceConst, transform.up);
            else
                wallForward = Vector3.Cross(wallRunHitRight.normal * wallForceConst, transform.up);

            if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
                wallForward = -wallForward;

            moveDir = wallForward;
        }

        if (Physics.Raycast(transform.position, orientation.right, out wallRunHitRight, playerRadius / 2 + 0.5f, wallLayer)) {
            // if (!wallRunHitRight.transform.CompareTag("WallRunnable")) return;
            if (isGrounded) return;
            if (currMag < wallRunMinVel) return;
            leftOrRightWall = false;
            currWall = wallRunHitRight.transform;
            wallRunPushDirection = wallRunHitRight.normal;

            if (Input.GetKeyDown(KeyCode.Space)) {
                rb.AddForce(-orientation.right * wallJumpForce, ForceMode.Impulse);
                rb.AddForce(Vector3.up * wallJumpForce * .65f, ForceMode.Impulse);
            }

            if (state != states.wallrunning)
                StartWallRun("fray");
        }
        else if (Physics.Raycast(transform.position, -orientation.right, out wallRunHitLeft, playerRadius / 2 + 0.5f, wallLayer)) {
            // if (!wallRunHitLeft.transform.CompareTag("WallRunnable")) return;
            if (isGrounded) return;
            if (currMag < wallRunMinVel) return;
            leftOrRightWall = true;
            currWall = wallRunHitLeft.transform;
            wallRunPushDirection = wallRunHitLeft.normal;

            if (Input.GetKeyDown(KeyCode.Space)) {
                rb.AddForce(orientation.right * wallJumpForce, ForceMode.Impulse);
                rb.AddForce(Vector3.up * wallJumpForce * .65f, ForceMode.Impulse);
            }

            if (state != states.wallrunning)
                StartWallRun("sray");
        }
        else {
            // Conditions to stop wall running while looking away from wall
            if (state != states.wallrunning) return;

            Collider[] cols = Physics.OverlapSphere(transform.position, playerRadius + 0.1F, wallLayer);
            if (cols.Length == 0) {
                StopWallRun("cols");
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                rb.AddForce(camHolder.forward * wallJumpForce, ForceMode.Impulse);
                rb.AddForce(Vector3.up * wallJumpForce * .65f, ForceMode.Impulse);
                Debug.Log("d");
            }

            if (!leftOrRightWall)
                rb.AddForce(-wallRunHitRight.normal * wallRunPush, ForceMode.Force);
            else
                rb.AddForce(-wallRunHitLeft.normal * wallRunPush, ForceMode.Force);
        }
    }

    void StartWallRun(string str = "Default") {
        state = states.wallrunning;
        fovChanger.StopAllCoroutines(); // Making sure it's not currently changing fov
        tiltCamera.StopAllCoroutines(); // Making sure it's not current changing rotation
        if (leftOrRightWall)
            tiltCamera.StartCoroutine(tiltCamera.RotateCameraZ(-6.5f, .4f));
        else
            tiltCamera.StartCoroutine(tiltCamera.RotateCameraZ(6.5f, .4f));
        fovChanger.StartCoroutine(fovChanger.ChangeFOV(95, .4f));
        rb.useGravity = false;
    }

    void StopWallRun(string str = "Default") {
        Debug.Log(str);
        state = states.running;
        fovChanger.StopAllCoroutines();
        tiltCamera.StopAllCoroutines();
        tiltCamera.StartCoroutine(tiltCamera.RotateCameraZ(0, .4f));
        fovChanger.StartCoroutine(fovChanger.ChangeFOV(80, .4f));
        rb.useGravity = true;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Checkpoint")) {
            deathManager.lastCheckPoint = other.transform.position;
            PlayerPrefs.SetFloat(deathManager.lastCheckPointX, other.transform.position.x);
            PlayerPrefs.SetFloat(deathManager.lastCheckPointY, other.transform.position.y);
            PlayerPrefs.SetFloat(deathManager.lastCheckPointZ, other.transform.position.z);
            PlayerPrefs.SetInt("CheckPointSceneBuildIndex", SceneManager.GetActiveScene().buildIndex);
        }
    }
}