using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertices2 : MonoBehaviour
{
	private static int staticTreeID = 0;
	
	public Edge2[] paths;
	public List<Vertices2> vertexTree = new List<Vertices2>();
	
	public int treeRank;
	public Vertices2 treeRoot;
	
	void Start () 
	{
		vertexTree.Clear();
		paths = new Edge2[4];
		
		treeRank = 0;
		this.treeRoot = this;
	}
	
	public void AddEdge(Edge2 edge, Direction dir)
	{
		// paths[0] == north
		if(dir == Direction.NORTH)
		{
			paths[0] = edge;
		}
		// paths[1] == east
		else if(dir == Direction.EAST)
		{
			paths[1] = edge;
		}
		// paths[2] == south
		else if(dir == Direction.SOUTH)
		{
			paths[2] = edge;
		}
		// paths[3] == west
		else if(dir == Direction.WEST)
		{
			paths[3] = edge;
		}
		
		vertexTree.Add (edge.connectedVerts[1].GetComponent<Vertices2>());
	}
	
	public Vertices2 GetRoot()
	{
		if(this.treeRoot != this)
		{
			this.treeRoot = this.treeRoot.GetRoot();
		}
		
		return this.treeRoot;
	}
	
	public void JoinRoots(Vertices2 v1)
	{
		if(v1.treeRank < this.treeRank)
		{
			v1.treeRoot = this;
			this.JoinLists(v1);
		}
		else
		{
			this.treeRoot = v1;
			v1.JoinLists(this);
			if(this.treeRank == v1.treeRank)
			{
				v1.treeRank++;
			}
		}
	}
	
	private void JoinLists(Vertices2 v1)
	{
		for(int i = 0; i < v1.vertexTree.Count; i++)
		{
			v1.vertexTree[i].treeRoot = this;
			this.vertexTree.Add (v1.vertexTree[i]);
			v1.vertexTree.Remove(v1.vertexTree[i]);
		}
	}
}
