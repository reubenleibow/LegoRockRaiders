using UnityEngine;
using System.Collections;

public enum CurrentJob
{
	Nothing,
	Drilling,
	WalkingToDrill,
	WalkToCrystal,
	CarringCrystal
}

public static class Constants
{
	public static int MinDrillDistance = 2;
}

/// <summary>
/// This used for any lego object(men,cars)
/// </summary>
public class Lego_Character : MonoBehaviour
{
	private SelectCode SelectCode;
	private GameObject Parent;
	public GameObject CollectionSpace;

	//Propeties
	public bool UnSelectable = false;
	public bool CanDrill = true;
	public int DrillStrength = 5;


	//Current Tasks(No changes doen here)
	public float DistFromJob;
	public GameObject TaskObject;
	public CurrentJob CurrentTask;
	public RaycastHit TaskPoint;

	void Start()
	{
		Parent = this.gameObject;
		SelectCode = GetComponent<SelectCode>();
		System_Script.RaidersList.Add(this.gameObject);
	}

	void Update()
	{

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
				//var DirectDistance = Vector3.Distance(this.transform.position, TaskPoint.collider.gameObject.transform.position);

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

					//if (DistFromJob > DirectDistance)
					//{
					//	DistFromJob = DirectDistance;
					//}
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

			if (CurrentTask == CurrentJob.WalkToCrystal && DistFromJob <= Constants.MinDrillDistance)
			{
				PickUpCrystal();
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

	public void PickUpCrystal()
	{
		CurrentTask = CurrentJob.CarringCrystal;
		TaskObject.GetComponent<Collectable>().Collected = CollectionSpace;
	}
}
