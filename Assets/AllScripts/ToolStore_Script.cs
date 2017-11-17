using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class ToolStore_Script : MonoBehaviour {

	public GameObject Sys;
	private GameObject Raider;
	private bool BusyCreatingMan = false;
	public int createmanque = 0;
	public GameObject CreatePoint;
	public GameObject EndPoint;
	private GameObject CreatedMan;
	private System_Script Sys_;

	// Use this for initialization
	void Start ()
	{
		Sys_ = Sys.GetComponent<System_Script>();
		Raider = Sys_.Lego_Raider;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(createmanque > 0 && !BusyCreatingMan)
		{
			CreateRaider();
		}

		if(CreatedMan != null)
		{
			//var X = CreatedMan.transform.position.x;
			//var Z = CreatedMan.transform.position.z;
			//var EndX = EndPoint.transform.position.x;
			//var EndZ = EndPoint.transform.position.z;
			//var ManSelect = CreatedMan.GetComponent<Lego_Character>();

			//if (X > EndX -0.2 && X< EndX+0.2 && Z > EndZ - 0.2 && Z < EndZ + 0.2)
			//{
				//ManSelect.UnSelectable = false;
				CreatedMan = null;
				BusyCreatingMan = false;
			//}
		}
	}

	public void CreateRaider()
	{
		BusyCreatingMan = true;
		Sys_.CreateRaiderQue--;
		CreatedMan = Instantiate(Raider, this.transform.position, Quaternion.identity) as GameObject;
		//var test = this.transform.forward * 12;
		//CreatedMan.GetComponent<NavMeshAgent>().SetDestination(this.transform.position);
		//CreatedMan.GetComponent<Lego_Character>().UnSelectable = true;
		//CreatedMan.GetComponent<Lego_Character>().TaskChassis = TaskChassis.JWalking;
		createmanque--;
	}

	public void OnDestroy()
	{
		Sys_.RaiderQueSize += createmanque;
	}
}
