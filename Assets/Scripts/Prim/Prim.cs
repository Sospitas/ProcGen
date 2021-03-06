﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Prim : MonoBehaviour 
{	
	public Transform pathGroup;
	
	public Transform gridNode, path;
	public Vector2 gridSize;
	public Transform [,] grid;
	
	public Transform lastNode, currentNode, treeNode;
	
	public List<GameObject> mazeList = new List<GameObject>();
	public List<GameObject> pathList = new List<GameObject>();
	
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
	
	// Generates the grid by instantiating a load of cubes
	// in a 2 dimensional grid
	void SetupGrid()
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
	
	IEnumerator Generate()
	{
		float startTime = Time.time;
		while(mazeList.Count < maximumNodes)
		{
			Algorithm();
			yield return new WaitForSeconds(0.0005f);
			
			float endTime = Time.time;
			generationTime = endTime - startTime;
		}
	}
	
	// Setting currentNode doesn't update it properly
	void Algorithm()
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
			AddPaths(treeNode, currentNode);
		}
		//Debug.Log (currentNode.name);
	}
	
	// Delete the grid if something goes wrong. Iterates through children
	// of this transform and deletes them all
	void DeleteGrid()
	{		
		foreach(Transform t in grid)
		{
			Destroy(t.gameObject);
		}
		
		foreach(Transform t in pathGroup)
		{
			Destroy(t.gameObject);
		}
		
		for(int i = 0; i < pathList.Count; i++)
		{
			Destroy(pathList[i].gameObject);
		}
		
		mazeList.Clear ();
		
		hasStartPoint = false;
	}
	
	// Selects a random node in the grid to start the maze from
	void SelectStartingPoint()
	{
		int x = Random.Range (0, (int)gridSize.x - 1);
		int z = Random.Range (0, (int)gridSize.y - 1);
		
		//Transform startNode = GameObject.Find ("(" + x + ", 0, " + z + ")").transform;
		Transform startNode = grid[x, z];
		
		startNode.renderer.material.color = Color.red;
		
		currentNode = startNode;
		
		currentNode.GetComponent<NodeWeights>().hasBeenVisited = true;
		
		totalWeight += startNode.GetComponent<NodeWeights>().weight;
			
		hasStartPoint = true;
		
		mazeList.Add(currentNode.gameObject);
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
	void AddPaths(Transform last, Transform current)
	{
		Transform nextPath = null;
		
		if(last.position.z < current.position.z)
		{
			nextPath = Instantiate(path, last.position, Quaternion.Euler(0, -90, 0)) as Transform;
		}
		if(last.position.x < current.position.x)
		{
			nextPath = Instantiate (path, last.position, Quaternion.identity) as Transform;
		}
		if(last.position.z > current.position.z)
		{
			nextPath = Instantiate(path, last.position, Quaternion.Euler(0, 90, 0)) as Transform;
		}
		if(last.position.x > current.position.x)
		{
			nextPath = Instantiate (path, last.position, Quaternion.Euler(0, 180, 0)) as Transform;
		}
		
		if(nextPath != null)
		{
			nextPath.parent = pathGroup;
			pathList.Add(nextPath.gameObject);
		}
	}
	
	void ShowStartAndEnd()
	{
		Transform start = pathList[0].transform;
		start.gameObject.renderer.enabled = true;
		start.gameObject.transform.position += new Vector3(0.0f, 0.2f, 0.0f);
		start.gameObject.renderer.material.color = Color.yellow;
		
		Transform end = pathList[pathList.Count - 1].transform;
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
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width - 110, 0 + 10, 100, 50), "Generate"))
		{
			SetupGrid ();
		}
		
		if(GUI.Button (new Rect(Screen.width - 110, 0 + 60, 100, 50), "AdjWeights"))
		{
			GetAdjacentWeights();
		}
		
		if(GUI.Button(new Rect(Screen.width - 110, 0 + 110, 100, 50), "StartPoint"))
		{
			if(hasStartPoint == false)
			{
				SelectStartingPoint();	
			}
		}
		
		if(GUI.Button(new Rect(Screen.width - 110, 0 + 160, 100, 50), "GenerateMaze"))
		{
			StartCoroutine("Generate");
		}
		
		if(GUI.Button(new Rect(Screen.width - 110, 0 + 210, 100, 50), "Delete"))
		{
			DeleteGrid();
			generationTime = 0;
		}
		
//		if(GUI.Button (new Rect(Screen.width - 110, 0 + 260, 100, 50), "Show Start/End"))
//		{
//			ShowStartAndEnd();
//		}
		
		GUI.Box(new Rect(Screen.width/2 + 50, 0 + 10, 200, 50), "Generation Time: \n" + generationTime.ToString("f2"));
	}
}
