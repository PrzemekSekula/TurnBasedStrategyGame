using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyUnitList;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("UnitManager already exists");
            Destroy(gameObject);
        }

        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();
    }

    private void Start() 
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;   
    }

    private void Unit_OnAnyUnitSpawned(object sender, System.EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Add(unit);

        if (unit.IsEnemy())
        {
            if (enemyUnitList == null)
            {
                enemyUnitList = new List<Unit>();
            }
            enemyUnitList.Add(unit);
        }
        else
        {
            if (friendlyUnitList == null)
            {
                friendlyUnitList = new List<Unit>();
            }
            friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, System.EventArgs e)
    {
        Unit unit = sender as Unit;

        unitList.Remove(unit);

        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);
        }
        else
        {
            friendlyUnitList.Remove(unit);
        }
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public List<Unit> GetFriendlyUnitList()
    {
        return friendlyUnitList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        return enemyUnitList;
    }

}
