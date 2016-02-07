using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {
	
	public int health = 1000;
	public string buildingName = "New Building";
	public int teamNumber = 0;
	
	public bool produceUnits = false;
	public Transform unit = null;
	public Transform unitSpawn = null;
	public int unitHealth = 100;
	public int unitSpeed = 5;
	public float unitFireRate = 2; // Shots per second
	public float unitDamage = 15;
	public int unitProductionRate = 30; // Units per Minute
	public int unitMax = 10000; // 10,000 is used as a default being equal to infinate.
	private float tSpawn = 0.0f;
	
	//[HideInInspector]
	//public int unitsOut = 0;
	
	public bool hasTorrent = false;
	public Transform torrentTop = null;
	public float torrentDamage = 15;
	public float torrentFireRate = 1;
	
	// Use this for initialization
	void Awake () {
		tSpawn = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(produceUnits){
			if(unit != null){
				if(unitSpawn != null){
					if(tSpawn >= unitProductionRate){
						// First thing first, reset the clock
						tSpawn = 0.0f;
						// Instantiate the gameobject as a Transform to make the "unit" prefab and
						// the new Unit variable types match
						Transform newUnit = GameObject.Instantiate(unit, unitSpawn.position, Quaternion.identity) as Transform;
						// Unity makes me write out an extra line delcaring the component as a new temp. variable
						UnitAI tempAiComp = newUnit.GetComponent<UnitAI>();
						tempAiComp.owner = transform.gameObject;
						tempAiComp.destination = new Vector3(0,0,20);

					}
					else{
						tSpawn += Time.deltaTime;
					}
				}
				else{
					Debug.LogError("No unit spawn found for : " + transform.name.ToString());
				}
			}
			else{
				Debug.LogError("No unit found for : " + transform.name.ToString());
			}
		}
	}
	
	public void Enter() {
		//unitsOut++;
	}
	public int returnTeamNumber(){
		return teamNumber;
	}
}
