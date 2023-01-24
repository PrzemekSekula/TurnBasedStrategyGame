using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PathFindingGridDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro gCostText;
    [SerializeField] private TextMeshPro hCostText;
    [SerializeField] private TextMeshPro fCostText;
    [SerializeField] private SpriteRenderer isWalkableSpriteRenderer;

    private PathNode pathNode;
    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        pathNode = (PathNode)gridObject;
    }
    // Update is called once per frame
    protected override void Update()
    {
        if (pathNode == null)
        {
            return;
        }
        
        base.Update();
        gCostText.text = pathNode.getGCost().ToString();
        hCostText.text = pathNode.getHCost().ToString();
        fCostText.text = pathNode.getFCost().ToString();
        isWalkableSpriteRenderer.color = pathNode.IsWalkable() ? Color.green : Color.red;

    }
}
