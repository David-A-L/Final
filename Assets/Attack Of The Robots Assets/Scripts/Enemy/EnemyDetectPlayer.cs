﻿using UnityEngine;
using System.Collections;

public class EnemyDetectPlayer: MonoBehaviour
{
	public float fieldOfViewAngle = 110f;				// Number of degrees, centred on forward, for the enemy see.
	public bool playerInSight;							// Whether or not the player is currently sighted.
	public Vector3 personalLastSighting;				// Last place this enemy spotted the player.
	public GameObject gun;

	private NavMeshAgent nav;							// Reference to the NavMeshAgent component.
	private SphereCollider col;							// Reference to the sphere collider trigger component.
	private Animator anim;								// Reference to the Animator.
	private GlobalLastPlayerSighting lastPlayerSighting;	// Reference to last global sighting of the player.
	private GameObject player;							// Reference to the player.
	private Animator playerAnim;						// Reference to the player's animator component.
	private PlayerHealth playerHealth;				// Reference to the player's health script.
	private AnimatorHashIDs hash;							// Reference to the HashIDs.
	private Vector3 previousSighting;					// Where the player was sighted last frame.
	private int numDetectedPlayers = 0;

	
	void Awake ()
	{
		// Setting up the references.
		nav = GetComponent<NavMeshAgent>();
		col = GetComponent<SphereCollider>();
		anim = GetComponent<Animator>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(InGameTags.gameController).GetComponent<GlobalLastPlayerSighting>();
		player = GameObject.FindGameObjectWithTag(InGameTags.player);
		playerAnim = player.GetComponent<Animator>();
		playerHealth = player.GetComponent<PlayerHealth>();
		hash = GameObject.FindGameObjectWithTag(InGameTags.gameController).GetComponent<AnimatorHashIDs>();

		// Set the personal sighting and the previous sighting to the reset position.
		personalLastSighting = lastPlayerSighting.resetPosition;
		previousSighting = lastPlayerSighting.resetPosition;
	}
	
	
	void Update ()
	{
		// If the last global sighting of the player has changed...
//		if (lastPlayerSighting.position != previousSighting) {
//			// ... then update the personal sighting to be the same as the global sighting.
//			personalLastSighting = lastPlayerSighting.position;
//		}
		
		// Set the previous sighting to the be the sighting from this frame.
		previousSighting = lastPlayerSighting.position;
		
		// If the player is alive...
		if (playerHealth.health > 0f) {
			anim.SetBool (hash.playerInSightBool, playerInSight); // ... set the animator parameter to whether the player is in sight or not.
		} else {
			anim.SetBool (hash.playerInSightBool, false);// ... set the animator parameter to false.
		}
	}
	
	
	void OnTriggerStay (Collider other)
	{
		// If a player or a footstep is in range
		if(other.gameObject.tag == InGameTags.player || other.gameObject.tag == InGameTags.footprint){
			if(other.gameObject.tag == InGameTags.footprint && other.gameObject.GetComponent<FootprintDirection>().hasBeenSeen){
				return;
			}
			// By default the player is not in sight.
			playerInSight = false;
			
			// Create a vector from the enemy to the player and store the angle between it and forward.
			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);
			
			// If the angle between forward and where the player is, is less than half the angle of view...
			if(angle < fieldOfViewAngle * 0.5f){
				RaycastHit hit;
				
				// ... and if a raycast towards the player from the gun and the head hits something...
				bool gunCanSeePlayer = Physics.Raycast(gun.transform.position + transform.up, direction.normalized, out hit, col.radius);
				bool headCanSeePlayer = Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius);
				if(other.gameObject.tag == InGameTags.player && gunCanSeePlayer && headCanSeePlayer){
					if(hit.collider.tag == InGameTags.player){ // ... and if the raycast hits the player...
						playerInSight = true;// ... the player is in sight.
						lastPlayerSighting.position = player.transform.position;// Set the last global sighting is the players current position.
					}
				} else if (other.gameObject.tag == "Footprint"){
					FootprintDirection footDir = other.gameObject.GetComponent<FootprintDirection>();
					if(footDir.hasBeenSeen == false && footDir.nextFootprintPos != Vector3.zero){
						personalLastSighting = footDir.nextFootprintPos;
					}
					footDir.hasBeenSeen = true;
				}
			}
			
			// Store the name hashes of the current states.
			int playerLayerZeroStateHash = playerAnim.GetCurrentAnimatorStateInfo(0).nameHash;
			int playerLayerOneStateHash = playerAnim.GetCurrentAnimatorStateInfo(1).nameHash;
			
			// If the player is running
			if(playerLayerZeroStateHash == hash.locomotionState){
				if(CalculatePathLength(player.transform.position) <= col.radius){ // ... and if the player is within hearing range...
					// ... set the last personal sighting of the player to the player's current position.
					personalLastSighting = player.transform.position;
				}
			}
		}
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == InGameTags.player) {// If the player enters the trigger zone
			numDetectedPlayers++; //increment number of players in the trigger zone
			playerInSight = true; //a player is in trigger zone
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.gameObject.tag == InGameTags.player) {// If the player enters the trigger zone
			numDetectedPlayers--; //increment number of players in the trigger zone
			playerInSight = numDetectedPlayers > 0; //update players in sight.
		}
	}
	
	
	float CalculatePathLength (Vector3 targetPosition)
	{
		// Create a path and set it based on a target position.
		NavMeshPath path = new NavMeshPath();
		if (nav.enabled) {
			nav.CalculatePath (targetPosition, path);
		}
		
		// Create an array of points which is the length of the number of corners in the path + 2.
		Vector3 [] allWayPoints = new Vector3[path.corners.Length + 2];
		
		// The first point is the enemy's position.
		allWayPoints[0] = transform.position;
		
		// The last point is the target position.
		allWayPoints[allWayPoints.Length - 1] = targetPosition;
		
		// The points inbetween are the corners of the path.
		for(int i = 0; i < path.corners.Length; i++){
			allWayPoints[i + 1] = path.corners[i];
		}
		
		// Create a float to store the path length that is by default 0.
		float pathLength = 0;
		
		// Increment the path length by an amount equal to the distance between each waypoint and the next.
		for(int i = 0; i < allWayPoints.Length - 1; i++){
			pathLength += Vector3.Distance(allWayPoints[i], allWayPoints[i + 1]);
		}
		
		return pathLength;
	}

	void SetTargetPlayer(GameObject p){
		player = p;
		playerHealth = p.GetComponent<PlayerHealth> ();
	}
}
