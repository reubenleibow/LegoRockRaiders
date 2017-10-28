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
	WalkingToVehicle,
}

public enum TaskChassis
{
	GatherOre,
	GatherStops,
	GatherCrystals,
	Sweeping,
	Nothing,
	JWalking,
	Drilling,
	PackAwwayJunk,
	VehicleProperties,
	IsDriving,
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
	FindEmptyVehicle
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
	public bool CanSweep = true;
	public bool CanCarryOre = true;
	public bool CanCarryCrystals = true;
	public bool CanCarryStops = true;
	public bool CanDrive = true;

	public bool DriverIsSeated = false;
	public bool IsDriving = false;
	public bool CallForDriver = false; 
	public GameObject Driver;
	public GameObject DriverSeat;
	public GameObject Vehicle;

	private int DrillStrength = 50;
	public bool Arrived = false;

	//Current Tasks(No changes doen here)
	public float DistFromJob;
	public GameObject TaskObject;
	public CurrentJob CurrentTask;
	public RaycastHit TaskPoint;
	public TaskChassis TaskChassis = TaskChassis.Nothing;
	public ExtraCommands ExtraCommands = ExtraCommands.Nothing;

	public CollectableType ItemType;
	public bool DropOffTaskPointDestroyed = false;


	public System_Script SystemSrpt;
	public AddToSystemList_Script AddToSystem_Srpt;
	public NavMeshAgent navMeshAgent_SC;

	void Start()
	{
		Parent = gameObject;
		System_Script.AllWorkers.Add(this.gameObject);
		SystemSrpt = GameObject.Find("System").GetComponent<System_Script>();
		navMeshAgent_SC = this.GetComponent<NavMeshAgent>();
		AddToSystem_Srpt = this.GetComponent<AddToSystemList_Script>();
	}

