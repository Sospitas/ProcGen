using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Kruskal : MonoBehaviour
{
	// List to store all of the edges
	public List<Edge> edges = new List<Edge>();
	
	public Vector2 gridSize;
	
	public bool generatingMaze = false;
	
	public Transform wallGroup;
	public Transform gridNode, wall;
	public Transform [,] grid;
	
	public Transform lastVertex, currentVertex;
	
	public List<GameObject> mazeList = new List<GameObject>();
	public List<GameObject> wallList = new List<GameObject>();
	
	public List<int> edgeWeightsInt = new List<int>();
	
	private static int treeID;
	
	// Time taken to generate the maze
	private float generationTime;

	// Use this for initialization
	void Start ()
	{
		grid = new Transform[(int)gridSize.x, (int)gridSize.y];
		lastVertex = currentVertex = null;
		mazeList.Clear();
		wallList.Clear();
		edges.Clear();
		generationTime = 0.0f;
		treeID = 0;
	}
	
	void Setup()
	{
		// Set the camera at a height/position where it can see all of the generated grid
		Camera.main.transform.position = new Vector3(gridSize.x/2, gridSize.x, gridSize.y/2);
		Camera.main.orthographicSize = ((gridSize.x + gridSize.y)/2)/1.8f;
		
		// Iterate through the gridSizes
		for(int i = 0; i < gridSize.x; ++i)
		{
			for(int j = 0; j < gridSize.y; ++j)
			{
				// Create a grid cube/node at each of the points
				Transform node;
				node = Instantiate (gridNode, new Vector3(i, 0, j), Quaternion.identity) as Transform;
				node.GetComponent<Vertices>().treeID = treeID;
				treeID++;
				// Set the cube/nodes parent/name for grouping/easy recognition
				node.parent = this.transform;
				node.name = "(" + i + ", 0, " + j + ")";
				
				grid[i, j] = node;
			}
		}
	}
	
	void Weights()
	{
		Vertices verts;
		
		for(int i = 0; i < gridSize.x; ++i)
		{
			for(int j = 0; j < gridSize.y; ++j)
			{
				Transform node;
				node = grid[i, j];
				
				verts = node.GetComponent<Vertices>();
				
				// Adjacent nodes are stored as shown below
				
				///////////[0]////////////
				////////[3][x][1]/////////
				///////////[2]////////////
				
				////// Up = Position j, Down = Negative j
				////// Right = Positive i, Left = Negative i
				if(i - 1 >= 0 && i + 1 <= gridSize.x)
				{
					verts.adjNode[1] = grid[i, j];
					verts.adjNode[3] = grid[i - 1, j];
					verts.connectedNodes[1] = grid[i, j];
					verts.connectedNodes[3] = grid[i - 1, j];
					
					CreateEdge(node, verts.adjNode[1], false);
					CreateEdge(node, verts.adjNode[3], true);
				}
				else if(i - 1 < 0)
				{
					verts.adjNode[1] = grid[i + 1, j];
					verts.adjNode[3] = null;
					
					verts.connectedNodes[1] = grid[i + 1, j];
					verts.connectedNodes[3] = null;
					
					CreateEdge(node, verts.adjNode[1], true);
				}
				else if(i + 1 > gridSize.x)
				{
					verts.adjNode[1] = null;
					verts.adjNode[3] = grid[i - 1, j];
					verts.connectedNodes[1] = null;
					verts.connectedNodes[3] = grid[i - 1, j];
					
					CreateEdge(node, verts.adjNode[3], false);					
				}
				
				if(j - 1 >= 0 && j + 1 < gridSize.y)
				{
					verts.adjNode[0] = grid[i, j + 1];
					verts.adjNode[2] = grid[i, j - 1];
					verts.connectedNodes[0] = grid[i, j + 1];
					verts.connectedNodes[2] = grid[i, j - 1];
					
					CreateEdge (node, verts.adjNode[0], true);
					CreateEdge(node, verts.adjNode[2], false);
				}
				else if(j - 1 < 0)
				{
					verts.adjNode[0] = grid[i, j + 1];
					verts.adjNode[2] = null;
					verts.connectedNodes[0] = grid[i, j + 1];
					verts.connectedNodes[2] = null;
					
					CreateEdge (node, verts.adjNode[0], true);
						
				}
				else if(j + 1 > gridSize.y)
				{
					verts.adjNode[0] = null;
					verts.adjNode[2] = grid[i, j - 1];
					verts.connectedNodes[0] = null;
					verts.connectedNodes[2] = grid[i, j - 1];
					
					CreateEdge(node, verts.adjNode[2], false);
				}
			}
		}
		
		edges = edges.OrderBy(o=>o.edgeWeight).ToList();

	}
	
	IEnumerator Generate()
	{
		while(edges.Count > 0)
		{
			Algorithm ();
			yield return new WaitForSeconds(0.0005f);
		}
	}
	
	void Algorithm()
	{
		Transform nextWall = null;
		
		Edge edge = edges.First();
		
		if(edge.fromID == edge.toID)
		{
			edges.Remove (edge);
			return;
		}
		else if(edge.fromID != edge.toID)
		{
			int treeID1 = edge.toID;
			
			for(int i = 0; i < gridSize.x; ++i)
			{
				for(int j = 0; j < gridSize.y; ++j)
				{
					Transform node = grid[i, j];
					if(node.GetComponent<Vertices>().treeID == treeID1)
					{
						node.GetComponent<Vertices>().treeID = edge.fromID;
					}
				}
			}
			
			Vector3 wallPos = new Vector3(edge.fromX, 0, edge.fromY);
			// Spawn Walls
			if(edge.fromX - edge.toX == -1)
			{
				nextWall = Instantiate (wall, wallPos, Quaternion.identity) as Transform;
				// Set both tree ID's to the same value
				edge.toID = edge.fromID;
			}
			else if(edge.fromX - edge.toX == 1)
			{
				nextWall = Instantiate (wall, wallPos, Quaternion.Euler(0, 180, 0)) as Transform;
				// Set both tree ID's to the same value
				edge.toID = edge.fromID;
			}
			else if(edge.fromY - edge.toY == -1)
			{
				nextWall = Instantiate (wall, wallPos, Quaternion.Euler(0, -90, 0)) as Transform;
				// Set both tree ID's to the same value
				edge.toID = edge.fromID;
			}
			else if(edge.fromY - edge.toY == 1)
			{
				nextWall = Instantiate (wall, wallPos, Quaternion.Euler(0, 90, 0)) as Transform;
				// Set both tree ID's to the same value
				edge.toID = edge.fromID;
			}
			else
			{
				Destroy (nextWall);
				nextWall = null;
			}
		}
		
		edges.Remove (edge);
		
		if(nextWall != null)
		{
			nextWall.parent = wallGroup;
			wallList.Add(nextWall.gameObject);
		}
		
		Debug.Log (edges.Count);
	}
	
	void ShowStartAndEnd()
	{
//		firstNode.renderer.material.color = Color.yellow;
//		firstNode.transform.position += new Vector3(0, 0.1f, 0);
//		lastNode.renderer.material.color = Color.green;
//		lastNode.transform.position += new Vector3(0, 0.1f, 0);
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(0 + 10, 0 + 10, 100, 50), "Setup"))
		{
			if(generatingMaze == false)
			{
				Setup ();
			}
		}
		
		if(GUI.Button (new Rect(0 + 10, 0 + 60, 100, 50), "Weights"))
		{
			Weights ();
		}
		
		if(GUI.Button (new Rect(0 + 10, 0 + 110, 100, 50), "Generate"))
		{
			StartCoroutine("Generate");
		}
		
		if(GUI.Button (new Rect(Screen.width - 110, 0 + 60, 100, 50), "Show Start/End"))
		{
			ShowStartAndEnd();
		}
	}

	void CreateEdge(Transform fromNode, Transform toNode, bool addEdge)
	{
		//Edge edge = new Edge();
		Edge edge = ScriptableObject.CreateInstance("Edge") as Edge;
		if(edge.SetConnectingNodes(fromNode, toNode) == false)
		{
			edges.Remove (edge);
		}
		else
		{	
			if(addEdge)
			{
				edges.Add (edge);
			}
		}
	}
}
