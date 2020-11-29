using System;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Open = 0,
    Blocked = 1,
}
public class Node : MonoBehaviour, IComparable<Node>
{
    [Header("Configuration Parameters")]
    [Range(0, 1.5f)]
    [SerializeField] private float borderSize;

    [Header("Node Properties")]
    [SerializeField] private GameObject tile;
    [SerializeField] private GameObject arrow;
    private NodeType nodeType = NodeType.Open;
    private Vector3 worldPosition;
    private float distanceTraveled = Mathf.Infinity;
    public float priority;

    [Header("Node's index in array")]
    private int xIndex;
    private int yIndex;

    [Header("Node's neighbors and previous node")]
    private List<Node> neighbors;
    private Node previous = null;

    //Properties
    public NodeType NodeType { get { return nodeType; } set { nodeType = value; } }
    public Vector3 NodeWorldPos { get { return worldPosition; } set { worldPosition = value; } }
    public float DistanceTraveled { get { return distanceTraveled; } set { distanceTraveled = value; } }
    public int XIndex { get { return xIndex; } set { xIndex = value; } }
    public int YIndex { get { return yIndex; } set { yIndex = value; } }
    public List<Node> Neighbors { get { return neighbors; } set { neighbors = value; } }
    public Node Previous { get { return previous; } set { previous = value; } }


    public int CompareTo(Node other)
    {
        if (this.priority < other.priority)
            return 1;
        else if (this.priority > other.priority)
            return -1;
        else
            return 0;
    }

    //Methods
    public void RotateArrow()
    {
        if (this.previous != null)
        {
            Vector2 difference = this.previous.worldPosition - this.worldPosition;
            difference.Normalize();
            float zRotation = Mathf.Atan2(difference.y, difference.x);
            zRotation *= Mathf.Rad2Deg;

            arrow.SetActive(true);
            Transform pivot = arrow.transform.parent;
            pivot.rotation = Quaternion.Euler(0, 0, zRotation);
        }
    }
    public void ColorNode(Color color)
    {
        SpriteRenderer mySpriteRenderer = tile.GetComponent<SpriteRenderer>();
        mySpriteRenderer.color = color;
    }

    //Initlization
    public void Init()
    {
        gameObject.name = "Node (" + xIndex + "," + yIndex + ")";
        gameObject.transform.position = worldPosition;
        tile.transform.localScale = new Vector3(tile.transform.localScale.x - borderSize, tile.transform.localScale.y - borderSize, tile.transform.localScale.z);
        arrow.SetActive(false);
    }
}
