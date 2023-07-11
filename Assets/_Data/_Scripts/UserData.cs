using UnityEngine;

[CreateAssetMenu(fileName = "User_Data", menuName = "Nick/User_Data", order = 100)]
public class UserData : ScriptableObject
{
    [Header("General")]
    public int id = 0;
    public string username = string.Empty;
    public float totalScore = 0.0f;
    public float missedShotScorePenalty = 0.0f;

    [Header("Visuals")]
    public GameObject prefab = null;
    public Material defaultMaterial = null;
    public Color defaultColor = Color.white;

    [Header("Movement")]
    public float movementSpeed = 0.0f;
    public float runSpeed = 0.0f;
    public float runMaxStamina = 0.0f;
    public float rateOfStaminaLoss = 0.0f;
    public float rateOfStaminaGain = 0.0f;
    public float rotationSpeed = 0.0f;
    [Range(0f, 90f)] public float yRotationLimit = 87f;

    [Header("Weapons")]
    public int activeWeaponIndex = 0;
    public int[] weaponsEquippedIndexes = null;
    public WeaponData[] weaponInventory = null;

    [Header("Set At Runtime - Viewable For Testing")]
    public float currentRunStamina = 0.0f;
    public MeshRenderer playerMeshRenderer = null;

    public void ResetGameRelatedVariables()
    {
        currentRunStamina = runMaxStamina;
        totalScore = 0.0f;
        id = 0;
        defaultColor = Color.green;
    }

    private void OnEnable()
    {
        ResetGameRelatedVariables();
        playerMeshRenderer = null;
    }
}