using UnityEngine;
using System.Collections;

public class Lego_Character : MonoBehaviour
{
	private SelectCode SelectCode;
	public bool UnSelectable = false;
	private GameObject Parent;
	public bool CanDrill = true;


	void Start()
	{
		Parent = this.gameObject;
		SelectCode = GetComponent<SelectCode>();
		System_Script.RaidersList.Add(this.gameObject);
	}

	void Update()
	{
		if (UnSelectable)
		{
			Parent.GetComponent<SelectCode>().Selectable = false;
		}
		else
		{
			Parent.GetComponent<SelectCode>().Selectable = true;
		}
	}
}
