using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
    BreadthFirstSearch = 0,
    Dijkstra = 1,
    AStar = 2
}
public class Pathfinder : MonoBehaviour
{
    [Header("Pathfinder Configuration")]
    public Mode pathfinderMode = Mode.BreadthFirstSearch;
    [SerializeField] private bool showArrows = true;
    [SerializeField] private bool exitOnGoal = true;

    [Header("Componenets")]
    [SerializeField] private Graph graph;

    [Header("Varibales")]
    private Queue<Node> toVisitNodesBFS;
    private PriorityQueue<Node> toVisitNodesDijkstra;
    private PriorityQueue<Node> toVisitNodesAStar;
    private List<Node> visitedNodes;
    private List<Node> pathNodes;
    private float totalDistanceTraveled = 0;

    [Header("Colors")]
    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color toVisitColor = Color.magenta;
    public Color visitedColor = Color.gray;
    public Color pathColor = Color.cyan;

    //Searching Algorithms
    public IEnumerator BFS(Node start, Node goal, float timeStep = 0.01f)
    {
        toVisitNodesBFS = new Queue<Node>();
        visitedNodes = new List<Node>();
        pathNodes = new List<Node>();

        toVisitNodesBFS.Enqueue(start);

        while (toVisitNodesBFS.Count > 0)
        {
            Node currentNode = toVisitNodesBFS.Dequeue();

            if (exitOnGoal == true)
                if (currentNode.name == goal.name)
                    break;

            if (!visitedNodes.Contains(currentNode))
            {
                visitedNodes.Add(currentNode);

                List<Node> neighbors = currentNode.Neighbors;
                foreach (Node neighbor in neighbors)
                {
                    if (!visitedNodes.Contains(neighbor) && !toVisitNodesBFS.Contains(neighbor))
                    {
                        neighbor.Previous = currentNode;
                        if (showArrows == true)
                            neighbor.RotateArrow();
                        toVisitNodesBFS.Enqueue(neighbor);
                    }
                }

                ColorNodes(start, goal);
               yield return new WaitForSeconds(timeStep);
            }
        }

        GetPathNodes(goal);
        ColorNodes(start, goal);
    }
    public IEnumerator Dijkstra(Node start, Node goal, float timeStep = 0.01f)
    {
        toVisitNodesDijkstra = new PriorityQueue<Node>();
        visitedNodes = new List<Node>();
        pathNodes = new List<Node>();

        toVisitNodesDijkstra.Enqueue(start);
        start.DistanceTraveled = 0;

        while (toVisitNodesDijkstra.Count > 0)
        {
            Node currentNode = toVisitNodesDijkstra.Dequeue();

            if (exitOnGoal == true)
                if (currentNode.name == goal.name)
                    break;

            if (!visitedNodes.Contains(currentNode))
            {
                visitedNodes.Add(currentNode);

                List<Node> neighbors = currentNode.Neighbors;
                foreach (Node neighbor in neighbors)
                {
                    if (!visitedNodes.Contains(neighbor))
                    {
                        float distanceToNeighbor = graph.GetNodeDistance(currentNode, neighbor);
                        float distanceTraveled = distanceToNeighbor + currentNode.DistanceTraveled;

                        if (float.IsPositiveInfinity(neighbor.DistanceTraveled) || distanceTraveled < neighbor.DistanceTraveled)
                        {
                            neighbor.Previous = currentNode;
                            if (showArrows == true)
                                neighbor.RotateArrow();
                            neighbor.DistanceTraveled = distanceTraveled;

                        }

                        if (!toVisitNodesDijkstra.Contains(neighbor))
                        {
                            neighbor.priority = neighbor.DistanceTraveled;
                            toVisitNodesDijkstra.Enqueue(neighbor);
                        }
                    }
                }
            }

            ColorNodes(start, goal);
            yield return new WaitForSeconds(timeStep);
        }

        GetPathNodes(goal);
        StartCoroutine(ColorPathNodes(start, goal));
        Debug.Log(goal.DistanceTraveled);
    }
    public IEnumerator AStar(Node start, Node goal, float timeStep = 0.01f)
    {
        toVisitNodesAStar = new PriorityQueue<Node>();
        visitedNodes = new List<Node>();
        pathNodes = new List<Node>();

        toVisitNodesAStar.Enqueue(start);
        start.DistanceTraveled = 0;

        while (toVisitNodesAStar.Count > 0)
        {
            Node currentNode = toVisitNodesAStar.Dequeue();

            if (exitOnGoal == true)
                if (currentNode.name == goal.name)
                    break;

            if (!visitedNodes.Contains(currentNode))
            {
                visitedNodes.Add(currentNode);

                List<Node> neighbors = currentNode.Neighbors;
                foreach (Node neighbor in neighbors)
                {
                    if (!visitedNodes.Contains(neighbor))
                    {
                        float distanceToNeighbor = graph.GetNodeDistance(currentNode, neighbor);
                        float distanceTraveled = distanceToNeighbor + currentNode.DistanceTraveled;

                        if (float.IsPositiveInfinity(neighbor.DistanceTraveled) || distanceTraveled < neighbor.DistanceTraveled)
                        {
                            neighbor.Previous = currentNode;
                            if (showArrows == true)
                                neighbor.RotateArrow();
                            neighbor.DistanceTraveled = distanceTraveled;
                        }

                        if (!toVisitNodesAStar.Contains(neighbor))
                        {
                            int distanceToGoal = (int)graph.GetNodeDistance(neighbor, goal);
                            neighbor.priority = neighbor.DistanceTraveled + distanceToGoal;
                            toVisitNodesAStar.Enqueue(neighbor);
                        }
                    }
                }
            }

            ColorNodes(start, goal);
            yield return new WaitForSeconds(timeStep);
        }

        GetPathNodes(goal);
        StartCoroutine(ColorPathNodes(start, goal));
        Debug.Log(goal.DistanceTraveled);

    }

    private void ColorNodes(Node start, Node goal)
    {
        if (pathfinderMode == Mode.BreadthFirstSearch)
        {
            foreach (Node node in toVisitNodesBFS)
                node.ColorNode(toVisitColor);
        }
        else if (pathfinderMode == Mode.Dijkstra)
        {
            List<Node> frontierNodes = toVisitNodesDijkstra.ToList();
            for (int i = 0; i < toVisitNodesDijkstra.Count; i++)
            {
                frontierNodes[i].ColorNode(toVisitColor);
            }
        }
        else if (pathfinderMode == Mode.AStar)
        {
            List<Node> frontierNodes = toVisitNodesAStar.ToList();
            foreach (Node node in frontierNodes)
                node.ColorNode(toVisitColor);
        }
        foreach (Node node in visitedNodes)
            node.ColorNode(visitedColor);

        start.ColorNode(startColor);
        goal.ColorNode(goalColor);
    }
    private IEnumerator ColorPathNodes(Node start, Node goal)
    {
        foreach (Node pathNode in pathNodes)
        {
            if (pathNode != start && pathNode != goal)
                pathNode.ColorNode(pathColor);
            yield return new WaitForSeconds(0.05f);
        }
    }
    public List<Node> GetPathNodes(Node endNode)
    {
        pathNodes.Add(endNode);

        Node currentNode = endNode.Previous;

        while (currentNode != null)
        {
            pathNodes.Insert(0, currentNode);
            currentNode = currentNode.Previous;
        }

        return pathNodes;
    }
}
