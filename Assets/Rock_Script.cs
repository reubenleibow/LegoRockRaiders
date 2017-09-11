using UnityEngine;
using System.Collections;

public class Rock_Script : MonoBehaviour
{
	public GridPos RockProperties;
	public int X;
	public int Y;
	public float Health = 100;
	private GameObject System_;

	// Use this for initialization
	void Start()
	{
		System_ = GameObject.Find("System");
	}

	// Update is called once per frame
	void Update()
	{
		if (RockProperties != null)
		{
			X = RockProperties.X;
			Y = RockProperties.Y;
		}

		if(Health <= 0)
		{
			DestroyRock();
		}
	}

	public void DestroyRock()
	{
		System_.GetComponent<Game_Script>().OnDestroyRock(RockProperties);
	}
}
