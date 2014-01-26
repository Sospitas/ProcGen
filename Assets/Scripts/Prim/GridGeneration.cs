using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridGeneration : MonoBehaviour 
{
	public bool generatingMaze = false;
	public bool mazeActive = false;
	
	public Transform wallGroup;
	
	public Transform gridNode, wall, otherWalls;
	public Vector2 gridSize;
	public Transform [,] grid;
	
	public Transform lastNode, currentNode, treeNode;
	
	// Queue used to store all nodes that have been added to the tree
	// When all nodes are added, this then iterates through and deletes
	// all objects as it traces through the maze
	public Queue<GameObject> mazeQueue = new Queue<GameObject>();
	
	public List<GameObject> mazeList = new List<GameObject>();
	public List<GameObject> wallList = new List<GameObject>();
	
	private bool hasStartPoint = false;
	private int maximumNodes;
	
	private int totalWeight;
	
	private Transform nextTrans;
	
	private float generationTime;
	
	// Initialise all variables
	void Start()
	{
		grid = new Transform[(int)gridSize.x, (int)gridSize.y];
		lastNode = null;
		currentNode = null;
		maximumNodes = (int)gridSize.x * (int)gridSize.y;
		totalWeight = 0;
		generationTime = 0;
	}
	
	// Delete the grid if something goes wrong. Iterates through children
	// of this transform and deletes them all
	void DeleteGrid()
	{		
		if(this.transform.childCount != 0)
		{			
			List<GameObject> childrenToDelete = new List<GameObject>();
			foreach(Transform child in transform)
			{
				childrenToDelete.Add(child.gameObject);
			}
			
			childrenToDelete.ForEach(child => Destroy (child));
		}
		
		List<GameObject> wallToDelete = new List<GameObject>();
		foreach(Transform childWall in wallGroup)
		{
			wallToDelete.Add(childWall.gameObject);
		}
		
		wallToDelete.ForEach(childWall => Destroy(childWall));
		
		mazeQueue.Clear();
		mazeList.Clear ();
		
		hasStartPoint = false;
	}
	
	// Generates the grid by instantiating a load of cubes
	// in a 2 dimensional grid
	void GenerateGrid()
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
				// Set the cube/nodes parent/name for grouping/easy recognition
				node.parent = this.transform;
				node.name = "(" + i + ", 0, " + j + ")";
				
				grid[i, j] = node;
			}
		}
	}
	
	// Selects a random node in the grid to start the maze from
	void SelectStartingPoint()
	{
		int x = Random.Range (0, (int)gridSize.x);
		int z = Random.Range (0, (int)gridSize.y);
		
		Transform startNode = GameObject.Find ("(" + x + ", 0, " + z + ")").transform;
		
		mazeQueue.Enqueue(GameObject.Find ("(" + x + ", 0, " + z + ")"));
		
		startNode.renderer.material.color = Color.red;
		
		currentNode = startNode;
		
		currentNode.GetComponent<NodeWeights>().hasBeenVisited = true;
		
		totalWeight += startNode.GetComponent<NodeWeights>().weight;
			
		hasStartPoint = true;
		
		mazeList.Add(currentNode.gameObject);
	}
	
	IEnumerator GenerateAlgorithm()
	{
		float startTime = Time.time;
		generatingMaze = true;
		while(mazeList.Count < maximumNodes)
		{
			PrimsAlgorithm();
			yield return new WaitForSeconds(0.0005f);
			//Debug.Log (totalWeight);
			
			float endTime = Time.time;
		
			generationTime = endTime - startTime;
		}
		
//		if(mazeList.Count == maximumNodes)
//		{
//			float endTime = Time.time;
//		
//			generationTime = endTime - startTime;
//		}
		
		generatingMaze = false;
		
		GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
		
		for(int i = 0; i < walls.Length; ++i)
		{
			walls[i].transform.parent = wallGroup;
		}
		
		Debug.Log (generationTime);
		yield return new WaitForSeconds(0.0f);
	}
	
	// Setting currentNode doesn't update it properly
	void PrimsAlgorithm()
	{
		nextTrans = null;
		
		// Set up lowest weight to a high value so that nothing in the list can be higher
		int lowestWeight = 1000000;
		
		// Iterate over all of the objects in the maze list
		for(int j = 0; j < mazeList.Count; ++j)
		{
			// Set current node to current run of loop
			currentNode = mazeList[j].transform;
			
			// Get current node weight script component
			NodeWeights weights = currentNode.GetComponent<NodeWeights>();
			
			// Check all adjacent nodes weights
			for(int i = 0; i < 4; ++i)
			{
				// If the node we are checking has a lower weight than the current lowest and it is more than -1
				if(weights.adjWeights[i] < lowestWeight && weights.adjWeights[i] > -1)
				{
					// Get adjacent node and check if it is part of the spanning tree
					NodeWeights temp = weights.adjNode[i].GetComponent<NodeWeights>();
					if(temp.hasBeenVisited == true)
					{
						// If it is part of the tree, continue the loop
						continue;
					}
					else
					{
						// If it is not part of the tree set the lowest weight to its weight
						// and set the next transform to the object with said weight
						treeNode = currentNode;
						lowestWeight = temp.weight;
						nextTrans = weights.adjNode[i];
					}
				}
			}
		}
		if(nextTrans != null)
		{
			// Highlight the node that has been selected
			nextTrans.renderer.material.color = Color.red;
			// Set the selected node to 'visited'
			nextTrans.GetComponent<NodeWeights>().hasBeenVisited = true;
			// Add the selected nodes weight to the total weight
			totalWeight += nextTrans.GetComponent<NodeWeights>().weight;
			
			lastNode = currentNode;
			currentNode = nextTrans;
			
			// Add the node to the list of objects in the maze for checking
			// in the next run through the loop
			mazeList.Add(currentNode.gameObject);
			AddWalls(treeNode, currentNode);
		}
		//Debug.Log (currentNode.name);
	}
	
	// Gets all adjacent nodes and adjacent weights
	void GetAdjacentWeights()
	{
		NodeWeights weightScript;
		
		for(int i = 0; i < gridSize.x; ++i)
		{
			for(int j = 0; j < gridSize.y; ++j)
			{
				Transform node;
				node = grid[i, j];
				
				weightScript = node.GetComponent<NodeWeights>();
				
				// Adjacent blocks are stored as shown below
				
				///////////[0]////////////
				////////[3][x][1]/////////
				///////////[2]////////////
				
				////// Up = Position j, Down = Negative j
				////// Right = Positive i, Left = Negative i				
				if(i - 1 >= 0 && i + 1 < gridSize.x)
				{
					weightScript.adjNode[3] = grid[i - 1, j];
					weightScript.SetAdjWeight(3);
					
					weightScript.adjNode[1] = grid[i + 1, j];
					weightScript.SetAdjWeight(1);
				}
				else if(i - 1 < 0)
				{
					weightScript.adjNode[3] = null;
					weightScript.SetAdjWeight(3);
					
					weightScript.adjNode[1] = grid[i + 1, j];
					weightScript.SetAdjWeight(1);
				}
				else if(i + 1 >= gridSize.x)
				{
					weightScript.adjNode[3] = grid[i - 1, j];
					weightScript.SetAdjWeight(3);
					
					weightScript.adjNode[1] = null;
					weightScript.SetAdjWeight(1);
				}
				
				if(j - 1 >= 0 && j + 1 < gridSize.y)
				{
					weightScript.adjNode[2] = grid[i, j - 1];
					weightScript.SetAdjWeight(2);
					
					weightScript.adjNode[0] = grid[i, j + 1];
					weightScript.SetAdjWeight(0);
				}
				else if(j - 1 < 0)
				{
					weightScript.adjNode[2] = null;
					weightScript.SetAdjWeight(2);
					
					weightScript.adjNode[0] = grid[i, j + 1];
					weightScript.SetAdjWeight(0);
				}
				else if(j + 1 >= gridSize.y)
				{
					weightScript.adjNode[2] = grid[i, j - 1];
					weightScript.SetAdjWeight(2);
					
					weightScript.adjNode[0] = null;
					weightScript.SetAdjWeight(0);
				}
			}
		}
	}
	
	
	// Adjacent blocks are stored as shown below
				
				///////////[0]////////////
				////////[3][x][1]/////////
				///////////[2]////////////
	
	// Checks against adjacent blocks to get rotation for wall
	// Wall spawns at position x, with pivot at edge of wall
	void AddWalls(Transform last, Transform current)
	{
		Transform nextWall = null;
		
		if(last.position.z < current.position.z)
		{
			nextWall = Instantiate(wall, last.position, Quaternion.Euler(0, -90, 0)) as Transform;
		}
		if(last.position.x < current.position.x)
		{
			nextWall = Instantiate (wall, last.position, Quaternion.identity) as Transform;
		}
		if(last.position.z > current.position.z)
		{
			nextWall = Instantiate(wall, last.position, Quaternion.Euler(0, 90, 0)) as Transform;
		}
		if(last.position.x > current.position.x)
		{
			nextWall = Instantiate (wall, last.position, Quaternion.Euler(0, 180, 0)) as Transform;
		}
		
		if(nextWall != null)
		{
			wallList.Add(nextWall.gameObject);
		}
	}
	
	void ShowStartAndEnd()
	{
		Transform start = wallList[0].transform;
		start.gameObject.renderer.enabled = true;
		start.gameObject.transform.position += new Vector3(0.0f, 0.2f, 0.0f);
		start.gameObject.renderer.material.color = Color.yellow;
		
		Transform end = wallList[wallList.Count - 1].transform;
		end.gameObject.renderer.enabled = true;
		end.gameObject.transform.position += new Vector3(0.0f, 0.2f, 0.0f);
		end.gameObject.renderer.material.color = Color.green;
		
		//List<GameObject> nodes = new List<GameObject>();
		foreach(Transform child in transform)
		{
			child.renderer.enabled = false;
			//nodes.Add(child.gameObject);
		}
	}
	
	void PrintQueue()
	{
		for(int i = 0; i < mazeQueue.Count; ++i)
		{
			Debug.Log("Item: " + i.ToString() + mazeQueue.Dequeue().name);
		}
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(0 + 10, 0 + 10, 100, 50), "Generate"))
		{
			if(mazeActive == false)
			{
				GenerateGrid ();
			}
		}
		
		if(GUI.Button (new Rect(0 + 10, 0 + 60, 100, 50), "AdjWeights"))
		{
			GetAdjacentWeights();
		}
		
		if(GUI.Button(new Rect(0 + 10, 0 + 110, 100, 50), "StartPoint"))
		{
			if(hasStartPoint == false)
			{
				SelectStartingPoint();	
			}
		}
		
		if(GUI.Button(new Rect(0 + 10, 0 + 160, 100, 50), "GenerateMaze"))
		{
			if(generatingMaze == false)
			{
				StartCoroutine(GenerateAlgorithm());
			}
		}
		
		if(GUI.Button(new Rect(0 + 10, 0 + 210, 100, 50), "Delete"))
		{
			if(mazeActive == true)
			{
				mazeActive = false;
				DeleteGrid();
			}
		}
		
		if(GUI.Button (new Rect(Screen.width - 110, 0 + 10, 100, 50), "Print Queue"))
		{
			PrintQueue();
		}
		
		if(GUI.Button (new Rect(Screen.width - 110, 0 + 60, 100, 50), "Show Start/End"))
		{
			ShowStartAndEnd();
		}
		
		GUI.Box(new Rect(Screen.width/2 - 100, 0 + 10, 200, 50), "Generation Time: \n" + generationTime.ToString("f2"));
	}
}
