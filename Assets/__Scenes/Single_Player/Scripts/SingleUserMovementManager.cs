using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SingleUserMovementManager : MonoBehaviour, IUserManager
{
    [SerializeField] private ApplicationData applicationData = null;

    private int playerIndex = 0;
    private UserData userData = null;
    private CharacterController characterController = null;
    private Vector2 playerRotation = Vector2.zero;
    private float playerMovementSpeed = 0.0f;
    private Vector3 movementDirection = Vector3.zero;

    public void SetPlayer(int _newPlayerIndex)
    {
        playerIndex = _newPlayerIndex;
        userData = applicationData.users[playerIndex];
    }

    private void Awake()
    {
        OnInitialize();
    }

    private void OnEnable()
    {
        SetDefaultValues();
    }

    private void FixedUpdate()
    {
        movementDirection.Normalize();
        characterController.Move(movementDirection * playerMovementSpeed * Time.deltaTime);
    }

    private void Update()
    {
        float playerRotationSpeed;

        playerRotationSpeed = applicationData.users[playerIndex].rotationSpeed;

        if (playerRotationSpeed > 0)
        {
            playerRotation.x += Input.GetAxis("Mouse X") * playerRotationSpeed;
            playerRotation.y += Input.GetAxis("Mouse Y") * playerRotationSpeed;
            playerRotation.y = Mathf.Clamp(playerRotation.y, -1f * userData.yRotationLimit,
                userData.yRotationLimit);

            var xQuat = Quaternion.AngleAxis(playerRotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(playerRotation.y, Vector3.left);

            transform.localRotation = xQuat * yQuat;
        }

        // If the movement speed is <= 0, then there is no point in checking movement input.
        playerMovementSpeed = userData.movementSpeed;
        if(playerMovementSpeed <= 0.0f)
        {
            return;
        }

        if((Input.GetKey(KeyCode.LeftShift) == true) && (userData.currentRunStamina > 0))
        {
            playerMovementSpeed = userData.runSpeed;
            userData.currentRunStamina -= (Time.deltaTime * userData.rateOfStaminaLoss);
        }
        else if ((Input.GetKey(KeyCode.LeftShift) == false) 
            && (userData.currentRunStamina <= userData.runMaxStamina))
        {
            userData.currentRunStamina += (Time.deltaTime * userData.rateOfStaminaGain);
        }

        float xAxisMovement = Input.GetAxis("Horizontal");
        float zAxisMovement = Input.GetAxis("Vertical");

        movementDirection = (transform.right * xAxisMovement) + (transform.forward * zAxisMovement);
        movementDirection.y = 0.0f;
    }

    private void OnInitialize()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void SetDefaultValues()
    {
        userData = applicationData.networkUsers[0];
    }
}