using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Work_Script : MonoBehaviour
{

	public enum ObjectType
	{
		Rock,
		Rubble,
		Nothing
	}

	public GameObject Worker;
	public bool WorkedOn = false;
	public float Health = 100;
	public ObjectType Type = ObjectType.Nothing;
	private GameObject System_;
	private System_Script System_St;
	public GridPos RockProperties;
	public Building_System Building_System;
	public Game_Script Game_Script;

	public int OreCreated = 0;

	private int SpreadX = 5;
	private int SpreadY = 5;



	//Properties for grid finding(Used for array)
	public int X;
	public int Y;


	// Use this for initialization
	void Start()
	{
		System_ = GameObject.Find("System");
		System_St = System_.GetComponent<System_Script>();
		Game_Script = System_.GetComponent<Game_Script>();
		Building_System = System_.GetComponent<Building_System>();
	}

	// Update is called once per frame
	void Update()
	{
		if (RockProperties != null)
		{
			X = RockProperties.X;
			Y = RockProperties.Y;
		}

		if (Worker != null)
		{
			if (Worker.GetComponent<Lego_Character>().TaskObject != this.gameObject)
			{
				Worker = null;

				if (WorkedOn)
				{
					AddtoList();
				}
			}

			if (System_Script.DrillRocks.Contains(this.gameObject))
			{
				System_Script.DrillRocks.Remove(this.gameObject);
			}
		}

		if (Health <= 0)
		{
			if (Type == ObjectType.Rock)
			{
				Game_Script.EraseRock(X, Y);
			}

			Destroy(this.gameObject);
		}

		if (Type == ObjectType.Rubble)
		{
			if (Health < 75 && Health > 70 && OreCreated == 0)
			{
				CreateOre();
			}

			if (Health < 70 && Health > 35 && OreCreated < 2)
			{
				CreateOre();
			}

			if (Health < 35 && OreCreated < 3)
			{
				CreateOre();
			}
		}
	}

	public void AddtoList()
	{
		System_Script.DrillRocks.Add(this.gameObject);
	}

	private void OnDestroy()
	{
		foreach (var unit in System_Script.AllWorkers)
		{
			if (unit != null)
			{
				var unit_ = unit.GetComponent<Lego_Character>();

				if (unit_.TaskObject == this.gameObject)
				{
					unit_.TaskObject = null;
					unit_.TaskChassis = TaskChassis.JWalking;
					unit_.CurrentTask = CurrentJob.WalkToPoint;
				}

			}
		}

		if (GetComponent<Work_Script>().Worker != null)
		{
			ResetLegoUnit();

			if (Building_System.BuildingGrid[X, Y].Object = this.gameObject)
			{
				Building_System.FullReset(X, Y);
			}
		}

		//Remove from list commands
		if (System_Script.ClearRubble.Contains(this.gameObject))
		{
			System_Script.ClearRubble.Remove(this.gameObject);
		}

		if (System_Script.DrillRocks.Contains(this.gameObject))
		{
			System_Script.DrillRocks.Remove(this.gameObject);
		}

		if (Health <= 0)
		{
			var POS = this.transform.position;

			if (Type == ObjectType.Rock)
			{
				var Rubble = Instantiate(System_.GetComponent<System_Script>().Rubble, new Vector3(POS.x, 0.1f, POS.z), Quaternion.identity);
				Building_System.BuildingGrid[X, Y].B_Types = BuildingTypes.Rubble;
				Building_System.BuildingGrid[X, Y].Object = Rubble;
				Rubble.GetComponent<Work_Script>().X = X;
				Rubble.GetComponent<Work_Script>().Y = Y;


			}
		}

		foreach (var Unit in System_Script.SelectedGameObjects)
		{
			if (Unit.GetComponent<Lego_Character>().TaskObject == this.gameObject)
			{
				var legounit = Unit.GetComponent<Lego_Character>();
				legounit.TaskObject = null;
				legounit.CurrentTask = CurrentJob.Nothing;
				legounit.TaskChassis = TaskChassis.Nothing;
			}
		}
	}

	//clear lego unit commands
	public void ResetLegoUnit()
	{
		var legounit = GetComponent<Work_Script>().Worker.GetComponent<Lego_Character>();
		legounit.TaskObject = null;
		legounit.CurrentTask = CurrentJob.Nothing;
		legounit.TaskChassis = TaskChassis.Nothing;
	}

	public void DestroyRock()
	{
		//System_Script.DrillRocks.Remove(this.gameObject);
		System_.GetComponent<Game_Script>().OnDestroyRock(RockProperties);
	}

	public void CreateOre()
	{
		OreCreated++;

		var SpawnPosX = Random.Range(-SpreadX, SpreadX);
		var SpawnPosY = Random.Range(-SpreadY, SpreadY);
		var rubblePos = this.transform.position;
		var SpawnPos = new Vector3(rubblePos.x + SpawnPosX, 0, rubblePos.z + SpawnPosY);

		var newore = Instantiate(System_St.Ore, SpawnPos, Quaternion.identity);
	}

}