	void Update()
	{
		//If ther raider is in the car and seated then play set IsdriverSeated = true;
		if (Vehicle != null && TaskChassis == TaskChassis.IsDriving)
		{
			var IsDriverSeated_ = Vehicle.GetComponent<Lego_Character>().DriverIsSeated;

			if (IsDriverSeated_)
			{
				IsDriverSeated_ = true;
			}
			else
			{
				IsDriverSeated_ = false;
			}
		}

		if(AddToSystem_Srpt.IsVehicle)
		{
			if(DriverIsSeated)
			{
				navMeshAgent_SC.enabled = true;
			}
			else
			{
				navMeshAgent_SC.enabled = false;
			}
		}

		if(TaskChassis == TaskChassis.IsDriving)
		{
			IsDriving = true;
			
		}
		else
		{
			IsDriving = false;
		}

		if(IsDriving)
		{
			this.transform.position = Vehicle.transform.position;
			this.transform.eulerAngles = Vehicle.transform.eulerAngles;
			navMeshAgent_SC.enabled = false;
			UnSelectable = true;
		}
		else
		{
			navMeshAgent_SC.enabled = true;
		}

		//check if it say that it has a driver but not actually
		if (Driver != null && CallForDriver)
		{
			var driverTaskObject = Driver.GetComponent<Lego_Character>().TaskObject;

			if(driverTaskObject != this.gameObject)
			{
				Driver = null;

				if (!System_Script.AllVehicles.Contains(this.gameObject))
				{
					System_Script.AllVehicles.Add(this.gameObject);
				}
			}
			else
			{
				if (System_Script.AllVehicles.Contains(this.gameObject))
				{
					System_Script.AllVehicles.Remove(this.gameObject);
				}
			}
		}

		if (Driver == null && CallForDriver)
		{
			if (!System_Script.AllVehicles.Contains(this.gameObject))
			{
				System_Script.AllVehicles.Add(this.gameObject);
			}
		}

		if (!AddToSystem_Srpt.IsVehicle && navMeshAgent_SC.enabled  || (AddToSystem_Srpt.IsVehicle && DriverIsSeated  && navMeshAgent_SC.enabled) )
		{
			if(DropOffTaskPointDestroyed)
			{
				TaskObject = null;
				FindNearestCollectableDropOff();
				DropOffTaskPointDestroyed = false;
			}

			//if the user says that it has an item in hand but it does not the set defalut
			if(Items.Count == 0 && ItemType != CollectableType.Nothing)
			{
				ItemType = CollectableType.Nothing;
			}

			//doing nothing than find an obnject
			if(CurrentTask == CurrentJob.Nothing && TaskChassis != TaskChassis.JWalking)
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

				//Start Drilling or sweeping oce within distance of task
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
			}
				//arrived at crystal and pick it up.
				if (CurrentTask == CurrentJob.WalkToCollectable && TaskObject != null && DistFromJob <= Constants.MinDrillDistance)
			{
				if (TaskObject.GetComponent<Collectable>().Collector == gameObject)
					PickUpCollectable();
			}
				//procede to next stage
				if (CurrentTask == CurrentJob.Drilling)
			{
				Drilling();
			}
				//procede to next stage
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

				if (distance <= 2 && ItemType == CollectableType.Stops && TaskChassis == TaskChassis.PackAwwayJunk)
				{
					PutCollectableDownAtBase();
				}

				//stops drop off
				if (ItemType == CollectableType.Stops && TaskChassis == TaskChassis.GatherStops)
				{
					var index = 0;

					if (TaskObject.GetComponent<Construction_Script>().Workerlist_Stops.Contains(this.gameObject))
					{
						//var StopPoint = new GameObject();

						index = TaskObject.GetComponent<Construction_Script>().Workerlist_Stops.IndexOf(this.gameObject);

						if(index < TaskObject.GetComponent<Construction_Script>().RequiredStopsListPoints.Count)
						{
							var StopPoint = TaskObject.GetComponent<Construction_Script>().RequiredStopsListPoints[index];
							var distanceToStop = Vector3.Distance(transform.position, StopPoint.transform.position);
							this.GetComponent<NavMeshAgent>().SetDestination(StopPoint.transform.position);

							if (distanceToStop <= 1)
							{
								placeStopDown();
							}
						}
					}
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

		if(GetComponent<NavMeshAgent>().enabled)
		{
			if(Arrived == false && GetComponent<NavMeshAgent>().remainingDistance < 2)
			{
				Arrived = true;
				ArrivedAtDest();
			}
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
	//Pick up resources-------------------------------------------------------------------------------------------------------------------------------------------
	public void FindNearestCollectableDropOff()
	{
		var set = false;

		if (System_Script.ConstructionSites.Count > 0 && !set)
		{
			var shortestPath = this.ShortestPath(System_Script.ConstructionSites, ExtraCommands.R_Ore_Crystal);
			GetComponent<NavMeshAgent>().SetPath(shortestPath);

			if(SystemSrpt.TotalStopsNeeded > 0 && ItemType == CollectableType.Stops)
			{
				var index = shortestPath.Object.GetComponent<Construction_Script>().Workerlist_Stops.IndexOf(this.gameObject);
				var point = shortestPath.Object.GetComponent<Construction_Script>().RequiredStopsListPoints[index];
				SystemSrpt.TotalStopsNeeded--;
			}

			if (shortestPath.Length != float.MaxValue)
			{
				set = true;
				CurrentTask = CurrentJob.ConstructionWorker;
			}
		}

		if (SystemSrpt.TotalStopsNeeded <= 0 && !set && System_Script.AllStops.Count > 0)
		{
			var shortestPath = this.ShortestPath(System_Script.ListOfAllToolStores, ExtraCommands.ReturnCrystal);
			GetComponent<NavMeshAgent>().SetPath(shortestPath);

			if (shortestPath.Length != float.MaxValue)
			{
				set = true;
				TaskChassis = TaskChassis.PackAwwayJunk;
				CurrentTask = CurrentJob.DropOffCollectable;
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
			Destroy(Items[0]);
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

			TaskObject.GetComponent<Construction_Script>().aquiredObj.Add(Items[0]);
			Items[0].GetComponent<Collectable>().MakeStatic();
		}

		if (Items.Count > 0)
		{
			Items.Clear();

			CurrentTask = CurrentJob.Nothing;
			TaskChassis = TaskChassis.Nothing;
		}
	}

	public void placeStopDown()
	{
		if (Items[0].GetComponent<Collectable>().CollectableType == CollectableType.Stops)
		{
			//var parent = TaskObject.transform.parent;
			TaskObject.GetComponent<Construction_Script>().Contained_Stops++;

			System_Script.CollectableStops.Remove(Items[0]);
			var index = TaskObject.GetComponent<Construction_Script>().Workerlist_Stops.IndexOf(this.gameObject);
            Items[0].transform.rotation = TaskObject.GetComponent<Construction_Script>().RequiredStopsListPoints[index].transform.rotation;
			Items[0].transform.eulerAngles += new Vector3(0,180,0);
			Items[0].transform.position = new Vector3(Items[0].transform.position.x, 0.1f, Items[0].transform.position.z);

			TaskObject.GetComponent<Construction_Script>().RequiredStopsListPoints.RemoveAt(index);

			TaskObject.GetComponent<Construction_Script>().aquiredObj.Add(Items[0]);
			Items[0].GetComponent<Collectable>().MakeStatic();
            Items[0].GetComponent<Animation>().Play("Open");

        }

		Items.Clear();

		CurrentTask = CurrentJob.Nothing;
		TaskChassis = TaskChassis.Nothing;
		ItemType = CollectableType.Nothing;
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

		if (CurrentTask == CurrentJob.WalkingToVehicle && run && TaskChassis == TaskChassis.VehicleProperties)
		{
			TaskChassis = TaskChassis.IsDriving;
			Vehicle.GetComponent<Lego_Character>().DriverIsSeated = true;
			run = false;
		}
	}

	// this must be set by priorities
	public void SetNextJob() 
	{
		if(TaskChassis != TaskChassis.JWalking)
		{
			TaskChassis = TaskChassis.Nothing;

			if (System_Script.CollectableCrystals.Count > 0 && CanCarryCrystals)
			{
				TaskChassis = TaskChassis.GatherCrystals;
				FindAndCollectCrystal();
			}


			if (System_Script.CollectableOre.Count > 0 && CanCarryOre)
			{
				TaskChassis = TaskChassis.GatherOre;
				FindAndCollectOre();
			}

			if (System_Script.DrillRocks.Count > 0 && CanDrill)
			{
				TaskChassis = TaskChassis.Drilling;
				FindAndDrillRocks();
			}

			if (System_Script.ClearRubble.Count > 0 && CanSweep)
			{
				FindAndClearRubble();
				TaskChassis = TaskChassis.Sweeping;
			}

			if (System_Script.CollectableStops.Count > 0 && CanCarryStops)
			{
				TaskChassis = TaskChassis.GatherStops;
				FindAndCollectStops();
			}

			if (System_Script.AllVehicles.Count > 0 && CanDrive)
			{
				TaskChassis = TaskChassis.VehicleProperties;
				FindVehicle();
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

	public void StartDriverProcess()
	{
		CurrentTask = CurrentJob.WalkingToVehicle;
		DistFromJob = float.MaxValue;
	}

	public void SetToDefault()
	{
		CurrentTask = CurrentJob.Nothing;
		DistFromJob = float.MaxValue;
		TaskChassis = TaskChassis.Nothing;
		TaskObject = null;
		Vehicle = null;
		UnSelectable = false;
		this.GetComponent<NavMeshAgent>().enabled = true;
	}

	public void RaiderGotOutOfVehicle()
	{
		DriverIsSeated = false;
		CallForDriver = false;
		Driver = null;
		SetToDefault();
	}

	public void FindVehicle()
	{
		var shortestPath = this.ShortestPath(System_Script.AllVehicles, ExtraCommands.FindEmptyVehicle);

		if (shortestPath.Length != float.MaxValue)
		{
			GetComponent<NavMeshAgent>().SetPath(shortestPath);
			StartDriverProcess();
		}
	}
}
