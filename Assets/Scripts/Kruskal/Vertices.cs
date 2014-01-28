using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertices : MonoBehaviour
{
	public int treeRank;
	public Vertices treeRoot;
	
	void Start () 
	{		
		treeRank = 0;
		this.treeRoot = this;
	}
	
	public Vertices GetRoot()
	{
		if(this.treeRoot != this)
		{
			this.treeRoot = this.treeRoot.GetRoot();
		}
		
		return this.treeRoot;
	}
	
	public void JoinRoots(Vertices v1)
	{
		if(v1.treeRank < this.treeRank)
		{
			v1.treeRoot = this;
		}
		else
		{
			this.treeRoot = v1;
			if(this.treeRank == v1.treeRank)
			{
				v1.treeRank++;
			}
		}
	}
}
