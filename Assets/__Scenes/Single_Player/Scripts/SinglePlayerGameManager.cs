using System.Collections.Generic;
using UnityEngine;

public class SinglePlayerGameManager : MonoBehaviour, IGameManager
{
    [SerializeField] internal ApplicationData applicationData = null;

    private GameObject currentLevelManager = null;
    private List<GameObject> currentPlayers = new List<GameObject>();

    private const int playerAvatarTransformIndex = 2;

    private void Awake()
    {
        CreateLevelManager();
    }

    private void OnEnable()
    {
        CreateAllLocalPlayers();
    }

    private void Start()
    {
#if UNITY_EDITOR

        // Within the Unity Editor, the PUN 2 SDK automatically adds a Photon Script to the scene.
        // Because the single player mode is built for it's unique build that cuts out all of the
        // Photon Libraries, we want to remove it from the scene when developing in the Unity Editor.
        Destroy(GameObject.Find("PhotonMono"));

#endif
    }

    private void OnDisable()
    {
        DestroyAllLocalPlayers();
    }

    private void CreateAllLocalPlayers()
    {
        for (int i = 0; i < applicationData.numberOfLocalPlayers; i++)
        {
            CreatePlayerCharacter(i);
        }
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
        if (applicationData.users[_userIndex] == null)
        {
            Debug.Log("Application Data's User #" + _userIndex + " is null.");
            return;
        }

        GameObject newPlayer = Instantiate(applicationData.users[_userIndex].prefab);
        newPlayer.transform.name =
            "Player: " + applicationData.users[_userIndex].username;
        applicationData.users[_userIndex].playerMeshRenderer =
            newPlayer.transform.GetChild(playerAvatarTransformIndex).GetComponent<MeshRenderer>();
        applicationData.users[_userIndex].playerMeshRenderer.material =
            applicationData.users[_userIndex].defaultMaterial;
        applicationData.users[_userIndex].playerMeshRenderer.material.color =
            applicationData.users[_userIndex].defaultColor;

        IUserManager[] newUserManagers = newPlayer.GetComponents<IUserManager>();
        for (int i = 0; i < newUserManagers.Length; i++)
        {
            newUserManagers[i].SetPlayer(applicationData.users[_userIndex].id);
        }

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