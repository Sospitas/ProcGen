using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Kruskal2 : MonoBehaviour 
{
	public Vector2 gridSize;
	public Transform gridNode;
	
	public List<Transform> grid = new List<Transform>();
	
	public List<Edge2> edges = new List<Edge2>();
	
	void Start()
	{
		grid.Clear();
		edges.Clear();
	}
	
	void Setup()
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
				grid.Add (node);
			}
		}
	}
	
	void Weights()
	{
		for(int i = 0; i < gridSize.x; ++i)
		{
			for(int j = 0; j < gridSize.y; ++j)
			{
				//GameObject.Find ("(" + i + ", 0, " + j + ")");
				Transform node = GameObject.Find ("(" + i + ", 0, " + j + ")").transform;
				
				// X bounds checks
				if(i - 1 < 0)
				{
					
				}
				else if(i + 1 > gridSize.x)
				{
					
				}
				else if(i - 1 >= 0 && i + 1 <= gridSize.x)
				{
					
				}
				
				// Y bounds checks
				if(j - 1 < 0)
				{
					
				}
				else if(j + 1 > gridSize.y)
				{
					
				}
				else if(j - 1 >= 0 && j + 1 <= gridSize.y)
				{
					
				}
			}
		}
	}
	
	void Generate()
	{
		
	}
	
	void Algorithm()
	{
		
	}
	
	void CreateEdge(Transform fromNode, Transform toNode, bool addEdgeToList = false)
	{
		Edge2 edge = new Edge2();
		
		float fromX = fromNode.position.x;
		float fromY = fromNode.position.y;
		float toX = toNode.position.x;
		float toY = toNode.position.y;
		
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
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(0 + 10, 0 + 10, 100, 50), "Setup"))
		{
			Setup ();
		}
	}
}
