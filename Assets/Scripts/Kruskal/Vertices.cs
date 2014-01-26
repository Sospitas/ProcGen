using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertices : MonoBehaviour 
{
	public int treeID;
	public Transform[] adjNode = new Transform[4];
	public Transform[] connectedNodes = new Transform[4];
	public int[] edgeWeights = new int[4];
	
	public void SetAdjWeight(int i)
	{
		if(adjNode[i] != null)
		{
			//edgeWeights[i] = adjNode[i].GetComponent<Vertices>().weight;
		}
		else
		{
			edgeWeights[i] = -1;
		}
	}
}
