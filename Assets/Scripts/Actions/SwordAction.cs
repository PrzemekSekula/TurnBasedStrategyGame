using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAction : BaseAction
{
    public static event EventHandler OnAnySwordHit;
    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    private enum State
    {
        SwingingSwordBeforeHit,
        SwingingAfterBeforeHit,
    } 

    private int maxSwordDistance = 1;
    private State state;
    private float stateTimer;
    private Unit targetUnit;



    private void Update() {
        if (!isActive) 
        {
            return;
        }

        stateTimer -= Time.deltaTime;

        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - transform.position).normalized;
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward, aimDir, rotateSpeed * Time.deltaTime);
                break;
            case State.SwingingAfterBeforeHit:
                break;

        }

        if (stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (state)
        {
            case State.SwingingSwordBeforeHit:
                state = State.SwingingAfterBeforeHit;
                float afterHitStateTime = 0.5f;
                stateTimer = afterHitStateTime;
                targetUnit.Damage(100);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingingAfterBeforeHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Sword";
    }
    

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 200,
        };
    }
    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validActionGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        List<GridPosition> neighborGridPositionList = unitGridPosition.FindNeighbors(maxSwordDistance);
        foreach (GridPosition testGridPosition in neighborGridPositionList)
        {
            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
            {
                // Invalid grid position
                continue;
            }

            if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid Position is empty, no Unit
                continue;
            }

            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

            if (targetUnit.IsEnemy() == unit.IsEnemy())
            {
                // Target Unit is on the same team
                continue;
            }


            validActionGridPositionList.Add(testGridPosition);
        }

        return validActionGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        state = State.SwingingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;
        stateTimer = beforeHitStateTime;
        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);

        ActionStart(onActionComplete);
    }

    public int GetMaxSwordDistance()
    {
        return maxSwordDistance;
    }


}
