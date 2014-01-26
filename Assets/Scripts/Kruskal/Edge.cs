using UnityEngine;
using System.Collections;

public class Edge : ScriptableObject
{
	// Position of the node this edge is coming from
	public float fromX = -1, fromY = -1;
	// Position of the node this edge is going to
	public float toX = -1, toY = -1;
	
	// ID of the node (tree) this edge is coming from
	public int fromID;
	// ID of the node (tree) this edge is going to
	public int toID;
	
	// Edge weight, might be removed later on
	public int edgeWeight;
	
	public Edge()
	{
	}
	
	public bool SetConnectingNodes(Transform node1, Transform node2)
	{
		edgeWeight = -1;
		edgeWeight = Random.Range(0, 1000);
		
		Vertices verts = node1.GetComponent<Vertices>();
		
		for(int i = 0; i < 4; i++)
		{
			if(verts.connectedNodes[i] == node2)
			{
				return false;
			}
			else
			{
				fromX = node1.position.x;
				fromY = node1.position.y;
				toX = node2.position.x;
				toY = node2.position.y;
				fromID = node1.GetComponent<Vertices>().treeID;
				toID = node2.GetComponent<Vertices>().treeID;
				return true;
			}
		}
		return true;
	}
}