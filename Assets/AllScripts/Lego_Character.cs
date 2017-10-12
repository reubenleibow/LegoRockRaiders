using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public enum CurrentJob
{
	Nothing,
	Drilling,
	WalkingToDrill,
	WalkingToRubble,
	WalkToCollectable,
	CarringCollectable,
	DropOffCollectable,
	WanderAroungWithItem,
	ClearingRubble,
	ConstructionWorker,

}

public enum TaskChassis
{
	GatherOre,
	GatherStops,
	GatherCrystals,
	Nothing,
	JWalking,
	Drilling,
}

public enum ExtraCommands
{
	//rock,rubble
	FindUnTargeted,
	Nothing,
	ReturnCrystal,
	ReturnOre,
	//collectable
	FindUnTargetedObjects,
	//used for construction
	R_Ore_Crystal,
	//used for construction
	PlaceStops,
}


public static class Constants
{
	public static int MinDrillDistance = 2;
	public static int MinDropOffDistance = 2;

}

/// <summary>
/// This used for any lego object(men,cars)
/// </summary>
public class Lego_Character : MonoBehaviour
{
	//private SelectCode SelectCode;
	private GameObject Parent;
	public GameObject CollectionSpace;

	public List<GameObject> Items = new List<GameObject>();

	//Propeties
	public bool UnSelectable = false;
	public bool CanDrill = true;
	private int DrillStrength = 10;
	public bool Arrived = false;


	//Current Tasks(No changes doen here)
	public float DistFromJob;
	public GameObject TaskObject;
	public CurrentJob CurrentTask;
	public RaycastHit TaskPoint;
	public TaskChassis TaskChassis = TaskChassis.Nothing;
	public ExtraCommands ExtraCommands = ExtraCommands.Nothing;

	public CollectableType ItemType;


	public System_Script SystemSrpt;

	void Start()
	{
		Parent = gameObject;
		//SelectCode = GetComponent<SelectCode>();
		//System_Script.RaidersList.Add(this.gameObject);
		SystemSrpt = GameObject.Find("System").GetComponent<System_Script>();
	}

	void Update()
	{
		//if the user says that it has an item in hand but it does not the set defalut
		if(Items.Count == 0 && ItemType != CollectableType.Nothing)
		{
			ItemType = CollectableType.Nothing;
		}

		//doing nothing than find an obnject
		if(CurrentTask == CurrentJob.Nothing && TaskChassis != TaskChassis.JWalking && !UnSelectable)
		{
			SetNextJob();
		}

		//arrived at a target
		if(GetComponent<NavMeshAgent>().remainingDistance > 2)
		{
			Arrived = false;
		}

		//detecting if the rock that the raider must mine is in range using ray caster method
		if (TaskObject != null)
		{
			var Ray = Physics.Linecast(new Vector3(transform.position.x, 1, transform.position.z),
										new Vector3(TaskObject.transform.position.x, 1, TaskObject.transform.position.z),
										out TaskPoint);


			var DirectDistance = Vector3.Distance(transform.position, TaskObject.transform.position);

			if (Ray)
			{
				var TagName = TaskPoint.collider.gameObject.transform.tag;
				var HasParent = TagName == "Rock";

				//only tag neededs to be edited
				if (HasParent)
				{
					if (TaskPoint.collider.gameObject.transform.parent.gameObject == TaskObject)
					{
						DistFromJob = Vector3.Distance(transform.position, TaskPoint.point);
					}
					else
					{
						DistFromJob = float.MaxValue;
					}
				}
			}
			else
			{
				DistFromJob = DirectDistance;
			}

			if (DistFromJob <= Constants.MinDrillDistance)
			{
				if (CurrentTask == CurrentJob.WalkingToDrill)
				{
					StartDrilling();
				}

				if (CurrentTask == CurrentJob.WalkingToRubble)
				{
					StartClearingRubble();
				}

				//arrived at crystal and pick it up.
				if (CurrentTask == CurrentJob.WalkToCollectable && TaskObject != null)
				{
					if (TaskObject.GetComponent<Collectable>().Collector == gameObject)
						PickUpCollectable();
				}
			}

			if (CurrentTask == CurrentJob.Drilling)
			{
				Drilling();
			}

			if (CurrentTask == CurrentJob.ClearingRubble)
			{
				ClearingRubble();
			}

			//place collectable at base
			if (CurrentTask == CurrentJob.DropOffCollectable || CurrentTask == CurrentJob.ConstructionWorker)
			{
				var distance = Vector3.Distance(transform.position, TaskObject.transform.position);


				//ore and crystal drop off
				if (distance <= 2 && ItemType != CollectableType.Stops)
				{
					PutCollectableDownAtBase();
				}

				//stops drop off
				if (ItemType == CollectableType.Stops )
				{
					var index = TaskObject.GetComponent<Construction_Script>().Workerlist_Stops.IndexOf(this.gameObject);
					var StopPoint = TaskObject.GetComponent<Construction_Script>().RequiredStopsList[index];
					var distanceToStop = Vector3.Distance(transform.position, StopPoint.transform.position);
					this.GetComponent<NavMeshAgent>().SetDestination(StopPoint.transform.position);

					if (distanceToStop <= 1)
					{
						placeStopDown();
					}
				}
			}
		}

		if (UnSelectable)
		{
			Parent.GetComponent<SelectCode>().Selectable = false;
		}
		else
		{
			Parent.GetComponent<SelectCode>().Selectable = true;
		}

		if(Arrived == false && GetComponent<NavMeshAgent>().remainingDistance < 2)
		{
			Arrived = true;
			ArrivedAtDest();
		}
	}

