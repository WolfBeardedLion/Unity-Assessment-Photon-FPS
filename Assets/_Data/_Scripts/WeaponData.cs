using UnityEngine;

[CreateAssetMenu(fileName = "Weapon_Data", menuName = "Nick/Weapon_Data", order = 101)]
public class WeaponData : ScriptableObject
{
    [Header("General")]
    public int id = 0;
    public string label = string.Empty;
    public float scoreModifier = 1.0f;

    [Header("Visuals")]
    public GameObject prefab = null;
    public Material defaultBarrelMaterial = null;
    public Color defaultBarrelColor = Color.white;
    public Material defaultHandleMaterial = null;
    public Color defaultHandleColor = Color.white;

    [Header("Capability")]
    public float weaponDistance = 0.0f;
    public float[] rateOfFirePerSecond = null;
    public int currentRateOfFireIndex = 0;
    public int ammoCapacity = 0;
    public float reloadSeconds = 0.0f;
    public LayerMask collisionLayers;

    [Header("Set At Runtime - Viewable For Testing")]
    public MeshRenderer weaponBarrelMeshRenderer = null;
    public MeshRenderer weaponHandleMeshRenderer = null;

    private void OnEnable()
    {
        weaponBarrelMeshRenderer = null;
        weaponHandleMeshRenderer = null;
    }
}