using UnityEngine;
using System.Collections;

public class LaserDoorSwitchDeactivate : MonoBehaviour
{
	public GameObject laser;				// Reference to the laser that can we turned off at this switch.
	public Material unlockedMat;		 	// The screen's material to show the laser has been unloacked.
	private GlobalLastPlayerSighting lastPlayerSighting;		// Reference to the global last sighting of the player.
	//private GameObject player;				// Reference to the player.
	bool alreadyPressed;
	
	
	void Awake ()
	{
		// Setting up the reference.
		//player = GameObject.FindGameObjectWithTag(InGameTags.player);
		lastPlayerSighting = GameObject.FindGameObjectWithTag(InGameTags.gameController).GetComponent<GlobalLastPlayerSighting>();
		alreadyPressed = false;
	}
	
	
	void OnTriggerStay (Collider other)
	{
		if (alreadyPressed) {
			return;
		}
		// If the colliding gameobject is the player...
		if (other.gameObject.tag == InGameTags.player) {
			// ... and the switch button is pressed...
			if (Input.GetButtonDown ("Switch")) {
				// ... deactivate the laser.
				LaserDeactivation (other.gameObject);
				alreadyPressed = true;
			}
		}
	}
	
	
	void LaserDeactivation (GameObject player)
	{
		bool hasKey = player.GetComponent<PlayerInventory> ().hasKey;
		// Deactivate the laser GameObject.
		laser.SetActive(false);
		
		// Store the renderer component of the screen.
		Renderer screen = transform.Find("prop_switchUnit_screen_001").GetComponent<Renderer>();
		
		// Change the material of the screen to the unlocked material.
		screen.material = unlockedMat;

		//player activated the alarm
		if (!hasKey) {
			lastPlayerSighting.position = player.transform.position;
			print ("You don't have a keycard");
		} else {
			// Play switch deactivation audio clip.
			GetComponent<AudioSource> ().Play ();
			player.GetComponent<PlayerInventory> ().hasKey = false;
			print ("Used up keycard!");
		}
	}
}
