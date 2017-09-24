using UnityEngine;
using System.Collections;

public class Rock_Script : MonoBehaviour
{
	public GridPos RockProperties;
	public int X;
	public int Y;
	public float Health = 100;
	private GameObject System_;
	public bool WorkedOn = false;

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
		ResetLegoUnit();
		System_Script.DrillRocks.Remove(this.gameObject);
		System_.GetComponent<Game_Script>().OnDestroyRock(RockProperties);
	}


	public void OnDestroy()
	{
		if(GetComponent<Work_Script>().Worker != null)
		{
			ResetLegoUnit();
		}
	}

	public void ResetLegoUnit()
	{
		var legounit = GetComponent<Work_Script>().Worker.GetComponent<Lego_Character>();
		legounit.TaskObject = null;
		legounit.CurrentTask = CurrentJob.Nothing;
		legounit.TaskChassis = TaskChassis.Nothing;
	}
}
