using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NetworkUserWeaponManager : MonoBehaviourPunCallbacks, IUserManager
{
    [SerializeField] private ApplicationData applicationData = null;

    private int playerIndex = 0;
    private UserData userData = null;
    private Transform weaponPositionTransform = null;
    private WeaponData activeWeaponData = null;
    private bool isWeaponReadyToBeFired = true;
    private bool shouldFireWeapon = false;
    private float currentWaitToFireSeconds = 0.0f;
    private List<WeaponData> equippedWeaponData = new List<WeaponData>();
    private Transform activeWeaponTransform = null;
    private PhotonView playerPhotonView = null;

    private const int weaponPositionTranformIndex = 0;
    private const int weaponBarrelTransformIndex = 0;
    private const int weaponHandleTransformIndex = 1;

    public void SetPlayer(int _newPlayerIndex)
    {
        playerIndex = _newPlayerIndex;
        userData = applicationData.users[playerIndex];
    }

    public float GetCurrentMovementSpeed()
    {
        return currentWaitToFireSeconds;
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

        InstantiateEquippedWeapons();
        SetActiveWeapon();
    }

    private void FixedUpdate()
    {
        if (playerPhotonView.IsMine == false)
        {
            return;
        }

        if (shouldFireWeapon == true)
        {
            shouldFireWeapon = false;
            PlayerFiredTheirWeapon();
        }
    }

    private void Update()
    {
        if (playerPhotonView.IsMine == false)
        {
            return;
        }

        if (isWeaponReadyToBeFired == true)
        {
            if (Input.GetButtonDown("Fire1") == true)
            {
                isWeaponReadyToBeFired = false;
                shouldFireWeapon = true;
            }
        }
        else if (isWeaponReadyToBeFired == false)
        {
            currentWaitToFireSeconds += Time.deltaTime;
            if (currentWaitToFireSeconds >= activeWeaponData
                .rateOfFirePerSecond[activeWeaponData.currentRateOfFireIndex])
            {
                currentWaitToFireSeconds = 0.0f;
                isWeaponReadyToBeFired = true;
            }
        }

        // Example of how player could rotate through their equipped weapons;
        // player has an inventory of weapons, but they can not equip them all at once.
        if (Input.GetKeyDown(KeyCode.Keypad1) == true)
        {
            userData.activeWeaponIndex += 1;

            if (userData.activeWeaponIndex >= userData.weaponsEquippedIndexes.Length)
            {
                userData.activeWeaponIndex = 0;
            }

            SetActiveWeapon();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad2) == true)
        {
            userData.activeWeaponIndex -= 1;

            if (userData.activeWeaponIndex < 0)
            {
                userData.activeWeaponIndex = userData.weaponsEquippedIndexes.Length - 1;
            }

            SetActiveWeapon();
        }
    }

    private void OnInitialize()
    {
        playerPhotonView = GetComponent<PhotonView>();
        weaponPositionTransform = transform.GetChild(weaponPositionTranformIndex).transform;
    }

    private void SetDefaultValues()
    {
        if (playerPhotonView.IsMine == false)
        {
            return;
        }

        userData = applicationData.networkUsers[0];
        isWeaponReadyToBeFired = true;
        currentWaitToFireSeconds = 0.0f;
    }

    private void InstantiateEquippedWeapons()
    {
        for (int i = 0; i < userData.weaponsEquippedIndexes.Length; i++)
        {
            if ((userData.weaponsEquippedIndexes[i] < 0) ||
                (userData.weaponsEquippedIndexes[i] >= userData.weaponInventory.Length))
            {
                Debug.Log("Equipped Weapon Index #" + userData.weaponsEquippedIndexes[i] +
                    " is out of range of " + userData.username + "'s weapon inventory.");

                continue;
            }

            WeaponData currentWeaponData =
                userData.weaponInventory[userData.weaponsEquippedIndexes[i]];

            GameObject newWeapon = Instantiate(currentWeaponData.prefab, weaponPositionTransform);
            newWeapon.name = userData.username + "_" + currentWeaponData.label;

            currentWeaponData.weaponBarrelMeshRenderer =
                newWeapon.transform.GetChild(weaponBarrelTransformIndex).
                GetComponent<MeshRenderer>();

            currentWeaponData.weaponHandleMeshRenderer =
                newWeapon.transform.GetChild(weaponHandleTransformIndex).
                GetComponent<MeshRenderer>();

            currentWeaponData.weaponBarrelMeshRenderer.material =
                currentWeaponData.defaultBarrelMaterial;
            currentWeaponData.weaponBarrelMeshRenderer.material.color =
                currentWeaponData.defaultBarrelColor;

            currentWeaponData.weaponHandleMeshRenderer.material =
                currentWeaponData.defaultHandleMaterial;
            currentWeaponData.weaponHandleMeshRenderer.material.color =
                currentWeaponData.defaultHandleColor;

            activeWeaponTransform = newWeapon.transform;

            equippedWeaponData.Add(currentWeaponData);
        }
    }

    private void SetActiveWeapon()
    {
        activeWeaponData = userData.weaponInventory[userData.activeWeaponIndex];
    }

    private void PlayerFiredTheirWeapon()
    {
        RaycastHit rayHitData;
        if (Physics.Raycast(activeWeaponTransform.position,
            transform.TransformDirection(Vector3.forward), out rayHitData,
            activeWeaponData.weaponDistance, activeWeaponData.collisionLayers))
        {

#if UNITY_EDITOR

            Debug.DrawRay(activeWeaponTransform.position,
                transform.TransformDirection(Vector3.forward) * rayHitData.distance,
                Color.green);

#endif
            
            for (int i = 0; i < applicationData
                .levels[applicationData.currentLevelIndex].targetDataList.Count; i++)
            {
                if (applicationData
                    .levels[applicationData.currentLevelIndex].targetDataList[i] == null)
                {
                    Debug.Log(applicationData.levels[applicationData.currentLevelIndex].label
                        + "'s target data #" + i + " is null.");

                    continue;
                }

                if (applicationData
                    .levels[applicationData.currentLevelIndex].targetDataList[i]
                    .targetGameObject.GetInstanceID() !=
                    rayHitData.transform.gameObject.GetInstanceID())
                {
                    continue;
                }

                if(applicationData.levels[applicationData.currentLevelIndex]
                    .targetDataList[i].hasBeenHit == true)
                {
                    break;
                }

                playerPhotonView.RPC("RPCTargetWasHit", RpcTarget.All, i, 
                    userData.defaultColor.r, 
                    userData.defaultColor.g, 
                    userData.defaultColor.b);

                break;
            }
        }
        else
        {

#if UNITY_EDITOR

            Debug.DrawRay(activeWeaponTransform.position,
                transform.TransformDirection(Vector3.forward) * rayHitData.distance,
                Color.red);

#endif

            Debug.Log(userData.username + " missed their target!");
            userData.totalScore -= userData.missedShotScorePenalty;
            if (userData.totalScore < 0.0f)
            {
                userData.totalScore = 0.0f;
            }
        }
    }

    [PunRPC]
    private void RPCTargetWasHit(int _targetIndex, float _colorR, float _colorG, float _colorB)
    {
        SetTargetNewColor(_targetIndex, new Color(_colorR, _colorG, _colorB));
    }

    private void SetTargetNewColor(int _targetIndex, Color _newColor)
    {
        applicationData.levels[applicationData.
            currentLevelIndex].targetDataList[_targetIndex].
            targetManager.UserHitTarget(_newColor);
    }
}