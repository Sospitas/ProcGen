using UnityEngine;
using System.Collections;

public class NodeWeights : MonoBehaviour
{
	public int weight;
	public Transform[] adjNode = new Transform[4];
	public int[] adjWeights = new int[4];
	
	public bool hasBeenVisited = false;
	
	// Use this for initialization
	void Start ()
	{
		weight = Random.Range (0, 100);
	}
	
	public void SetAdjWeight(int i)
	{
		if(adjNode[i] != null)
		{
			adjWeights[i] = adjNode[i].GetComponent<NodeWeights>().weight;
		}
		else
		{
			adjWeights[i] = -1;
		}
	}
}
