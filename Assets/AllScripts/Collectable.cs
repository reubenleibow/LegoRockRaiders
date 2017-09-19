using UnityEngine;
using System.Collections;

public enum CollectableType
{
	Ore,
	Crystal,
	Nothing
}


public class Collectable : MonoBehaviour {

	public GameObject Collector;
	public GameObject CollectedCart;
	public bool HostChanged = false;



	public CollectableType CollectableType = CollectableType.Nothing;

	// Use this for initialization
	void Start ()
	{
		if (CollectableType == CollectableType.Crystal)
			System_Script.AllCrystals.Add(this.gameObject);

		if (CollectableType == CollectableType.Ore)
			System_Script.AllOre.Add(this.gameObject);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (CollectedCart != null)
			this.transform.position = CollectedCart.transform.position;

		if (Collector != null)
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

			if(!Contains)
			{
				Collector = null;
				CollectedCart = null;
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
			System_Script.AllCrystals.Remove(this.gameObject);

		if (CollectableType == CollectableType.Ore)
			System_Script.AllOre.Remove(this.gameObject);
	}
	
	public void ClearAllCollectors()
	{
		Collector.GetComponent<Lego_Character>().CurrentTask = CurrentJob.Nothing;
		Collector.GetComponent<Lego_Character>().TaskObject = null;
	}
}
