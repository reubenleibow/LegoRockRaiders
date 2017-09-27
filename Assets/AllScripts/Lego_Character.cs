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
	PutCollectableDownAtCentre,
	WanderAroungWithItem,

}

public enum TaskChassis
{
	GatherOre,
	GatherCrystals,
	Nothing,
	JWalking,
	Drilling,
}

public enum ExtraCommands
{
	FindUnTargeted,
	Nothing,
	ReturnCrystal,
	ReturnOre,
	FindUnTargetedObjects
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

			//If Walking to rock
			if (CurrentTask == CurrentJob.WalkingToDrill && DistFromJob <= Constants.MinDrillDistance)
			{
				StartDrilling();
			}

			//arrived at crystal and pick it up.
			if (CurrentTask == CurrentJob.WalkToCollectable && TaskObject != null && DistFromJob <= Constants.MinDrillDistance)
			{
				if (TaskObject.GetComponent<Collectable>().Collector == gameObject)
					PickUpCollectable();
			}

			if (CurrentTask == CurrentJob.Drilling)
			{
				Drilling();
			}

			//place collectable at base
			if (CurrentTask == CurrentJob.DropOffCollectable)
			{
				if(TaskObject != null)
				{
					var distance = Vector3.Distance(transform.position, TaskObject.transform.position);

					if (distance <= 2)
					{
						CurrentTask = CurrentJob.PutCollectableDownAtCentre;
						PutCollectableDownAtBase();
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
		TaskObject.GetComponent<Rock_Script>().Health -= DrillStrength * Time.deltaTime;
	}

	public void StartDrilling()
	{
		GetComponent<NavMeshAgent>().ResetPath();
		CurrentTask = CurrentJob.Drilling;
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
		if(ItemType == CollectableType.Crystal)
		{
			var shortestPath = this.ShortestPath(System_Script.AllBuildings, ExtraCommands.ReturnCrystal);
			GetComponent<NavMeshAgent>().SetPath(shortestPath);
		}

		if (ItemType == CollectableType.Ore)
		{
			var shortestPath = this.ShortestPath(System_Script.AllBuildings, ExtraCommands.ReturnOre);
			GetComponent<NavMeshAgent>().SetPath(shortestPath);
		}

		CurrentTask = CurrentJob.DropOffCollectable;
	}

	//put crystal away
	public void PutCollectableDownAtBase()
	{
		CurrentTask = CurrentJob.Nothing;

		if(Items[0].GetComponent<Collectable>().CollectableType == CollectableType.Crystal)
		{
			SystemSrpt.CrystalsCollectedCart++;
			TaskChassis = TaskChassis.GatherCrystals;
		}

		if (Items[0].GetComponent<Collectable>().CollectableType == CollectableType.Ore)
		{
			SystemSrpt.OreCollectedCart++;
			TaskChassis = TaskChassis.GatherOre;
		}

		Destroy(Items[0]);
		Items.Clear();

		TaskChassis = TaskChassis.Nothing;
	}

	public void ArrivedAtDest()
	{
		var run = true;

		//if the player is wandering with an Item in hand and has reched the end of its journey set by player then find a place to drop it off
		if(CurrentTask == CurrentJob.WanderAroungWithItem && ItemType != CollectableType.Nothing && run)
		{
			FindNearestCollectableDropOff();
			CurrentTask = CurrentJob.DropOffCollectable;
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
