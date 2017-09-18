using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CurrentJob
{
	Nothing,
	Drilling,
	WalkingToDrill,
	WalkToCollectable,
	CarringCollectable,
	DropOffCollectable,
	PutCollectableDownAtCentre,
	WanderAroungWithItem
}

//public enum ItemType
//{
//	Crystal,
//	Ore,
//	Nothing
//}


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
	private SelectCode SelectCode;
	private GameObject Parent;
	public GameObject CollectionSpace;

	public List<GameObject> Items = new List<GameObject>();

	//Propeties
	public bool UnSelectable = false;
	public bool CanDrill = true;
	public int DrillStrength = 5;
	public bool Arrived = false;


	//Current Tasks(No changes doen here)
	public float DistFromJob;
	public GameObject TaskObject;
	public CurrentJob CurrentTask;
	public RaycastHit TaskPoint;

	public CollectableType ItemType;


	public System_Script SystemSrpt;

	void Start()
	{
		Parent = this.gameObject;
		SelectCode = GetComponent<SelectCode>();
		//System_Script.RaidersList.Add(this.gameObject);
		SystemSrpt = GameObject.Find("System").GetComponent<System_Script>();
	}

	void Update()
	{
		if(Items.Count == 0 && ItemType != CollectableType.Nothing)
		{
			ItemType = CollectableType.Nothing;
		}

		if(DistFromJob > 2)
		{
			Arrived = false;
		}

		if (TaskObject != null)
		{
			var Ray = Physics.Linecast(new Vector3(this.transform.position.x, 1, this.transform.position.z),
										new Vector3(TaskObject.transform.position.x, 1, TaskObject.transform.position.z),
										out TaskPoint);


			var DirectDistance = Vector3.Distance(this.transform.position, TaskObject.transform.position);

			if (Ray)
			{
				var TagName = TaskPoint.collider.gameObject.transform.tag;
				var HasParent = TagName == "Rock" || TagName == "Crystal";

				//only tag neededs to be edited
				if (HasParent)
				{
					if (TaskPoint.collider.gameObject.transform.parent.gameObject == TaskObject)
					{
						DistFromJob = Vector3.Distance(this.transform.position, TaskPoint.point);
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

			if (CurrentTask == CurrentJob.WalkToCollectable && DistFromJob <= Constants.MinDrillDistance)
			{
				PickUpCollectable();
			}

			if (CurrentTask == CurrentJob.Drilling)
			{
				Drilling();
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

		if(CurrentTask == CurrentJob.DropOffCollectable)
		{
			var distance = Vector3.Distance(this.transform.position, TaskObject.transform.position);

			if(distance <= 2)
			{
				CurrentTask = CurrentJob.PutCollectableDownAtCentre;
				PutCollectableDownAtBase();
			}
		}

		if(Arrived == false && this.GetComponent<NavMeshAgent>().remainingDistance < 2)
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
		this.GetComponent<NavMeshAgent>().ResetPath();
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
		var shortestPath = this.ShortestPath(System_Script.ListOfAllToolStores);
		GetComponent<NavMeshAgent>().SetPath(shortestPath);

		if(shortestPath != null)
		{
			CurrentTask = CurrentJob.DropOffCollectable;
		}
	}

	public void PutCollectableDownAtBase()
	{
		CurrentTask = CurrentJob.Nothing;

		if(Items[0].GetComponent<Collectable>().CollectableType == CollectableType.Crystal)
		{
			SystemSrpt.CrystalsCollectedCart++;
		}

		if (Items[0].GetComponent<Collectable>().CollectableType == CollectableType.Ore)
		{
			SystemSrpt.OreCollectedCart++;
		}

		Destroy(Items[0]);
	}

	public void ArrivedAtDest()
	{
		if(CurrentTask == CurrentJob.WanderAroungWithItem && ItemType != CollectableType.Nothing)
		{
			FindNearestCollectableDropOff();
			CurrentTask = CurrentJob.DropOffCollectable;
		}
	}
}
