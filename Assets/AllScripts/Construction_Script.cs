using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction_Script : MonoBehaviour {

	public List<GameObject> Workerlist_Ore = new List<GameObject>();
	public List<GameObject> Workerlist_Crystal = new List<GameObject>();
	public List<GameObject> Workerlist_Stops = new List<GameObject>();


	public int Required_Ore = 2;
	public int Required_Crystal = 0;
	public int Required_Stops = 0;


	public bool Ore_Worker = false;
	public bool Crystal_Worker = false;
	public bool Stops_Worker = false;
	public bool CompletedConstruction = false;


	public int Contained_Ore = 0;
	public int Contained_Crystal = 0;
	public int Contained_Stops = 0;

	public AllBuildings AllBuildings;
	public Building_System Building_System;


	// Use this for initialization
	void Start ()
	{
		AllBuildings = GameObject.Find("System").GetComponent<AllBuildings>();
		Building_System = GameObject.Find("System").GetComponent<Building_System>();

	}

	// Update is called once per frame
	void Update()
	{
		if(Workerlist_Ore.Count > 0)
		{
			foreach (var units in Workerlist_Ore.ToArray())
			{
				var unitsTask = units.GetComponent<Lego_Character>().TaskObject;

				if(unitsTask != this.gameObject)
				{
					Workerlist_Ore.Remove(units);
				}
			}
		}
		if (Workerlist_Crystal.Count > 0)
		{
			foreach (var units in Workerlist_Crystal.ToArray())
			{
				var unitsTask = units.GetComponent<Lego_Character>().TaskObject;

				if (unitsTask != this.gameObject)
				{
					Workerlist_Crystal.Remove(units);
				}
			}
		}

		if (Workerlist_Stops.Count > 0)
		{
			foreach (var units in Workerlist_Stops.ToArray())
			{
				var unitsTask = units.GetComponent<Lego_Character>().TaskObject;

				if (unitsTask != this.gameObject)
				{
					Workerlist_Stops.Remove(units);
				}
			}
		}

		if (Required_Ore > Workerlist_Ore.Count + Contained_Ore)
		{
			Ore_Worker = true;
		}
		else
		{
			Ore_Worker = false;
		}

		if (Required_Crystal > Workerlist_Crystal.Count + Contained_Crystal)
		{
			Crystal_Worker = true;
		}
		else
		{
			Crystal_Worker = false;
		}

		if (Required_Stops > Workerlist_Stops.Count + Contained_Stops)
		{
			Stops_Worker = true;
		}
		else
		{
			Stops_Worker = false;
		}

		//if the building is not complete
		if(Crystal_Worker || Ore_Worker || Stops_Worker)
		{
			if(!System_Script.ConstructionSites.Contains(this.gameObject))
			{
				System_Script.ConstructionSites.Add(this.gameObject);
			}
		}

		//if the building is complete
		if (Required_Ore <= Contained_Ore  && Required_Crystal <= Contained_Crystal && Required_Stops <= Contained_Stops)
		{
			CompletedConstruction = true;
			Completed();
		}

	}

	public void PlaceOre()
	{

	}

	public void Completed()
	{
		if (System_Script.ConstructionSites.Contains(this.gameObject))
		{
			System_Script.ConstructionSites.Remove(this.gameObject);
		}

		Destroy(this.gameObject);
	}

	public void OnDestroy()
	{
		if(CompletedConstruction)
		{
			var newPowerpathcomplete = Instantiate(AllBuildings.PowerPathComplete, this.transform.position, Quaternion.identity);
			var X = Mathf.Round(this.transform.position.x / 12);
			var Z = Mathf.Round(this.transform.position.z / 12);

			Building_System.BuildingGrid[(int)X, (int)Z].Object = newPowerpathcomplete;
			Building_System.BuildingGrid[(int)X, (int)Z].B_Types = BuildingTypes.PowerPathComplete;

		}
	}
}
