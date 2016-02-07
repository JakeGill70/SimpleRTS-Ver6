using UnityEngine;
using System.Collections;

public class psuedoBuilding : MonoBehaviour {

	public Transform completedBuilding = null;
	public float buildPointsNeeded = 1000f;
	public float buildPointsGot = 0f;
	public bool selfGenerating = false;
	public bool hasBeenPlaced = false;

	// Update is called once per frame
	void Update () {
        GetComponentInChildren<Projector>().enabled = hasBeenPlaced;
        if (hasBeenPlaced && selfGenerating){
			increaseBuildPoints(100 * Time.deltaTime);
		}
	}

	public void increaseBuildPoints(float buildPoints){
		buildPointsGot += buildPoints;
		if(buildPointsGot >= buildPointsNeeded){
			if(completedBuilding == null){
				Debug.LogError("No completed building assaigned.");
			}
			else{
				GameObject.Instantiate(completedBuilding, transform.position, transform.rotation);
				Destroy(gameObject);
			}
		}
	}

	public void OnGUI(){
		Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position);
		GUI.Box(new Rect(screenPos.x - 30,Screen.height - screenPos.y - 60,100,22), buildPointsGot.ToString("0") + "/" + buildPointsNeeded.ToString());
		//GUI.Label(new Rect(screenPos.x - 30,Screen.height - screenPos.y - 60,100,22), buildPointsGot.ToString("0") + "/" + buildPointsNeeded.ToString());
	}

}
