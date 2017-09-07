using UnityEngine;
using System.Collections;

public class Rock_Script : MonoBehaviour
{
	public GridPos RockProperties;
	public int X;
	public int Y;
	public bool Testable = true;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Testable && RockProperties != null)
		{
			X = RockProperties.X;
			Y = RockProperties.Y;
		}
	}
}
