using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_Data", menuName = "Nick/Level_Data", order = 1000)]
public class LevelData : ScriptableObject
{
    [Serializable]
    public class TargetGroup
    {
        public string label = string.Empty;
        public TargetData target = null;
        public int numberOfTargetsInGroup = 0;
        public Vector2 xDistanceBetweenTargetsRange = Vector2.zero;
        public Vector2 yDistanceBetweenTargetsRange = Vector2.zero;
        public Vector2 zDistanceBetweenTargetsRange = Vector2.zero;
    }

    [Header("General")]
    public string label = string.Empty;
    public int[] scoreGoalThresholds = null;

    [Header("Visuals")]
    public GameObject prefab = null;
    public Material skymapMaterial = null;
    public float skymapRotation = 0.0f;
    public Material floorMaterial = null;
    [Range(1500, 20000)] public int directionLightTemperature = 6500;
    public float primaryDirectionLightIntensity = 0.0f;
    public float secondaryDirectionLightIntensity = 0.0f;

    [Header("Targets")]
    public TargetGroup[] targetGroups = null;

    [Header("Set At Runtime - Viewable For Testing")]
    public Vector3 playerStartingPosition = Vector3.zero;
    public List<TargetData> targetDataList = new List<TargetData>();
    public List<GameObject> targetGameObjectList = new List<GameObject>();

    private void OnEnable()
    {
        playerStartingPosition = Vector3.zero;
        targetDataList.Clear();
        targetGameObjectList.Clear();
    }
}