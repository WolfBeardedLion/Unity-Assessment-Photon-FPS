using Photon.Pun;
using Photon.Pun.UtilityScripts;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NetworkUserMovementManager : MonoBehaviourPunCallbacks, IUserManager
{
    [SerializeField] private ApplicationData applicationData = null;

    private int playerIndex = 0;
    private UserData userData = null;
    private CharacterController characterController = null;
    private Vector2 playerRotation = Vector2.zero;
    private float playerMovementSpeed = 0.0f;
    private Vector3 movementDirection = Vector3.zero;
    private PhotonView playerPhotonView = null;

    private const int cameraTranformIndex = 1;
    private const int avatarTranformIndex = 2;

    public void SetPlayer(int _newPlayerIndex)
    {
        playerIndex = _newPlayerIndex;
        userData = applicationData.users[playerIndex];
    }

    private void Awake()
    {
        OnInitialize();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        SetDefaultValues();
    }

    private void Start()
    {
        if (playerPhotonView.IsMine == false)
        {
            return;
        }

        playerPhotonView.RPC("RPCUpdateOtherMeshRenderer", RpcTarget.All);
    }

    private void FixedUpdate()
    {
        if (playerPhotonView.IsMine == false)
        {
            return;
        }

        movementDirection.Normalize();
        characterController.Move(movementDirection * playerMovementSpeed * Time.fixedDeltaTime);
    }

    private void Update()
    {
        if (playerPhotonView.IsMine == false)
        {
            return;
        }

        float playerRotationSpeed;

        playerRotationSpeed = applicationData.networkUsers[playerIndex].rotationSpeed;

        if (playerRotationSpeed > 0)
        {
            playerRotation.x += Input.GetAxis("Mouse X") * playerRotationSpeed;
            playerRotation.y += Input.GetAxis("Mouse Y") * playerRotationSpeed;

            try
            {
                playerRotation.y = Mathf.Clamp(playerRotation.y, -1f * userData.yRotationLimit,
                    userData.yRotationLimit);
            }
            catch { }

            var xQuat = Quaternion.AngleAxis(playerRotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(playerRotation.y, Vector3.left);

            transform.localRotation = xQuat * yQuat;
        }

        if(userData == null)
        { 
            return;
        }

        // If the movement speed is <= 0, then there is no point in checking movement input.
        playerMovementSpeed = userData.movementSpeed;
        if (playerMovementSpeed <= 0.0f)
        {
            return;
        }

        if ((Input.GetKey(KeyCode.LeftShift) == true) && (userData.currentRunStamina > 0))
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
        playerPhotonView = GetComponent<PhotonView>();
        characterController = GetComponent<CharacterController>();
    }

    private void SetDefaultValues()
    {
        if (playerPhotonView.IsMine == false)
        {
            Destroy(transform.GetChild(cameraTranformIndex).gameObject);
            return;
        }

        userData = applicationData.networkUsers[0];
    }

    [PunRPC]
    private void RPCUpdateOtherMeshRenderer()
    {
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        for(int i = 0;  i < photonViews.Length; i++)
        {
            if (photonViews[i].IsMine == true)
            {
                continue;
            }

            photonViews[i].transform.GetChild(avatarTranformIndex).
                GetComponent<MeshRenderer>().material.color = 
                applicationData.userColors[photonViews[i].Owner.GetPlayerNumber()];
        }
    }
}