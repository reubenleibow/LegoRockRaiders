using UnityEngine;
using System.Collections;

public class ToolStore_Script : MonoBehaviour {

	public GameObject Sys;
	private GameObject Raider;
	private bool BusyCreatingMan = false;
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
		if(!BusyCreatingMan && Sys_.CreateRaiderQue > 0 && Sys_.CreateRaiderQue > 0)
		{
			BusyCreatingMan = true;
			Sys_.CreateRaiderQue--;
			CreatedMan = Instantiate(Raider, CreatePoint.transform.position, Quaternion.identity) as GameObject;
			CreatedMan.GetComponent<NavMeshAgent>().SetDestination(EndPoint.transform.position);
			CreatedMan.GetComponent<Lego_Character>().UnSelectable = true;
		}

		if(CreatedMan != null)
		{
			var X = CreatedMan.transform.position.x;
			var Z = CreatedMan.transform.position.z;
			var EndX = EndPoint.transform.position.x;
			var EndZ = EndPoint.transform.position.z;
			var ManSelect = CreatedMan.GetComponent<Lego_Character>();

			if (X > EndX -0.2 && X< EndX+0.2 && Z > EndZ - 0.2 && Z < EndZ + 0.2)
			{
				ManSelect.UnSelectable = false;
				CreatedMan = null;
				BusyCreatingMan = false;
			}
		}
	}
}