	public void Drilling()
	{
		TaskObject.GetComponent<Work_Script>().Health -= DrillStrength * Time.deltaTime;
	}

	public void StartDrilling()
	{
		GetComponent<NavMeshAgent>().ResetPath();
		CurrentTask = CurrentJob.Drilling;
	}

	public void ClearingRubble()
	{
		TaskObject.GetComponent<Work_Script>().Health -= DrillStrength * Time.deltaTime;
	}

	public void StartClearingRubble()
	{
		GetComponent<NavMeshAgent>().ResetPath();
		CurrentTask = CurrentJob.ClearingRubble;
	}

	public void PickUpCollectable()
	{
		CurrentTask = CurrentJob.CarringCollectable;
		TaskObject.GetComponent<Collectable>().CollectedCart = CollectionSpace;
		Items.Add(TaskObject);
		ItemType = TaskObject.GetComponent<Collectable>().CollectableType;
		FindNearestCollectableDropOff();
	}

	public void FindNearestCollectableDropOff()
	{
		var set = false;

		if(System_Script.ConstructionSites.Count > 0 && !set)
		{
			var shortestPath = this.ShortestPath(System_Script.ConstructionSites, ExtraCommands.R_Ore_Crystal);
			GetComponent<NavMeshAgent>().SetPath(shortestPath);

			if(ItemType == CollectableType.Stops)
			{
				var index = shortestPath.Object.GetComponent<Construction_Script>().Workerlist_Stops.IndexOf(this.gameObject);
				var point = shortestPath.Object.GetComponent<Construction_Script>().RequiredStopsList[index];
				GetComponent<NavMeshAgent>().SetDestination(point.transform.position);
			}

			if(shortestPath.Length != float.MaxValue)
			{
				set = true;
				CurrentTask = CurrentJob.ConstructionWorker;
			}
		}

		if (ItemType == CollectableType.Crystal && !set)
		{
			var shortestPath = this.ShortestPath(System_Script.AllBuildings, ExtraCommands.ReturnCrystal);
			GetComponent<NavMeshAgent>().SetPath(shortestPath);

			if (shortestPath.Length != float.MaxValue)
			{
				set = true;
				CurrentTask = CurrentJob.DropOffCollectable;
			}
		}


		if (ItemType == CollectableType.Ore && !set)
		{
			var shortestPath = this.ShortestPath(System_Script.AllBuildings, ExtraCommands.ReturnOre);
			GetComponent<NavMeshAgent>().SetPath(shortestPath);

			if (shortestPath.Length != float.MaxValue)
			{
				set = true;
				CurrentTask = CurrentJob.DropOffCollectable;
			}

		}

	}

	//put crystal away
	public void PutCollectableDownAtBase()
	{
		if(CurrentTask != CurrentJob.ConstructionWorker)
		{
			// used for collection of resources
			if (Items[0].GetComponent<Collectable>().CollectableType == CollectableType.Crystal)
			{
				SystemSrpt.CrystalsCollectedCart++;
				TaskChassis = TaskChassis.GatherCrystals;
			}

			if (Items[0].GetComponent<Collectable>().CollectableType == CollectableType.Ore)
			{
				SystemSrpt.OreCollectedCart++;
				TaskChassis = TaskChassis.GatherOre;
			}
		}
		else
		{
			// used for building
			if (Items[0].GetComponent<Collectable>().CollectableType == CollectableType.Ore)
			{
				TaskObject.GetComponent<Construction_Script>().Contained_Ore++;
			}

			if (Items[0].GetComponent<Collectable>().CollectableType == CollectableType.Crystal)
			{
				TaskObject.GetComponent<Construction_Script>().Contained_Crystal++;
			}
		}

		Destroy(Items[0]);
		Items.Clear();

		CurrentTask = CurrentJob.Nothing;
		TaskChassis = TaskChassis.Nothing;
	}

