using UnityEngine;
using System.Collections;

public class Vertices2 : MonoBehaviour
{
	private static int staticTreeID = 0;
	
	public Edge2[] paths;
	public int vertexTreeID;
	
	void Start () 
	{
		vertexTreeID = ++staticTreeID;
		paths = new Edge2[4];
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
	}
	
	public void SetTreeID()
	{
		for(int i = 0; i < paths.Length; ++i)
		{
			if(paths[i] != null)
			{
				paths[i].treeID = vertexTreeID;
			}
		}
	}
}
