using UnityEngine;
using System.Collections;

public class UnitAI : MonoBehaviour {
	public GameObject owner = null;
	public bool returnHome = false;
	public Vector3 homePoint = new Vector3();
	public Vector3 destination = new Vector3();
	public int buildPointsGiven = 100;
	public int speed = 10;
	public int rotationSpeed = 3;
	public bool isActive = false;
	public float grace = 3.0f; // How close is "close 'nuff"
	public bool rechDest = false; // Shorthand for "Reached Destination"

    public bool selected = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (selected == true) {
            GetComponentInChildren<Projector>().enabled = true;
        }
        else {
            GetComponentInChildren<Projector>().enabled = false;
        }
		if(isActive){
			if(returnHome){
				// If going home is a possibility
				if(!rechDest){
					// If we have not reahced our destination
					// Go towards our destination
					movement(destination);
					if(DistanceCheck(destination)){
						rechDest = true;
					}
				}
				if(rechDest){
					// If we have reached our desination
					// Go back home
					movement(homePoint);
					if(DistanceCheck(homePoint)){
						rechDest = false;
						placeInside();
					}
				}
			}
			else{
				// If going home is not possible
				if(!rechDest){
					// If we have not reahced our destination
					// Go towards our destination
					movement(destination);
					if(DistanceCheck(destination)){
						rechDest = true;
					}
				}
			}
		}
	}

	bool DistanceCheck(Vector3 poi){
		// "poi" stands for "point of interest"
		if(Vector3.Distance(poi, transform.position) < grace){
			return true;
		}
		else{
			return false;
		}
	}

	void placeInside(){
		if(owner == null){
			Debug.LogError("Unit does not have an owner");
		}
		else{
			GameObject.Destroy(transform.gameObject);
		}

	}

	void movement(Vector3 whereYaGoing){
		transform.rotation = Quaternion.Slerp(transform.rotation,
		                                      Quaternion.LookRotation(whereYaGoing - transform.position), 
		                                      rotationSpeed*Time.deltaTime);
		transform.position += transform.forward * speed * Time.deltaTime;
	}
}
