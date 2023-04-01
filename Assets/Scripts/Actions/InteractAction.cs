using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int maxInteractDistance = 1;

    private void Update() {
        if (!isActive) 
        {
            return;
        }
    }

    public override string GetActionName()
    {
        return "Interact";
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition, 
            actionValue = 0
        };
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validActionGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        List<GridPosition> neighborGridPositionList = unitGridPosition.FindNeighbors(maxInteractDistance);

        foreach (GridPosition testGridPosition in neighborGridPositionList)
        {
            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
            {
                // Invalid grid position
                continue;
            }

            IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
            if (interactable == null)
            {
                continue;
            }

            validActionGridPositionList.Add(testGridPosition);
        }

        return validActionGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactable = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactable.Interact(OnInteractComplete);                
        ActionStart(onActionComplete);
        
    }
    private void OnInteractComplete()
    {
        ActionComplete();
    }

}
