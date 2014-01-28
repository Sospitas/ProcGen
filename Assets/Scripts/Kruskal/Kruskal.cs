using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Kruskal : MonoBehaviour 
{	
	public Vector2 gridSize;
	public Transform gridNode;
	public Transform pathPrefab, pathGroup;
	
	//public List<Transform> grid = new List<Transform>();
	public Transform[,] grid;
	public List<Transform> pathList = new List<Transform>();
	
	public List<Edge> edges = new List<Edge>();
	
	void Start()
	{
		//grid.Clear();
		grid = new Transform[(int)gridSize.x, (int)gridSize.y];
		edges.Clear();
		pathList.Clear();
	}
	
	void SetupGrid()
	{
		// Set the camera at a height/position where it can see all of the generated grid
		Camera.main.transform.position = new Vector3(gridSize.x/2, gridSize.x, gridSize.y/2);
		Camera.main.orthographicSize = ((gridSize.x + gridSize.y)/2)/1.8f;
		
		for(int i = 0; i < gridSize.x; ++i)
		{
			for(int j = 0; j < gridSize.y; ++j)
			{
				Transform node;
				node = Instantiate(gridNode, new Vector3(i, 0, j), Quaternion.identity) as Transform;
				node.parent = this.transform;
				node.name = node.name = "(" + i + ", 0, " + j + ")";
				grid[i, j] = node;
			}
		}
	}
	
	void CreateEdges()
	{
		for(int i = 0; i < gridSize.x; ++i)
		{
			for(int j = 0; j < gridSize.y; ++j)
			{
				//GameObject.Find ("(" + i + ", 0, " + j + ")");
				//Transform node = GameObject.Find ("(" + i + ", 0, " + j + ")").transform;
				
				// X bounds checks
				if(i == 0)
				{
					CreateEdge (new Vector2(i, j), new Vector2(i + 1, j), true);
				}
				else if(i == gridSize.x - 1)
				{
					CreateEdge (new Vector2(i, j), new Vector2(i - 1, j), true);
				}
				else if(i > 0 && i < gridSize.x - 1)
				{
					CreateEdge (new Vector2(i, j), new Vector2(i + 1, j), true);
					CreateEdge (new Vector2(i, j), new Vector2(i - 1, j), true);
				}
				
				// Y bounds checks
				if(j == 0)
				{
					CreateEdge (new Vector2(i, j), new Vector2(i, j + 1), true);
				}
				else if(j == gridSize.y - 1)
				{
					CreateEdge (new Vector2(i, j), new Vector2(i, j - 1), true);
				}
				else if(j > 0 && j < gridSize.y - 1)
				{
					CreateEdge (new Vector2(i, j), new Vector2(i, j + 1), true);
					CreateEdge (new Vector2(i, j), new Vector2(i, j - 1), true);
				}
			}
		}
		
		ShuffleList();
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
		Edge edge = edges[0];
		PlacePath (edge);
	}
	
	void CreateEdge(Vector2 fromPos, Vector2 toPos, bool addEdgeToList = false)
	{
		Edge edge = new Edge();
		edge.originX = fromPos.x;
		edge.originY = fromPos.y;
		
		float fromX = fromPos.x;
		float fromY = fromPos.y;
		float toX = toPos.x;
		float toY = toPos.y;
		
		if(toX - fromX == 1)
		{
			edge.edgeDir = Direction.EAST;
		}
		else if(toX - fromX == -1)
		{
			edge.edgeDir = Direction.WEST;
		}
		if(toY - fromY == 1)
		{
			edge.edgeDir = Direction.NORTH;
		}
		else if(toY - fromY == -1)
		{
			edge.edgeDir = Direction.SOUTH;
		}
		
		if(addEdgeToList == true)
		{
			edges.Add (edge);
		}
		
		edge.connectedVerts[0] = grid[(int)fromX, (int)fromY].GetComponent<Vertices>();
		edge.connectedVerts[1] = grid[(int)toX, (int)toY].GetComponent<Vertices>();
	}
	
	void ShuffleList()
	{		
		for(int i = edges.Count; i > 1; --i)
		{
			int rnd = Random.Range (0, i);
			
			Edge tmp = edges[rnd];
			edges[rnd] = edges[i - 1];
			edges[i - 1] = tmp;
		}
	}
	
	void PlacePath(Edge edge)
	{
		Transform path;
		path = null;
		
		Vertices originVert = edge.connectedVerts[0];
		Vertices targetVert = edge.connectedVerts[1];
		
		if(targetVert.GetRoot () != originVert.GetRoot())
		{
			if(edge.edgeDir == Direction.NORTH)
			{
				path = Instantiate(pathPrefab, new Vector3(edge.originX, 0, edge.originY), Quaternion.Euler(0, -90, 0)) as Transform;
			}
			if(edge.edgeDir == Direction.EAST)
			{
				path = Instantiate(pathPrefab, new Vector3(edge.originX, 0, edge.originY), Quaternion.identity) as Transform;
			}
			if(edge.edgeDir == Direction.SOUTH)
			{
				path = Instantiate(pathPrefab, new Vector3(edge.originX, 0, edge.originY), Quaternion.Euler(0, 90, 0)) as Transform;
			}
			if(edge.edgeDir == Direction.WEST)
			{
				path = Instantiate(pathPrefab, new Vector3(edge.originX, 0, edge.originY), Quaternion.Euler(0, 180, 0)) as Transform;
			}
			
			path.parent = pathGroup;
			
			pathList.Add(path);
			
			originVert.GetRoot ().JoinRoots(targetVert.GetRoot ());
		}
		
		edges.Remove (edge);
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(0 + 10, 0 + 10, 100, 50), "Setup"))
		{
			SetupGrid ();
		}
		
		if(GUI.Button (new Rect(0 + 10, 0 + 60, 100, 50), "Weights"))
		{
			CreateEdges ();
		}
		
		if(GUI.Button (new Rect(0 + 10, 0 + 110, 100, 50), "Generate"))
		{
			StartCoroutine("Generate");
		}
		
		GUI.Box (new Rect(0 + 1, Screen.height - 100, 100, 50), edges.Count.ToString());
	}
}
