using UnityEngine;
using System.Collections;

public enum Direction
{
	NORTH = 0,
	EAST = 1,
	SOUTH = 2,
	WEST = 3,
	NONE = 4,
}

[System.Serializable]
// Edge class for use within Kruskals Maze Generation
public class Edge2
{
	//public static int staticTreeID = 0;
	
	public float originX, originY;
	public Direction edgeDir;
	public int treeID;
	public Vertices2[] connectedVerts;
	
	public Edge2()
	{
		originX = -1;
		originY = -1;
		edgeDir = Direction.NONE;
		treeID = -1;
		connectedVerts = new Vertices2[2];
	}
}
