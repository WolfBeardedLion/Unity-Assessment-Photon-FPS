using UnityEngine;

[CreateAssetMenu(fileName = "Target_Data", menuName = "Nick/Target_Data", order = 1001)]
public class TargetData : ScriptableObject
{
    public enum TargetType { Static, Dynamic }
    public enum TargetDynamicMode { GoAndStop, GoAndReturn, PingPong }

    [Header("General")]
    public int id = 0;
    public string label = string.Empty;
    public GameObject prefab = null;
    public Material defaultMaterial = null;
    public Color defaultColor = Color.white;
    public TargetType type = TargetType.Static;
    public Vector3 scaling = Vector3.one;
    public float pointValue = 0.0f;
    public float secondsBeforeTargetResets = 5.0f;
    public bool hasBeenHit = false;

    [Header("Dynamic")]
    public TargetDynamicMode movementMode = TargetDynamicMode.GoAndStop;
    public float movementSpeed = 0.0f;
    public Vector3 movementDirection = Vector3.zero;
    public float movementDuration = 0.0f;

    [Space(16)]
    public TargetDynamicMode rotationMode = TargetDynamicMode.GoAndStop;
    public float rotationSpeed = 0.0f;
    public Vector3 rotationDirection = Vector3.zero;
    public float rotationDuration = 0.0f;

    [Space(16)]
    public TargetDynamicMode scaleMode = TargetDynamicMode.GoAndStop;
    public float scaleSpeed = 0.0f;
    public Vector3 scaleDirection = Vector3.zero;
    public float scaleDuration = 0.0f;

    [Header("Set At Runtime - Viewable For Testing")]
    public MeshRenderer targetMeshRenderer = null;
    public GameObject targetGameObject = null;
    public TargetManager targetManager = null;

    private void OnEnable()
    {
        hasBeenHit = false;
        targetMeshRenderer = null;
        targetGameObject = null;
        targetManager = null;
    }
}