using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGameManager : MonoBehaviourPunCallbacks, IGameManager
{
    [SerializeField] private ApplicationData applicationData = null;

    private GameObject currentLevelManager = null;
    private List<GameObject> currentPlayers = new List<GameObject>();

    private const int playerAvatarTransformIndex = 2;

    private void Awake()
    {
        applicationData.currentLevelIndex = 1;

        CreateLevelManager();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        CreatePlayerCharacter(0);
    }

    public override void OnDisable()
    {
        base.OnEnable();

        DestroyAllLocalPlayers();
    }

    private void DestroyAllLocalPlayers()
    {
        for (int i = applicationData.users.Count - 1; i >= 0; i--)
        {
            DestroyPlayerCharacter(i);
        }
    }

    private void CreateLevelManager()
    {
        currentLevelManager =
            Instantiate(applicationData.levels[applicationData.currentLevelIndex].prefab);
        currentLevelManager.transform.name =
            applicationData.levels[applicationData.currentLevelIndex].label;
    }

    private void CreatePlayerCharacter(int _userIndex)
    {
        int playerIndex = PhotonNetwork.LocalPlayer.GetPlayerNumber();

        applicationData.networkUsers[_userIndex].id = playerIndex;
        applicationData.networkUsers[_userIndex].defaultColor = 
            applicationData.userColors[playerIndex];

        Vector3 startingPosition =
            applicationData.levels[applicationData.currentLevelIndex].
            playerStartingPosition + (Vector3.left * 2.0f * applicationData.networkUsers[_userIndex].id);

        GameObject newPlayer = PhotonNetwork.Instantiate(
            applicationData.networkUsers[_userIndex].prefab.name,
            startingPosition, Quaternion.identity, 0);

        newPlayer.transform.name =
            "Player: " + applicationData.networkUsers[_userIndex].username;
        applicationData.networkUsers[_userIndex].playerMeshRenderer =
            newPlayer.transform.GetChild(playerAvatarTransformIndex).GetComponent<MeshRenderer>();
        applicationData.networkUsers[_userIndex].playerMeshRenderer.material =
            applicationData.networkUsers[_userIndex].defaultMaterial;
        applicationData.networkUsers[_userIndex].playerMeshRenderer.material.color =
            applicationData.networkUsers[_userIndex].defaultColor;

        currentPlayers.Add(newPlayer);
    }

    private void DestroyPlayerCharacter(int _userIndex)
    {
        if (applicationData.users[_userIndex] == null)
        {
            Debug.Log("Application Data's User #" + _userIndex + " is null.");

            return;
        }

        Destroy(currentPlayers[_userIndex]);
        currentPlayers.RemoveAt(_userIndex);
    }
}