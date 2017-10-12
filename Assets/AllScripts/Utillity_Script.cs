using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public static class Utillity_Script
{
	/// <summary>
	/// Given a path, calculate the total length.
	/// </summary>
	/// <param name="path">The path to find the length of.</param>
	/// <returns>Returns the total length of the path.</returns>
	public static float Length(this NavMeshPath path, float dist)
	{
		if (path.corners.Length < 2)
			return dist;

		Vector3 previousCorner = path.corners[0];
		float lengthSoFar = 0.0F;
		int i = 1;
		while (i < path.corners.Length)
		{
			Vector3 currentCorner = path.corners[i];
			lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
			previousCorner = currentCorner;
			i++;
		}

		return lengthSoFar;
	}

	public static NavMeshPathEx CalculatePath(this Lego_Character character, GameObject destination)
	{
		var path = new NavMeshPath();
		var dist = Vector3.Distance(character.transform.position,destination.transform.position);
		character.GetComponent<NavMeshAgent>().CalculatePath(new Vector3(destination.transform.position.x,0, destination.transform.position.z), path);

		var length = path.Length(dist);

		return new NavMeshPathEx
		{
			NavPath = path,
			Length = length,
			Object = destination
		};
	}

	//Then come here and do this event multiple times----->
	public static IEnumerable<NavMeshPathEx> ChosePathDestChasis(this Lego_Character character, GameObject destination, ExtraCommands ExtraCommands)
	{
		var Man = character.GetComponent<Lego_Character>();

		if (ExtraCommands == ExtraCommands.R_Ore_Crystal)
		{
			if(Man.ItemType == CollectableType.Crystal)
			{
				if (destination.GetComponent<Construction_Script>().Crystal_Worker)
				{
					yield return CalculatePath(character, destination);
				}
			}

			if (Man.ItemType == CollectableType.Ore)
			{
				if (destination.GetComponent<Construction_Script>().Ore_Worker)
				{
					yield return CalculatePath(character, destination);
				}
			}

			if (Man.ItemType == CollectableType.Stops)
			{
				if (destination.GetComponent<Construction_Script>().Stops_Worker)
				{
					//foreach (var stop in destination.GetComponent<Construction_Script>().RequiredStopsListPoints)
					//{
						//yield return CalculatePath(character, stop);
					//}
					yield return CalculatePath(character, destination);
				}
			}
		}

		//under is for returning ore + crystals
		if (ExtraCommands == ExtraCommands.ReturnCrystal)
		{
			if (destination.GetComponent<Building_Script>().CanTakeCrystal)
			{
				yield return CalculatePath(character, destination);
			}
		}

		if (ExtraCommands == ExtraCommands.ReturnOre)
		{
			if (destination.GetComponent<Building_Script>().CanTakeOre)
			{
				yield return CalculatePath(character, destination);
			}
		}

		//find the nearsest collectable that is not being collected or to be collected by a different user
		if (ExtraCommands == ExtraCommands.FindUnTargeted)
		{
			if (destination.GetComponent<Collectable>().Collector == null)
			{
				yield return CalculatePath(character, destination);
			}
		}

		if (ExtraCommands == ExtraCommands.FindUnTargetedObjects)
		{ 
			if(destination != null)
			{
				if (destination.GetComponent<Work_Script>().Worker == null)
				{
					yield return CalculatePath(character, destination);
				}
			}
		}

		//No path found
		yield return new NavMeshPathEx
		{
			NavPath = new NavMeshPath(),
			Length = float.MaxValue,
			Object = destination
		};
	}

	//first come here and dp this event----->
	public static NavMeshPathEx ShortestPath(this Lego_Character LegoUnit, List<GameObject> listOfGameObjects, ExtraCommands ExtraCommands)
	{
		var results = listOfGameObjects.SelectMany(dest => LegoUnit.ChosePathDestChasis(dest, ExtraCommands)).ToArray();
		var shortest = results.OrderBy(r => r.Length).FirstOrDefault();
		var Man = LegoUnit.GetComponent<Lego_Character>();

		//make sure that the length of the closest object is not max(could be a fake route)
		//look for construction sites
		if (ExtraCommands == ExtraCommands.R_Ore_Crystal)
		{
			if (shortest.Length != float.MaxValue)
			{
				if(Man.ItemType == CollectableType.Ore)
				{
					shortest.Object.GetComponent<Construction_Script>().Workerlist_Ore.Add(LegoUnit.gameObject);
				}

				if (Man.ItemType == CollectableType.Crystal)
				{
					shortest.Object.GetComponent<Construction_Script>().Workerlist_Crystal.Add(LegoUnit.gameObject);
				}
			}

			if (shortest.Length != float.MaxValue)
			{
				shortest.Object.GetComponent<Construction_Script>().Workerlist_Stops.Add(LegoUnit.gameObject);
			}
		}


		//FindUnTArgeted is for collectables
		if (ExtraCommands == ExtraCommands.FindUnTargeted)
		{
			if(shortest.Object.GetComponent<Collectable>().Collector == null && shortest.Length != float.MaxValue)
			{
				shortest.Object.GetComponent<Collectable>().Collector = LegoUnit.gameObject;
			}
		}

		//FindUnTargetedObjects is for work jobs like drilling and rubble
		if (ExtraCommands == ExtraCommands.FindUnTargetedObjects)
		{

			if(shortest.Object.GetComponent<Work_Script>().Worker == false && shortest.Length != float.MaxValue)
			{
				shortest.Object.GetComponent<Work_Script>().Worker = LegoUnit.gameObject;
			}
		}

		return shortest;
	}

	public static void SetPath(this NavMeshAgent character, NavMeshPathEx path)
	{
		character.SetPath(path == null ? null : path.NavPath);
		character.GetComponent<Lego_Character>().TaskObject = path.Object;
	}
}

public class NavMeshPathEx
{
	public NavMeshPath NavPath { get; set; }
	public float Length { get; set; }
	public GameObject Object { get; set; }
}