	public void placeStopDown()
	{
		if (Items[0].GetComponent<Collectable>().CollectableType == CollectableType.Stops)
		{
			//var parent = TaskObject.transform.parent;
			TaskObject.GetComponent<Construction_Script>().Contained_Stops++;

			System_Script.CollectableStops.Remove(Items[0]);
			var index = TaskObject.GetComponent<Construction_Script>().Workerlist_Stops.IndexOf(this.gameObject);
			TaskObject.GetComponent<Construction_Script>().RequiredStopsList.RemoveAt(index);
		}

		Destroy(Items[0]);
		Items.Clear();

		CurrentTask = CurrentJob.Nothing;
		TaskChassis = TaskChassis.Nothing;
	}

	public void ArrivedAtDest()
	{
		var run = true;

		//if the player is wandering with an Item in hand and has reched the end of its journey set by player then find a place to drop it off
		if(CurrentTask == CurrentJob.WanderAroungWithItem && ItemType != CollectableType.Nothing && run)
		{
			CurrentTask = CurrentJob.DropOffCollectable;
			FindNearestCollectableDropOff();
			run = false;
		}

		//if the player is jwalking and has no task then find a job once arrived
		if (TaskChassis == TaskChassis.JWalking && run && CurrentTask == CurrentJob.Nothing)
		{
			TaskChassis = TaskChassis.Nothing;
			run = false;
		}
	}

	// this must be set by priorities
	public void SetNextJob() 
	{
		if(TaskChassis != TaskChassis.JWalking)
		{
			TaskChassis = TaskChassis.Nothing;

			if (System_Script.CollectableCrystals.Count > 0)
			{
				//if (TaskChassis == TaskChassis.GatherCrystals || TaskChassis == TaskChassis.Nothing)
				FindAndCollectCrystal();
			}

			if (System_Script.CollectableOre.Count > 0)
			{
				TaskChassis = TaskChassis.GatherOre;
			}

			if (System_Script.CollectableOre.Count > 0 && TaskChassis == TaskChassis.GatherOre)
			{
				//if (TaskChassis == TaskChassis.GatherOre)
				FindAndCollectOre();
			}


			if (System_Script.DrillRocks.Count > 0)
			{
				TaskChassis = TaskChassis.Drilling;
			}

			if (System_Script.DrillRocks.Count > 0 && TaskChassis == TaskChassis.Drilling)
			{
				FindAndDrillRocks();
			}

			if (System_Script.ClearRubble.Count > 0)
			{
				FindAndClearRubble();
			}

			if (System_Script.CollectableStops.Count > 0)
			{
				TaskChassis = TaskChassis.GatherStops;
				FindAndCollectStops();
			}
		}
	}

	public void FindAndCollectStops()
	{
		var shortestPath = this.ShortestPath(System_Script.CollectableStops, ExtraCommands.FindUnTargeted);

		if (shortestPath.Length != float.MaxValue)
		{
			GetComponent<NavMeshAgent>().SetPath(shortestPath);
			StartCollecting();
		}
	}

	public void FindAndCollectOre()
	{
		var shortestPath = this.ShortestPath(System_Script.CollectableOre,ExtraCommands.FindUnTargeted);

		if (shortestPath.Length != float.MaxValue)
		{
			GetComponent<NavMeshAgent>().SetPath(shortestPath);
			StartCollecting();
		}
	}

	public void FindAndDrillRocks()
	{
		var shortestPath = this.ShortestPath(System_Script.DrillRocks, ExtraCommands.FindUnTargetedObjects);

		if (shortestPath.Length != float.MaxValue)
		{
			GetComponent<NavMeshAgent>().SetPath(shortestPath);
			StartDrillingProcess();
		}
	}

	public void FindAndClearRubble()
	{
		var shortestPath = this.ShortestPath(System_Script.ClearRubble, ExtraCommands.FindUnTargetedObjects);

		if (shortestPath.Length != float.MaxValue)
		{
			GetComponent<NavMeshAgent>().SetPath(shortestPath);
			StartClearingProcess();
		}
	}

	public void FindAndCollectCrystal()
	{
		var shortestPath = this.ShortestPath(System_Script.CollectableCrystals, ExtraCommands.FindUnTargeted);
		if (shortestPath.Length != float.MaxValue)
		{
			GetComponent<NavMeshAgent>().SetPath(shortestPath);
			StartCollecting();
		}
	}

	public void StartCollecting()
	{
		CurrentTask = CurrentJob.WalkToCollectable;
		DistFromJob = float.MaxValue;
	}

	public void StartDrillingProcess()
	{
		CurrentTask = CurrentJob.WalkingToDrill;
		DistFromJob = float.MaxValue;
	}

	public void StartClearingProcess()
	{
		CurrentTask = CurrentJob.WalkingToRubble;
		DistFromJob = float.MaxValue;
	}


}
