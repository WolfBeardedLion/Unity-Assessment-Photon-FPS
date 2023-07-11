using UnityEngine;
using static LevelData;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private ApplicationData applicationData = null;

    private Light primaryDirectionalLight = null;
    private Light secondaryDirectionalLight = null;
    private MeshRenderer floorMeshRenderer = null;
    private Transform playerStartingPositionTransform = null;
    private Transform targetZoneOriginTransform = null;
    private Vector3 offset = Vector3.zero;

    private const int primaryDirectionLightTranformIndex = 0;
    private const int secondaryDirectionLightTranformIndex = 1;
    private const int floorTranformIndex = 3;
    private const int playerStartingPositionTransformIndex = 4;
    private const int targetZoneOriginTranformIndex = 5;

    private void Awake()
    {
        primaryDirectionalLight = 
            transform.GetChild(primaryDirectionLightTranformIndex).GetComponent<Light>();
        secondaryDirectionalLight =
            transform.GetChild(secondaryDirectionLightTranformIndex).GetComponent<Light>();
        floorMeshRenderer = 
            transform.GetChild(floorTranformIndex).GetComponent<MeshRenderer>();

        // Apply the Player Start Position transform to the level data, so that the Player
        // transform can utilize for it's initial spawn position; this approach decouple the
        // Player script and the Level Mananer script.
        playerStartingPositionTransform = transform.GetChild(playerStartingPositionTransformIndex);
        applicationData.levels[applicationData.
            currentLevelIndex].playerStartingPosition = playerStartingPositionTransform.position;
        targetZoneOriginTransform = transform.GetChild(targetZoneOriginTranformIndex);
    }

    private void OnEnable()
    {
        SetupEnvironemnt();
        SetupTargets();
    }

    private void OnDisable()
    {
        for (int i = applicationData.levels[applicationData.currentLevelIndex].
            targetGameObjectList.Count - 1; i >= 0; i--)
        {
            DestroyATarget(i);
        }
    }

    private void SetupEnvironemnt()
    {
        LevelData levelData = applicationData.levels[applicationData.currentLevelIndex];

        RenderSettings.skybox = levelData.skymapMaterial;
        RenderSettings.skybox.SetFloat("_Rotation", levelData.skymapRotation);

        primaryDirectionalLight.colorTemperature = levelData.directionLightTemperature;
        primaryDirectionalLight.intensity = levelData.primaryDirectionLightIntensity;

        secondaryDirectionalLight.colorTemperature = levelData.directionLightTemperature;
        secondaryDirectionalLight.intensity = levelData.secondaryDirectionLightIntensity;

        floorMeshRenderer.material = levelData.floorMaterial;
    }

    private void SetupTargets()
    {
        LevelData levelData = applicationData.levels[applicationData.currentLevelIndex];

        TargetGroup[] targetGroups = levelData.targetGroups;
        for (int i = 0; i < targetGroups.Length; i++)
        {
            if (targetGroups[i] == null)
            {
                Debug.Log("Target Group #" + i + " is null.");
                continue;
            }

            if(targetGroups[i].numberOfTargetsInGroup <= 0)
            {
                Debug.Log("Target Group: " + targetGroups[i].label + " has 0 targets in the group.");
                continue;
            }

            TargetData origialTargetData = targetGroups[i].target;

            if (origialTargetData.prefab == null)
            {
                Debug.Log("Target Group: " + targetGroups[i].label
                    + "'s " + origialTargetData.label + " target's prefab is null.");
                continue;
            }

            for (int j = 0; j < targetGroups[i].numberOfTargetsInGroup; j++)
            {
                TargetData newTargetData = origialTargetData.Clone();

                GameObject newTarget = Instantiate(newTargetData.prefab, targetZoneOriginTransform);
                newTarget.transform.name = newTargetData.label;
                newTarget.transform.localScale = newTargetData.scaling;

                newTarget.transform.position = DetermineTargetPosition(targetGroups[i], j);

                if(newTargetData.type == TargetData.TargetType.Static)
                {
                    newTarget.isStatic = true;
                }
                else
                {
                    newTarget.isStatic = false;
                }

                newTargetData.targetGameObject = newTarget.gameObject;

                newTargetData.targetManager = newTarget.GetComponent<TargetManager>();
                newTargetData.targetManager.SetTargetData(newTargetData);

                newTargetData.targetMeshRenderer = newTarget.GetComponent<MeshRenderer>();
                newTargetData.targetMeshRenderer.material = 
                    newTargetData.defaultMaterial;
                newTargetData.targetMeshRenderer.material.color =
                    newTargetData.defaultColor;

                levelData.targetDataList.Add(newTargetData);
                levelData.targetGameObjectList.Add(newTarget);
            }
        }
    }

    private Vector3 DetermineTargetPosition(TargetGroup _targetGroup, int targetIndex)
    {
        Vector3 newPosition = Vector3.zero;

        if (targetIndex == 0)
        {
            newPosition = targetZoneOriginTransform.position;
        }
        else
        {
            float randomX = Random.Range(_targetGroup.xDistanceBetweenTargetsRange.x,
                        _targetGroup.xDistanceBetweenTargetsRange.y);
            float randomY = Random.Range(_targetGroup.yDistanceBetweenTargetsRange.x,
                    _targetGroup.yDistanceBetweenTargetsRange.y);
            float randomZ = Random.Range(_targetGroup.zDistanceBetweenTargetsRange.x,
                    _targetGroup.zDistanceBetweenTargetsRange.y);

            if (targetIndex % 2 == 0)
            {
                offset += new Vector3(-1.0f * randomX, randomY, randomZ);
                newPosition = targetZoneOriginTransform.position + offset;
            }
            else if (targetIndex % 2 == 1)
            {
                offset += new Vector3(randomX, randomY, randomZ);
                newPosition = targetZoneOriginTransform.position + offset;
            }
        }

        return newPosition;
    }

    private void DestroyATarget(int _targetIndex)
    {
        LevelData levelData = applicationData.levels[applicationData.currentLevelIndex];

        Destroy(levelData.targetGameObjectList[_targetIndex]);
        levelData.targetGameObjectList.RemoveAt(_targetIndex);
    }
}