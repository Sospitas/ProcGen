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

// Edge class for use within Kruskals Maze Generation
public class Edge2
{
	public float originX, originY;
	public Direction edgeDir;
	
	public Edge2()
	{
		originX = -1;
		originY = -1;
		edgeDir = Direction.NONE;
	}
}
