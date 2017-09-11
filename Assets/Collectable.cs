using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour {

	public GameObject Collector;
	public GameObject Collected;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Collected != null)
			this.transform.position = Collected.transform.position;
	}
}
