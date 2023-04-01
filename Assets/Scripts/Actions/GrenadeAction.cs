using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform grenadeProjectilePrefab;
    [SerializeField] private LayerMask obstaclesLayerMask;

    private int maxThrowDistance = 7;

    private void Update() 
    {
        if (!isActive) 
        {
            return;
        }
    }

    public override string GetActionName()
    {
        return "Grenade";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0,
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validActionGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        List<GridPosition> neighborGridPositionList = unitGridPosition.FindNeighbors(maxThrowDistance);

        foreach (GridPosition testGridPosition in neighborGridPositionList)
        {

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
            {
                // Invalid grid position
                continue;
            }

            Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
            Vector3 targetWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
            Vector3 throwDir = (targetWorldPosition - unitWorldPosition).normalized;
            float unitShoulderHeight = 1.7f;
            if (Physics.Raycast(
                unitWorldPosition + Vector3.up * unitShoulderHeight,
                throwDir,
                Vector3.Distance(unitWorldPosition, targetWorldPosition),
                obstaclesLayerMask
                ))
            {
                // Blocked by obstacle
                continue;
            }
            validActionGridPositionList.Add(testGridPosition);
        }        

        return validActionGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(
            grenadeProjectilePrefab,
            unit.GetWorldPosition(),
            Quaternion.identity
        );
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenateBehaviourComplete);
        ActionStart(onActionComplete);
    }

    private void OnGrenateBehaviourComplete()
    {
        ActionComplete();
    }
}

