using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controlls : MonoBehaviour {

    public float cameraSpeed = 5f; // How fast the camera pans around the map
    public float guiHeightPercent = 15f; // The bottom percent of the screen to be used for the HUD
    private int h = 150; // The pixel perfect conversion of the guiHeightPercent

    public Vector3 maxZoomOutAngle; // The angle of the camera when completely zoomed out
    public Vector3 maxZoomInAngle; // The angle of the camera when completely zoomed in
    public float minZoomDist = 4f; // The smallest y-coordinate the camera will reach
    public float maxZoomDist = 18f; // The largest y-coordinate the camera will reach

    public bool isPlacingBuilding = false; // Is the player currently createing a pseudo building?
    public Transform psuedoBasePrefab = null; // The pseudo building for creating a base
    public Transform psuedoBarracksPrefab = null; // The pseudo building for creating a barracks
    public Transform psuedoResourcePrefab = null; // The pseudo building for creating a resource producer
    private Transform psuedoBuildingObject = null; // The pseudo building object currently being placed by the player
    public GameObject selectedBuilding = null; // The building currently selected by the player
    private GameObject[] _allBuildings; // All buildings currently in the game
    private GameObject[] _allTerritory; // All territory expanding objects currently in the game

    private Vector2 mouseClickPos; // Starting point of bounding box for unit selection
    private Vector2 mouseHoldPos; // Ending point of bounding box for unit selection

    private GameObject[] selectedUnits; // Array of all currently selected units

    // Use this for initialization
    void Start() {
        h = (int)(guiHeightPercent / 100) * Screen.height; // Calculate the pixel amount from the guiHeightPercent
    }

    // Update is called once per frame
    void Update() {
        #region Camera Movement
        //Edge Scrolling
        if (Input.mousePosition.x < 5) {
            transform.position = transform.position + new Vector3(-cameraSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.mousePosition.x > Screen.width - 5f) {
            transform.position = transform.position + new Vector3(cameraSpeed * Time.deltaTime, 0, 0);
        }
        if (Input.mousePosition.y < 5) {
            transform.position = transform.position + new Vector3(0, 0, -cameraSpeed * Time.deltaTime);
        }
        if (Input.mousePosition.y > Screen.height - 5) {
            transform.position = transform.position + new Vector3(0, 0, cameraSpeed * Time.deltaTime);
        }

        // Zooming
        float msw = Input.GetAxis("Mouse ScrollWheel");
        Quaternion mzoa = new Quaternion(); // Max Zoom Out Angle
        Quaternion mzia = new Quaternion(); // Max Zoom In Angle

        //mzoa.eulerAngles.Set(maxZoomOutAngle.x, maxZoomOutAngle.y, maxZoomOutAngle.z);
        mzoa.eulerAngles = maxZoomOutAngle;
        //mzia.eulerAngles.Set(maxZoomInAngle.x, maxZoomInAngle.y, maxZoomInAngle.z);
        mzia.eulerAngles = maxZoomInAngle;

        if (msw != 0f && transform.position.y >= minZoomDist && transform.position.y <= maxZoomDist) {
            transform.position = transform.position + new Vector3(0, -msw * cameraSpeed, 0);
            transform.rotation = Quaternion.Slerp(mzia, mzoa, (transform.position.y - minZoomDist) / (maxZoomDist - minZoomDist));
            Debug.Log((transform.position.y - minZoomDist) / (maxZoomDist - minZoomDist));
        }

        // Zoom Bounds Checking
        if (transform.position.y < minZoomDist)
            transform.position = new Vector3(transform.position.x, minZoomDist, transform.position.z);
        if (transform.position.y > maxZoomDist)
            transform.position = new Vector3(transform.position.x, maxZoomDist, transform.position.z);
        #endregion Camera Movement

        
        #region Mouse Controls
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        #region Placing a building
        if (isPlacingBuilding && Physics.Raycast(ray, out hit)) {
            // If we are placing a building
            psuedoBuildingObject.position = hit.point; // Move the pseudo building to the location of the mouse on the world map
            _allBuildings = getAllBuildings();
            _allTerritory = getAllTerritories();
            if (isGoodRange(hit.point, 3.0f, _allBuildings, _allTerritory)) {
                // The psuedo building is within a good building location
                psuedoBuildingObject.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
                if (Input.GetButtonUp("Fire1") && Input.mousePosition.y > h) {
                    // Act of placing the building
                    psuedoBuildingObject.GetComponent<psuedoBuilding>().hasBeenPlaced = true;
                    psuedoBuildingObject = null;
                    isPlacingBuilding = false;
                }
            }
            else {
                // The psuedo building is NOT in an ok location
                psuedoBuildingObject.GetComponentInChildren<MeshRenderer>().material.color = Color.red;
            }
        }
        #endregion Placing a building

        #region NOT Placing a building
        else {
            // if we are NOT placing a building
            if (Input.GetButtonDown("Fire1") && Input.mousePosition.y > h) {
                // If we have clicked outside of the HUD
                mouseClickPos = Input.mousePosition; // Set the position of the mouse when it is first clicked for use in unit selection

                if (Physics.Raycast(ray, out hit)) {
                    GameObject go = hit.transform.gameObject;
                    if (go.tag == "Building") {
                        selectedBuilding = hit.transform.gameObject;
                    }
                    else {
                        selectedBuilding = null;
                    }
                }
                else {
                    selectedBuilding = null;
                }
            }
            else if (Input.GetButton("Fire1") && Input.mousePosition.y > h) {
                mouseHoldPos = Input.mousePosition;
            }
            else if (Input.GetButtonUp("Fire1")) {
                selectedUnits = selectUnits();
                mouseClickPos = Vector2.zero;
                mouseHoldPos = Vector2.zero;
                Debug.Log("Button Up");
            }
        }
        #endregion NOT Placing a building
        #endregion Mouse Controls
    }

    void OnGUI() {
        Rect guiRect = new Rect(0, Screen.height - h, Screen.width, h);
        GUI.Box(guiRect, "");
        GUILayout.BeginArea(guiRect);
        if (selectedBuilding != null) {
            switch (selectedBuilding.GetComponent<Building>().buildingName) {
                case "MainBase":
                    menuOption_MainBase();
                    break;
                case "Barracks":
                    menuOption_Barracks();
                    break;
                default:
                    GUILayout.Label("Building Options Not Availible");
                    break;
            }
        }
        GUILayout.EndArea();

        // Draw Selection area
        if (Input.GetButton("Fire1") && mouseClickPos != Vector2.zero && mouseHoldPos != Vector2.zero) {
            GUI.Box(getRect(), "");
        }
    }

    void menuOption_MainBase() {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Main Base")) {
            isPlacingBuilding = true;
            clearPsuedoBuilding();
            psuedoBuildingObject = (Transform)GameObject.Instantiate(psuedoBasePrefab);
        }
        if (GUILayout.Button("Barracks")) {
            isPlacingBuilding = true;
            clearPsuedoBuilding();
            psuedoBuildingObject = (Transform)GameObject.Instantiate(psuedoBarracksPrefab);
        }
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Resource Producer")) {
            isPlacingBuilding = true;
            clearPsuedoBuilding();
            psuedoBuildingObject = (Transform)GameObject.Instantiate(psuedoResourcePrefab);
        }
    }

    void menuOption_Barracks() {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Attack Buildings")) {
            
        }
        if (GUILayout.Button("Attack Units")) {

        }
        GUILayout.EndHorizontal();
    }

    void clearPsuedoBuilding() {
        if (psuedoBuildingObject != null)
            Destroy(psuedoBuildingObject.gameObject);
    }

    GameObject[] getAllUnits() {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Unit");
        return gos;
    }

    Rect getRect() {
        return new Rect(mouseClickPos.x, Screen.height - mouseClickPos.y, mouseHoldPos.x - mouseClickPos.x, -1 * ((Screen.height - mouseClickPos.y) - (Screen.height - mouseHoldPos.y)));
    }

    GameObject[] selectUnits (){
        GameObject[] allUnits = getAllUnits();
        Rect selectionArea = getRect();
        List<GameObject> currentlySelectedUnits = new List<GameObject>();
        Debug.Log(selectionArea);
        for (int i = 0; i < allUnits.Length; i++) {
            allUnits[i].GetComponent<UnitAI>().selected = false;
            Debug.Log(Camera.main.WorldToScreenPoint(allUnits[i].transform.position));
            Vector3 unitPoint = Camera.main.WorldToScreenPoint(allUnits[i].transform.position);
            unitPoint.y = Screen.height - unitPoint.y;
            if (selectionArea.Contains(unitPoint, true)) {
                currentlySelectedUnits.Add(allUnits[i]);
                allUnits[i].GetComponent<UnitAI>().selected = true;
            }
        }
        return currentlySelectedUnits.ToArray();
    }

    GameObject[] getAllBuildings() {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Building");
        return gos;
    }

    GameObject[] getAllTerritories() {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Territory");
        return gos;
    }

    
    bool isGoodRange(Vector3 point, float minDist, GameObject[] allBuildings, GameObject[] allTerritories) {
        float currDist = 0;

        // Make sure we are not too close to other buildings
        for (int i = 0; i < allBuildings.Length; i++) {
            if (allBuildings[i].GetInstanceID() == psuedoBuildingObject.gameObject.GetInstanceID()) {
                // Skip this one, we found ourself :p
                continue;
            }
            currDist = Vector3.Distance(point, allBuildings[i].transform.position);
            if (currDist < minDist) {
                return false;
            }
        }

        // Make sure we are within our own territory
        for (int c = 0; c < allTerritories.Length; c++) {
            currDist = Vector3.Distance(point, findClosestGameObject(point, allTerritories).transform.position);
            if (currDist > 10f) {
                return false;
            }
        }

        return true;
    }

    GameObject findClosestGameObject(Vector3 p, GameObject[] gos) {
        float currDist = 10000f;
        float minDist = 10000f;
        GameObject cgo = gos[0];

        for (int i = 0; i < gos.Length; i++) {
            currDist = Vector3.Distance(p, gos[i].transform.position);
            if (currDist < minDist) {
                cgo = gos[i];
                minDist = currDist;
            }
        }

        return cgo;
    }
}