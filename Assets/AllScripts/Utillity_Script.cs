﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
		character.GetComponent<NavMeshAgent>().CalculatePath(destination.transform.position, path);

		var length = path.Length(dist);

		return new NavMeshPathEx
		{
			NavPath = path,
			Length = length,
			Object = destination
		};
	}

	public static NavMeshPathEx CalculatePath(this Lego_Character character, GameObject destination, ExtraCommands ExtraCommands)
	{

		if (ExtraCommands == ExtraCommands.ReturnCrystal)
		{
			if (destination.GetComponent<Building_Script>().CanTakeCrystal)
			{
				return CalculatePath(character, destination);
			}
		}

		if (ExtraCommands == ExtraCommands.ReturnOre)
		{
			if (destination.GetComponent<Building_Script>().CanTakeOre)
			{
				return CalculatePath(character, destination);
			}
		}

		//find the nearsest collectable that is not being collected or to be collected by a different user
		if (ExtraCommands == ExtraCommands.FindUnTargeted)
		{
			if (destination.GetComponent<Collectable>().Collector == null)
			{
				return CalculatePath(character, destination);
			}
		}

		//No path found
		return new NavMeshPathEx
		{
			NavPath = new NavMeshPath(),
			Length = float.MaxValue,
			Object = destination
		};
	}

	/// <summary>
	/// Given a character and a destination, calculate the path between them.
	/// </summary>
	//public static NavMeshPathEx CalculatePath(this NavMeshAgent character, GameObject destination)
	//{
	//	var path = new NavMeshPath();
	//	var dist = Vector3.Distance();
	//	character.CalculatePath(destination.transform.position, path);

	//	var length = path.Length();

	//	return new NavMeshPathEx
	//	{
	//		NavPath = path,
	//		Length = length,
	//		Object = destination
	//	};
	//}

	public static NavMeshPathEx ShortestPath(this Lego_Character LegoUnit, List<GameObject> listOfGameObjects)
	{
		// get all the paths
		var results = listOfGameObjects.Select(dest => LegoUnit.CalculatePath(dest));
	
		// get the shortest path after sorting (shortest at the top)
		var shortest = results.OrderBy(r => r.Length).FirstOrDefault();

		// get the actual mesh path
		return shortest;
	}

	public static NavMeshPathEx ShortestPath(this Lego_Character LegoUnit, List<GameObject> listOfGameObjects, ExtraCommands ExtraCommands)
	{
		var results = listOfGameObjects.Select(dest => LegoUnit.CalculatePath(dest, ExtraCommands)).ToArray();
		var shortest = results.OrderBy(r => r.Length).FirstOrDefault();

		//make sure that the length of the closest object is not max(could be a fake route)
		if(ExtraCommands == ExtraCommands.FindUnTargeted && shortest.Object.GetComponent<Collectable>().Collector == null && shortest.Length != float.MaxValue)
		{
			shortest.Object.GetComponent<Collectable>().Collector = LegoUnit.gameObject;
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
