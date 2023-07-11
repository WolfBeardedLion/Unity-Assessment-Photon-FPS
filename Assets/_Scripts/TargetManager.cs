using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private TargetData targetData = null;

    private float currentSecondsBeforeReset = 0.0f;

    public void SetTargetData(TargetData _targetData)
    {
        targetData = _targetData;
    }

    public void UserHitTarget(UserData _userWhoHitData, WeaponData _weaponData, RaycastHit hit)
    {
        if (targetData == null)
        {
            Debug.Log("UserHitTarget's TargetData is null");
            return;
        }

        if (_userWhoHitData == null)
        {
            Debug.Log("UserHitTarget Function's UserData parameter is null");
            return;
        }

        if (_weaponData == null)
        {
            Debug.Log("UserHitTarget Function's WeaponData parameter is null");
            return;
        }

        float amountOfPointsScored = targetData.pointValue * _weaponData.scoreModifier;
        _userWhoHitData.totalScore += amountOfPointsScored;

        Debug.Log(_userWhoHitData.username + " has hit their target with a "
            + _weaponData.label + "from " + hit.distance + " meters away for "
            + amountOfPointsScored + " points.");

        targetData.targetMeshRenderer.material.color = _userWhoHitData.defaultColor;
        targetData.hasBeenHit = true;
    }

    public void UserHitTarget(Color _userColor)
    {
        if (targetData == null)
        {
            Debug.Log("UserHitTarget's TargetData is null");
            return;
        }

        targetData.targetMeshRenderer.material.color = _userColor;
        targetData.hasBeenHit = true;
    }

    private void Update()
    {
        if (targetData == null)
        {
            Debug.Log("No Target Data Set.");
            return;
        }

        if(targetData.hasBeenHit == false)
        {
            return;
        }

        currentSecondsBeforeReset += Time.deltaTime;
        if(currentSecondsBeforeReset >= targetData.secondsBeforeTargetResets)
        {
            currentSecondsBeforeReset = 0.0f;
            targetData.hasBeenHit = false;
            targetData.targetMeshRenderer.material.color = targetData.defaultColor;
        }
    }
}