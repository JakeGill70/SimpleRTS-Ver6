using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Building))]
public class BuildingScriptEditor : Editor {

	bool unitStatsOpen = false;
	/*==============================================\\
	 * ******* NEAT IDEA THAT DIDN'T PAN OUT ****** ||
	 * ============================================ ||
	 * 												||
	 * enum namesEnum {mainBase = "Main Base",		||
	 * resourceGatherA = "*A* Depot",				||
	 * resourceGatherB = "*B* Depot", 				||
	 * tower = "Tower", 							||
	 * defenseBarracks = "Defensive Outpost",		||
	 * offenseBarracks = "Barracks"};				||
	 * namesEnum namesVar = namesEnum.mainBase;		||
	 *  											||
	 * ============================================*/

	public override void OnInspectorGUI(){
		Building myBuilding = (Building)target;
		// More of that fun code that didn't work...
		//=========================================
		//EditorGUILayout.LabelField("Name", myBuilding.buildingName);
		//namesVar = EditorGUILayout.EnumPopup("Building Type", namesVar);
		//myBuilding.buildingName = namesVar;
		//=========================================
		myBuilding.health = EditorGUILayout.IntField("Health", myBuilding.health);
		myBuilding.buildingName = EditorGUILayout.TextField("Building Name", myBuilding.buildingName);
		myBuilding.teamNumber = EditorGUILayout.IntField("Team Number", myBuilding.teamNumber);
		myBuilding.produceUnits = EditorGUILayout.Toggle("Produces Units", myBuilding.produceUnits);
		if(myBuilding.produceUnits){
			myBuilding.unit = (Transform)EditorGUILayout.ObjectField("Unit", myBuilding.unit, typeof(Transform), true);
			myBuilding.unitSpawn = (Transform)EditorGUILayout.ObjectField("Spawn Point", myBuilding.unitSpawn, typeof(Transform), true);
			myBuilding.unitProductionRate = EditorGUILayout.IntField("Production Speed", myBuilding.unitProductionRate);
			myBuilding.unitMax = EditorGUILayout.IntField("Unit Max", myBuilding.unitMax);
			unitStatsOpen = EditorGUILayout.Foldout(unitStatsOpen, "Unit Stats");
			if(unitStatsOpen){
				myBuilding.unitHealth = EditorGUILayout.IntField("Health", myBuilding.unitHealth);
				myBuilding.unitSpeed = EditorGUILayout.IntField("Speed", myBuilding.unitSpeed);
				myBuilding.unitFireRate = EditorGUILayout.FloatField("Fire Rate", myBuilding.unitFireRate);
				myBuilding.unitDamage = EditorGUILayout.FloatField("Damage", myBuilding.unitDamage);
			}

		}
		myBuilding.hasTorrent = EditorGUILayout.Toggle("Has Torrent", myBuilding.hasTorrent);
		if(myBuilding.hasTorrent){
			myBuilding.torrentTop = (Transform)EditorGUILayout.ObjectField("Torrent Gun", myBuilding.torrentTop, typeof(Transform), true);;
			myBuilding.torrentDamage = EditorGUILayout.FloatField("Damage", myBuilding.torrentDamage);
			myBuilding.torrentFireRate = EditorGUILayout.FloatField("Fire Rate", myBuilding.torrentFireRate);
		}
	}
}
