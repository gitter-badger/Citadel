﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MouseLookScript : MonoBehaviour {
	public bool inventoryMode;
	public Texture2D cursorTexture;
	public Vector2 cursorHotspot;
	public float lookSensitivity = 5;
	public GameObject tabControl;
	public Text dataTabHeader;
	public Text dataTabNoItemsText;
	public GameObject searchFX;
	public float lookSmoothDamp = 0.1f;
	public AudioSource SFXSource;
	public AudioClip SearchSFX;
	public GameObject searchOriginContainer;
	private float yRotationV;
	private float xRotationV;
	private float zRotationV;
	[HideInInspector]
	public float yRotation;
	private float xRotation;
	private float zRotation;
	private float currentZRotation;
	private string mlookstring1 = "";
	private string mlookstring2 = "";
	private string mlookstring3 = "";
	private string mlookstring4 = "";
	private GameObject currentSearchItem;
	private Camera playerCamera;
	
	//float headbobSpeed = 1;
	//float headbobStepCounter;
	//float headbobAmountX = 1;
	//float headbobAmountY = 1;
	//Vector3 parentLastPos;
	//float eyeHeightRatio = 0.9f;
	
	//void  Awake (){
	//parentLastPos = transform.parent.position;
	//}
	
	void  Start (){
		cursorHotspot = new Vector2 (cursorTexture.width/2, cursorTexture.height/2);
		Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		inventoryMode = true;  // Start with inventory mode turned on
		playerCamera = GetComponent<Camera>();
	}
	
	void  Update (){
		//if (transform.parent.GetComponent<PlayerMovement>().grounded)
		//headbobStepCounter += (Vector3.Distance(parentLastPos, transform.parent.position) * headbobSpeed);
		
		//transform.localPosition.x = (Mathf.Sin(headbobStepCounter) * headbobAmountX);
		//transform.localPosition.y = (Mathf.Cos(headbobStepCounter * 2) * headbobAmountY * -1) + (transform.localScale.y * eyeHeightRatio) - (transform.localScale.y / 2);
		//parentLastPos = transform.parent.position;
		if (inventoryMode == false) {
			yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
			xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
			xRotation = Mathf.Clamp(xRotation, -90, 90);  // Limit up and down angle. TIP:: Need to disable for Cyberspace!
			transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
		}
		
		if(Input.GetKeyDown(KeyCode.Tab))
			ToggleInventoryMode();

		if (!GUIState.isBlocking) {
			if(Input.GetMouseButtonDown(1)) {
				RaycastHit hit = new RaycastHit();
				if (Physics.Raycast(playerCamera.ScreenPointToRay(Input.mousePosition), out hit, 4.5f)) {
					// TIP: Use Camera.main.ViewportPointToRay for center of screen
					if (hit.collider == null)
						return;
				
					// Check if object is usable and then use it
					if (hit.collider.tag == "Usable") {
						hit.transform.SendMessageUpwards("Use");
						return;
					}
				
					if (hit.collider.tag == "Searchable") {
						currentSearchItem = hit.collider.gameObject;
						SearchObject(currentSearchItem.GetComponent<SearchableItem>().lookUpIndex);
						return;
					}
				
				
					Renderer rendererObj = hit.collider.GetComponent<MeshRenderer>();
					if (rendererObj != null && rendererObj.material != null && rendererObj.material.mainTexture != null) {
						MeshCollider meshCollider = hit.collider as MeshCollider;
						if (meshCollider == null || meshCollider.sharedMesh == null)
							return;
					
						Mesh mesh = hit.collider.gameObject.GetComponent<MeshFilter>().sharedMesh;
						int[] submeshTris;
						int[] hittedTriangle = new int[3];
						int subMeshIndex = -1;
						mlookstring1 = "hit.triangleIndex = " + hit.triangleIndex.ToString();
						hittedTriangle[0] = mesh.triangles[hit.triangleIndex * 3];
						hittedTriangle[1] = mesh.triangles[hit.triangleIndex * 3 + 1];
						hittedTriangle[2] = mesh.triangles[hit.triangleIndex * 3 + 2];
						for(int i=0;i<mesh.subMeshCount;i++) {
							submeshTris = mesh.GetTriangles(i);
							for (int j=0;j<submeshTris.Length;j += 3) {
								if(submeshTris[j] == hittedTriangle[0] && submeshTris[j+1] == hittedTriangle[1] && submeshTris[j+2] == hittedTriangle[2]) {
									subMeshIndex = i;
									mlookstring2 = "Submesh Index = " + subMeshIndex.ToString() + "\n";
									break;
								}
							}
							if (subMeshIndex != -1)
								break;
						}
						//Draw green lines on edges of hit tri
						//Vector3 p0 = mesh.vertices[mesh.triangles[(hit.triangleIndex * 3) + 0]];
						//Vector3 p1 = mesh.vertices[mesh.triangles[(hit.triangleIndex * 3) + 1]];
						//Vector3 p2 = mesh.vertices[mesh.triangles[(hit.triangleIndex * 3) + 2]];
						//Transform hitTransform = hit.collider.gameObject.transform;
						//p0 = hitTransform.TransformPoint(p0);
						//p1 = hitTransform.TransformPoint(p1);
						//p2 = hitTransform.TransformPoint(p2);
						//Debug.DrawLine(p0, p1, Color.green, 999, false);
						//Debug.DrawLine(p1, p2, Color.green, 999, false);
						//Debug.DrawLine(p2, p0, Color.green, 999, false);
					
						//Tell player that we can't use "suchnsuch" wall
						mlookstring3 = "Can't use " + rendererObj.materials[subMeshIndex].name + "\n";
					}
					// Draws a line from the camera to the raycast hit.point
					//Debug.DrawLine(transform.position, hit.point, Color.green, 999, false);
				
					//mlookstring4 = rendererObj.name;
					print("MouseLookScript: " + mlookstring1 + " " + mlookstring2 + " " + mlookstring3 + " " + mlookstring4 + "\n"); //rendererObj.name + "\n");
				}
			}
		}
	}
	
	void  ToggleInventoryMode (){
		if (inventoryMode) {
			Cursor.lockState = CursorLockMode.Locked;
			inventoryMode = false;
		} else {
			Cursor.lockState = CursorLockMode.None;
			inventoryMode = true;
		}
	}
	
	void  SearchObject ( int index  ){
		//string randomItem;
		//if (currentSearchItem.GetComponent<SearchableItem>().contents[0] == null) {
		//	if (currentSearchItem.GetComponent<SearchableItem>().generateContents) {
		//		switch(index) {
		//		case 0: randomItem = "";
		//			break;
		//		case 1: randomItem = "";
		//			break;
		//		default:randomItem = "";
		//			break;
		//		}
		//	}
		//}

		// Play search sound
		SFXSource.PlayOneShot(SearchSFX);

		// Enable search scaling box effect
		searchOriginContainer.GetComponent<RectTransform>().position = Input.mousePosition;
		searchFX.SetActive(true);
		searchFX.GetComponent<Animation>().Play();

		// Set header text on data tab
		dataTabHeader.text = currentSearchItem.GetComponent<SearchableItem>().objectName;

		// Turn off the text that displays "No Items" by default
		dataTabNoItemsText.enabled = false;

		int numberFoundContents = 0;

		for (int i=currentSearchItem.GetComponent<SearchableItem>().numSlots - 1;i>=0;i--) {
			if (currentSearchItem.GetComponent<SearchableItem>().contents[i] != null)
				numberFoundContents++;
		}

		if (numberFoundContents <=0) {
			dataTabNoItemsText.enabled = true;
		}

		// Change last active MFD tab (RH or LH depending on which was used last) to Data tab to show search contents
		if (tabControl.GetComponent<TabButtonsScript>().curTab != 4) {
			tabControl.GetComponent<TabButtonsScript>().TabButtonClickSilent(4);
		}
	}
	
	// Returns string for describing the walls/floors/etc. based on the material name
	string GetTextureDescription ( string material_name  ){
		string retval = "";
		
		// First handle the animated textures
		if (material_name.StartsWith("+"))
			retval = "normal screens";
		
		if (material_name.Contains("eng2_5"))
			retval = "power exchanger";
		
		if (material_name.Contains("fan"))
			retval = "field generation rotor";
		
		if (material_name.Contains("lift"))
			retval = "repulsor lift";
		
		if (material_name.Contains("bridg"))
			retval = "biological infestation";
		
		if (material_name.Contains("alert"))
			retval = "warning indicator";
		
		if (material_name.Contains("telepad"))
			retval = "jump disk";
		
		// Handle med* textures
		if (material_name.StartsWith("med")) {
			if (material_name == "med1_7" || material_name == "med2_6") {
				retval = "flourescent lighting";
			} else {
				retval = "medical panelling";
			}
		}
		
		if (material_name.Contains("crate"))
			retval = "storage crate";
		
		switch(material_name) {
		case "black32": retval = ""; break;
		case "black64": retval = ""; break;
		case "black128": retval = ""; break;
		case "bridg1_2": retval = "biological infestation"; break;
		case "bridg1_3": retval = "biological infestation"; break;
		case "bridg1_3b": retval = "biological infestation"; break;
		case "bridg1_4": retval = "biological infestation"; break;
		case "bridg1_5": retval = "data transfer schematic"; break;
		case "bridg2_1": retval = "monitoring port"; break;
		case "bridg2_2": retval = "stone mosaic tiling"; break;
		case "bridg2_3": retval = "monitoring port"; break;
		case "bridg2_4": retval = "video observation screen"; break;
		case "bridg2_5": retval = "cyber station"; break;
		case "bridg2_6": retval = "burnished platinum panelling"; break;
		case "bridg2_7": retval = "burnished platinum panelling"; break;
		case "bridg2_8": retval = "SHODAN neural bud"; break;
		case "bridg2_9": retval = "computer"; break;
		case "cabinet": retval = "cabinet"; break;
		case "charge_stat": retval = "energy charge station"; break;
		case "citmat1_1": retval = "CPU node"; break;
		case "citmat1_2": retval = "chair"; break;
		case "citmat1_3": retval = "catwalk"; break;
		case "citmat1_4": retval = "catwalk"; break;
		case "citmat1_5": retval = ""; break;
		case "citmat1_6": retval = "cabinet"; break;
		case "citmat1_7": retval = "catwalk"; break;
		case "citmat1_8": retval = "table top"; break;
		case "citmat1_9": retval = "catwalk"; break;
		case "citmat2_1": retval = "catwalk"; break;
		case "citmat2_2": retval = "cabinet"; break;
		case "citmat2_3": retval = "cabinet"; break;
		case "citmat2_4": retval = "cabinet"; break;
		case "console1_1": retval = "computer"; break;
		case "console1_2": retval = "computer"; break;
		case "console1_3": retval = "cart"; break;
		case "console1_4": retval = "computer"; break;
		case "console1_5": retval = "computer"; break;
		case "console1_6": retval = "console"; break;
		case "console1_7": retval = "console"; break;
		case "console1_8": retval = "console"; break;
		case "console1_9": retval = "console"; break;
		case "console2_1": retval = "console panel"; break;
		case "console2_2": retval = "desk"; break;
		case "console2_3": retval = "computer panel"; break;
		case "console2_4": retval = "computer panel"; break;
		case "console2_5": retval = "computer console"; break;
		case "console2_6": retval = "console controls"; break;
		case "console2_7": retval = "console"; break;
		case "console2_8": retval = "console controls"; break;
		case "console2_9": retval = "console"; break;
		case "console3_1": retval = "cyber space port"; break;
		case "console3_2": retval = "computer"; break;
		case "console3_3": retval = "computer"; break;
		case "console3_4": retval = "keyboard"; break;
		case "console3_5": retval = "computer panelling"; break;
		case "console3_6": retval = "normal screens"; break;
		case "console3_7": retval = "destroyed screen"; break;
		case "console3_8": retval = "desk"; break;
		case "console3_9": retval = "desk"; break;
		case "console4_1": retval = "console controls"; break;
		case "cyber": retval = "x-ray machine"; break;
		case "d_arrow1": retval = "repulsor lights"; break;
		case "d_arrow2": retval = "repulsor lights"; break;
		}
		return retval;
	}
}