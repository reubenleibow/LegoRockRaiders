using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum CollectableType
{
	Ore,
	Crystal,
	Stops,
	Nothing
}


public class Collectable : MonoBehaviour {

	public GameObject Collector = null;
	public GameObject CollectedCart = null;
	public bool HostChanged = false;
	public bool BecomeStatic = false;


	public CollectableType CollectableType = CollectableType.Nothing;

	// Use this for initialization
	void Start ()
	{
		if (CollectableType == CollectableType.Crystal)
			System_Script.AllCrystals.Add(this.gameObject);

		if (CollectableType == CollectableType.Ore)
			System_Script.AllOre.Add(this.gameObject);

		if (CollectableType == CollectableType.Stops)
			System_Script.AllStops.Add(this.gameObject);
	}

	// Update is called once per frame
	void Update ()
	{
		if (CollectedCart != null && !BecomeStatic)
			this.transform.position = CollectedCart.transform.position;

		if (Collector != null && !BecomeStatic)
		{
			var collectorS = Collector.GetComponent<Lego_Character>();
			var Contains = false;

			if (collectorS.Items.Count > 0)
			{
				for(var i = 0; i < collectorS.Items.Count; i++)
				{
					if(collectorS.Items[i] == this.gameObject)
					{
						Contains = true;
					}
				}
			}
			else
			{
				if (collectorS.TaskObject == this.gameObject)
				{
					Contains = true;
				}
			}

			if(!Contains && !BecomeStatic)
			{
				Collector = null;
				CollectedCart = null;
			}
		}

		if (Collector == null && !BecomeStatic)
		{ 
			if (CollectableType == CollectableType.Crystal)
			{

				if (!System_Script.CollectableCrystals.Contains(this.gameObject))
				{
					System_Script.CollectableCrystals.Add(this.gameObject);
				}
			}

			if (CollectableType == CollectableType.Ore)
			{
				if (!System_Script.CollectableOre.Contains(this.gameObject))
				{
					System_Script.CollectableOre.Add(this.gameObject);
				}
			}

			if (CollectableType == CollectableType.Stops)
			{
				if (!System_Script.CollectableStops.Contains(this.gameObject))
				{
					System_Script.CollectableStops.Add(this.gameObject);
				}
			}
		}
	}

	public void DropItem()
	{
		Collector = null;
		CollectedCart = null;
	}

	private void OnDestroy()
	{
		if(CollectableType == CollectableType.Crystal)
		{
			System_Script.AllCrystals.Remove(this.gameObject);
			System_Script.CollectableCrystals.Remove(this.gameObject);
		}

		if (CollectableType == CollectableType.Ore)
		{
			System_Script.AllOre.Remove(this.gameObject);
			System_Script.CollectableOre.Remove(this.gameObject);
		}

		if (CollectableType == CollectableType.Stops)
		{
			System_Script.CollectableStops.Remove(this.gameObject);
			System_Script.AllStops.Remove(this.gameObject);
		}
	}
	
	public void ClearAllCollectors()
	{
		Collector.GetComponent<Lego_Character>().CurrentTask = CurrentJob.Nothing;
		Collector.GetComponent<Lego_Character>().TaskObject = null;
		Collector.GetComponent<Lego_Character>().TaskChassis = TaskChassis.Nothing;
	}

	public void MakeStatic()
	{
		if (CollectableType == CollectableType.Crystal)
		{
			System_Script.AllCrystals.Remove(this.gameObject);
			System_Script.CollectableCrystals.Remove(this.gameObject);
		}

		if (CollectableType == CollectableType.Ore)
		{
			System_Script.AllOre.Remove(this.gameObject);
			System_Script.CollectableOre.Remove(this.gameObject);
		}

		if (CollectableType == CollectableType.Stops)
		{
			System_Script.CollectableStops.Remove(this.gameObject);
			System_Script.AllStops.Remove(this.gameObject);
		}
		ClearAllCollectors();
		Collector = null;
		BecomeStatic = true;
	}

	public void MakeUnStatic()
	{
		if (CollectableType == CollectableType.Crystal)
			System_Script.AllCrystals.Add(this.gameObject);

		if (CollectableType == CollectableType.Ore)
			System_Script.AllOre.Add(this.gameObject);

		if (CollectableType == CollectableType.Stops)
			System_Script.AllStops.Add(this.gameObject);

		BecomeStatic = false;
		Collector = null;
		CollectedCart = null;
		HostChanged = false;
	}
}
