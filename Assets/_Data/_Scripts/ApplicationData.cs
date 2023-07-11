using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Application_Data", menuName = "Nick/Application_Data", order = 0)]
public class ApplicationData : ScriptableObject
{
    [Header("Default Data")]
    public Color[] userColors = null;

    [Header("Session Data")]
    public int numberOfLocalPlayers = 1;
    public List<UserData> users = new List<UserData>();
    public List<UserData> networkUsers = new List<UserData>();

    [Header("Level Data")]
    public LevelData[] levels = null;
    public int currentLevelIndex = 0;

    private void OnEnable()
    {
        currentLevelIndex = 0;
    }
}