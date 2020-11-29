using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [Header("Graph Properties")]
    [SerializeField] private Node nodePrefab;
    private Node[,] nodes;
    private List<Node> blockedNodes;
    [Space(10f)]
    [SerializeField] private int column;
    [SerializeField] private int row;
    [Space(10f)]
    [SerializeField] private Pathfinder pathfinder;

    [Header("Pathfinder Configuration")]
    private int xStart = -1;
    private int yStart = -1;
    private int xGoal = -1;
    private int yGoal = -1;

    [Header("Directions")]
    private Vector2[] directions =
    {
        new Vector2(0, 1f),
        new Vector2(0, -1f),
        new Vector2(1f, 0),
        new Vector2(-1f, 0),
        new Vector2(1f, 1f),
        new Vector2(1f, -1f),
        new Vector2(-1f, 1f),
        new Vector2(-1f, -1f)
    };

    [Header("Booleans")]
    private bool canSetWalls = false;
    private bool canSetStartAndGoal = false;
    private bool startPathfinder = false;

    //Properties
    public Node[,] GetNodes { get { return nodes; } }
    public int GetColumn { get { return column; } }
    public int GetRow { get { return row; } }
    public bool CanSetWalls { set { canSetWalls = value; } }
    public bool CanSetStartAndGoal { set { canSetStartAndGoal = value; } }
    public bool StartPathfinder { set { startPathfinder = value; } }

    //Methods
    private void SetBlocks()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 clickPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit2D = Physics2D.Raycast(clickPos, Vector2.zero, 0);

            if (hit2D.collider.tag == "Node")
            {
                Node node = hit2D.collider.gameObject.GetComponent<Node>();

                if (node.NodeType != NodeType.Blocked)
                {
                    node.NodeType = NodeType.Blocked;
                    node.ColorNode(Color.black);
                }
            }
        }
        else if (Input.GetMouseButton(1))
        {
            Vector2 clickPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit2D = Physics2D.Raycast(clickPos, Vector2.zero, 0f);

            if (hit2D.collider.gameObject.tag == "Node")
            {
                Node node = hit2D.collider.gameObject.GetComponent<Node>();

                if (node.NodeType != NodeType.Open)
                {
                    node.NodeType = NodeType.Open;
                    node.ColorNode(Color.white);
                }
            }
        }
    }
    private void SetStartAndGoal()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit2D = Physics2D.Raycast(clickPos, Vector2.zero, 0);

            if (hit2D.collider.tag == "Node")
            {
                Node node = hit2D.collider.gameObject.GetComponent<Node>();
                if (xStart != -1 && yStart != -1)
                {
                    nodes[yStart, xStart].ColorNode(Color.white);
                }

                if (!blockedNodes.Contains(node))
                {
                    xStart = (int)node.NodeWorldPos.x;
                    yStart = (int)node.NodeWorldPos.y;
                    node.ColorNode(Color.green);
                }
                else
                    Debug.LogWarning("[ERROR]: Node is blocked make it open or choose a different start");
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Vector2 clickPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit2D = Physics2D.Raycast(clickPos, Vector2.zero, 0);

            if (hit2D.collider.tag == "Node")
            {
                Node node = hit2D.collider.gameObject.GetComponent<Node>();
                if (xGoal != -1 && yGoal != -1)
                {
                    nodes[yGoal, xGoal].ColorNode(Color.white);
                }

                if (!blockedNodes.Contains(node))
                {
                    xGoal = (int)node.NodeWorldPos.x;
                    yGoal = (int)node.NodeWorldPos.y;
                    node.ColorNode(Color.red);
                }
                else
                    Debug.LogWarning("[ERROR]: Node is blocked make it open or choose a different start/goal");
            }
        }
    }
    private void GetNeighbors()
    {
        blockedNodes = new List<Node>();

        for (int y = 0; y < column; y++)
        {
            for (int x = 0; x < row; x++)
            {
                List<Node> neighbors = new List<Node>();

                foreach (Vector2 direction in directions)
                {
                    int xNew = nodes[y, x].XIndex + (int)direction.x;
                    int yNew = nodes[y, x].YIndex + (int)direction.y;
                    bool withinRange = (yNew >= 0 && yNew < column && xNew >= 0 && xNew < row);

                    if (withinRange && nodes[yNew, xNew].NodeType == NodeType.Open)
                        neighbors.Add(nodes[yNew, xNew]);
                    else if (withinRange && nodes[yNew, xNew].NodeType == NodeType.Blocked)
                        blockedNodes.Add(nodes[yNew, xNew]);
                }

                nodes[y, x].Neighbors = neighbors;
            }
        }
    }
    public float GetNodeDistance(Node source, Node target)
    {
        int dx = Mathf.Abs(source.XIndex - target.XIndex);
        int dy = Mathf.Abs(source.YIndex - target.YIndex);

        int min = Mathf.Min(dx, dy);
        int max = Mathf.Max(dx, dy);

        int diagonalSteps = min;
        int straightSteps = max - min;

        return (1.4f * diagonalSteps + straightSteps);
    }

    //Initialization
    private void Init()
    {
        nodes = new Node[column, row];

        for (int y = 0; y < column; y++)
        {
            for (int x = 0; x < row; x++)
            {
                Node newNode = Instantiate(nodePrefab);

                newNode.NodeType = NodeType.Open;
                newNode.XIndex = x;
                newNode.YIndex = y;
                newNode.NodeWorldPos = new Vector3(x, y, 0);
                nodes[y, x] = newNode;

                newNode.Init();

                //All nodes are open and white by default
                newNode.ColorNode(Color.white);
            }
        }
    }

    private void Start()
    {
        Init();
    }
    private void Update()
    {
        if (canSetWalls == true)
            SetBlocks();
        else
            GetNeighbors();

        if (canSetStartAndGoal == true)
            SetStartAndGoal();

        if (startPathfinder == true)
        {
            if (pathfinder.pathfinderMode == Mode.BreadthFirstSearch)
                StartCoroutine(pathfinder.BFS(nodes[yStart, xStart], nodes[yGoal, xGoal]));
            else if (pathfinder.pathfinderMode == Mode.Dijkstra)
                StartCoroutine(pathfinder.Dijkstra(nodes[yStart, xStart], nodes[yGoal, xGoal]));
            else if (pathfinder.pathfinderMode == Mode.AStar)
                StartCoroutine(pathfinder.AStar(nodes[yStart, xStart], nodes[yGoal, xGoal]));
            startPathfinder = false;
        }
    }
}
