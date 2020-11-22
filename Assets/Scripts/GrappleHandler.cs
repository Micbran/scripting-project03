using UnityEngine;

public class GrappleHandler : MonoBehaviour
{
    #region Variable Declarations

    [Header("Grapple Fist Transforms")]
    [SerializeField] private Transform playerFistHome = null;
    [SerializeField] private Transform RopeOrigin = null;
    [SerializeField] private Transform RopeEndPoint = null;
    [Header("Other Object Connections")]
    [SerializeField] private GameObject objectArt = null;
    [SerializeField] private GrappleInput inputHandler = null;
    [SerializeField] private LineRenderer ropeLineDefinition = null;
    [SerializeField] private Rigidbody rigidbodyToMove = null;

    [Header("Grapple Behavior")]
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private float grappleFlySpeed = 10f;
    [SerializeField] private float grappleRetractSpeed = 0.5f;

    [Space(10)]
    [SerializeField] private float grappleForce = 45f;
    [SerializeField] private float grappleAntigravityConstant = 5f;
    [SerializeField] private float playerControlForceConstant = 7.5f;

    [Space(10)]
    [SerializeField] private float grappleCooldown = 1.5f;

    // Private, but acquired in awake
    private Camera mainCamera = null;
    private BoxCollider fistCollider = null;
    private Transform thisTransform = null;


    // State bools
    private bool grappleOut = false;
    private bool isGrappling = false;
    private bool grappleRetracting = false;
    private bool collisionHappened = false;
    
    // Bad practice global variable
    private Vector3 fistDirection = Vector3.zero;

    // Properties
    private float FistDistance
    { get { return (Vector3.Distance(thisTransform.position, playerFistHome.position)); } }

    private Vector3 RopeDirection
    { get { return -(RopeEndPoint.position - RopeOrigin.position).normalized; } }

    public float GrappleCurrentCooldown { get; private set; } = 0f;

    #endregion

    #region Monobehaviour Methods

    private void Awake()
    {
        mainCamera = Camera.main;
        fistCollider = GetComponent<BoxCollider>();
        thisTransform = transform;
        fistCollider.enabled = false;
        thisTransform.position = playerFistHome.position;
        objectArt.SetActive(false);
        fistCollider.enabled = false;
    }

    private void OnEnable()
    {
        inputHandler.GrapplePressed += OnGrapplePressed;
        inputHandler.GrappleReleased += OnGrappleReleased;
    }

    private void OnDisable()
    {
        inputHandler.GrapplePressed -= OnGrapplePressed;
        inputHandler.GrappleReleased -= OnGrappleReleased;
    }

    private void FixedUpdate()
    {
        if(grappleOut)
        {
            if (FistDistance < maxDistance && !isGrappling && !grappleRetracting)
            {
                thisTransform.position += fistDirection.normalized * grappleFlySpeed;
                thisTransform.rotation = Quaternion.LookRotation(RopeDirection);
            }

            if (Mathf.Abs(FistDistance - maxDistance) <= 0.5f)
            {
                isGrappling = false;
                grappleRetracting = true;
            }

            if (grappleRetracting)
            {
                thisTransform.position = Vector3.MoveTowards(thisTransform.position, playerFistHome.position, grappleRetractSpeed);
                thisTransform.rotation = Quaternion.LookRotation(RopeDirection);
            }

            if (Mathf.Abs(FistDistance) < 0.5f && grappleRetracting)
            {
                if(collisionHappened)
                {
                    DisableGrappleWithCooldown(grappleCooldown);
                }
                else
                {
                    DisableGrappleWithCooldown(0.00001f);
                }

                AudioManager.Instance.StopAllLoopedSoundEffects();
                AudioManager.Instance.PlaySoundEffect(SoundEffect.GrappleReturn);
            }

            if(isGrappling)
            {
                // Add force in direction of rope, aka in direction of hand -> "butt" of grapple
                rigidbodyToMove.AddForce(RopeDirection * grappleForce);

                // Helps counteract normal gravity, makes you fly a lot better
                rigidbodyToMove.AddForce(Vector3.up * grappleAntigravityConstant);

                ApplyPlayerControlForce(); 
            }

            ropeLineDefinition.SetPositions(new Vector3[] { RopeEndPoint.position, RopeOrigin.position });
        }
        else if(GrappleCurrentCooldown > 0)
        {
            GrappleCurrentCooldown -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Terrain") && !isGrappling)
        {
            AudioManager.Instance.PlaySoundEffect(SoundEffect.GrappleCollision);
            collisionHappened = true;
            isGrappling = true;
        }
    }

    #endregion

    #region Callbacks

    private void OnGrapplePressed()
    {
        if (!grappleOut && GrappleCurrentCooldown <= 0)
        {
            grappleOut = true;
            grappleRetracting = false;

            objectArt.SetActive(true);
            fistCollider.enabled = true;
            ropeLineDefinition.gameObject.SetActive(true);

            fistDirection = GrappleSetup();

            AudioManager.Instance.PlaySoundEffect(SoundEffect.GrappleFire);
            AudioManager.Instance.StartLoopedSoundEffect(SoundEffect.Spooling);
        }
    }

    private void OnGrappleReleased()
    {
        isGrappling = false;
        grappleRetracting = true;
        fistCollider.enabled = false;
    }

    #endregion

    #region Private Functions

    private Vector3 GrappleSetup()
    {
        thisTransform.position = playerFistHome.position;
        thisTransform.rotation = Quaternion.LookRotation(mainCamera.transform.forward);

        Ray toBeRaycasted = mainCamera.ScreenPointToRay(Input.mousePosition);
        return toBeRaycasted.direction;
    }

    private void DisableGrappleWithCooldown(float cooldown)
    {
        grappleOut = false;
        collisionHappened = false;
        ropeLineDefinition.gameObject.SetActive(false);
        objectArt.SetActive(false);
        fistCollider.enabled = false;
        GrappleCurrentCooldown = cooldown;
    }

    private void ApplyPlayerControlForce()
    {
        Vector3 playerControlNormalVector = GetPlayerControlNormalVector();

        rigidbodyToMove.AddForce(playerControlNormalVector * playerControlForceConstant);
    }

    private Vector3 GetPlayerControlNormalVector()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float zInput = Input.GetAxisRaw("Vertical");

        if (xInput != 0 || zInput != 0)
        {
            Vector3 horizontalMovement = thisTransform.right * xInput;
            Vector3 forwardMovement = thisTransform.forward * zInput;

            Vector3 totalMovement = (horizontalMovement + forwardMovement).normalized;
            return totalMovement;
        }
        return Vector3.zero;
    }

    #endregion
}
