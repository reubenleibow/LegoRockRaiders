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
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (CollectedCart != null)
			this.transform.position = CollectedCart.transform.position;
	}

	public void DropItem()
	{
		Collector = null;
		CollectedCart = null;
	}
}
