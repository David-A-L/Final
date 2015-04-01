using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
	public float patrolSpeed = 2f;							// The nav mesh agent's speed when patrolling.
	public float chaseSpeed = 5f;							// The nav mesh agent's speed when chasing.
	public float chaseWaitTime = 3f;						// The amount of time to wait when the last sighting is reached.
	public float patrolWaitTime = 0f;						// The amount of time to wait when the patrol way point is reached.
	public Transform[] patrolWayPoints;						// An array of transforms for the patrol route.
	
	
	private EnemyDetectPlayer enemyDetectPlayer;						// Reference to the EnemySight script.
	private NavMeshAgent nav;								// Reference to the nav mesh agent.
	private Transform targetPlayer;								// Reference to the player's transform.
	private PlayerHealth targetPlayerHealth;					// Reference to the PlayerHealth script.
//	private GlobalLastPlayerSighting lastPlayerSighting;		// Reference to the last global sighting of the player.
	private float chaseTimer;								// A timer for the chaseWaitTime.
	private float patrolTimer;								// A timer for the patrolWaitTime.
	private int wayPointIndex;								// A counter for the way point array.
	
	
	void Awake ()
	{
		// Setting up the references.
		enemyDetectPlayer = GetComponent<EnemyDetectPlayer>();
		nav = GetComponent<NavMeshAgent>();
		targetPlayer = GameObject.FindGameObjectWithTag(InGameTags.player).transform;
		targetPlayerHealth = targetPlayer.GetComponent<PlayerHealth>();
//		lastPlayerSighting = GameObject.FindGameObjectWithTag(InGameTags.gameController).GetComponent<GlobalLastPlayerSighting>();
	}
	
	
	void Update ()
	{
		// If the player is in sight and is alive...
		if (enemyDetectPlayer.playerInSight && targetPlayerHealth.health > 0f) {
			// ... shoot.
			Shooting ();
//		} else if (enemySight.personalLastSighting != lastPlayerSighting.resetPosition && targetPlayerHealth.health > 0f) {// If the player has been sighted and isn't dead...
		} else if (targetPlayerHealth.health > 0f) {// If the player has been sighted and isn't dead...
			// ... chase.
			Chasing ();
		} else {
			// ... patrol.
			Patrolling ();
		}
	}
	
	
	void Shooting ()
	{
		// Stop the enemy where it is.
		nav.Stop();
	}
	
	
	void Chasing ()
	{
		// Create a vector from the enemy to the last sighting of the player.
		Vector3 sightingDeltaPos = enemyDetectPlayer.personalLastSighting - transform.position;
		
		// If the the last personal sighting of the player is not close...
		if (sightingDeltaPos.sqrMagnitude > 4f) {
			// ... set the destination for the NavMeshAgent to the last personal sighting of the player.
			nav.destination = enemyDetectPlayer.personalLastSighting;
		}
		

		nav.speed = chaseSpeed;// Set the appropriate speed for the NavMeshAgent.
		

		if (nav.remainingDistance < nav.stoppingDistance) { // If near the last personal sighting...

			chaseTimer += Time.deltaTime;// ... increment the timer.
			

			if (chaseTimer >= chaseWaitTime) { // If the timer exceeds the wait time...
				// ... reset last global sighting, the last personal sighting and the timer.
//				lastPlayerSighting.position = lastPlayerSighting.resetPosition;
//				enemyDetectPlayer.personalLastSighting = lastPlayerSighting.resetPosition;
				chaseTimer = 0f;
			}
		} else {
			// If not near the last sighting personal sighting of the player, reset the timer.
			chaseTimer = 0f;
		}
	}

	
	void Patrolling (){
		// Set an appropriate speed for the NavMeshAgent.
		nav.speed = patrolSpeed;
		
		// If near the next waypoint...
		if (nav.remainingDistance < nav.stoppingDistance) {
			patrolTimer += Time.deltaTime; // ... increment the timer.
			if (patrolTimer >= patrolWaitTime) { // If the timer exceeds the wait time...
				wayPointIndex++;

				if(wayPointIndex == patrolWayPoints.Length){ //wrap around waypoints when reach end
					wayPointIndex = 0;
				}
				patrolTimer = 0;// Reset the timer.
			}
		} else {
			patrolTimer = 0;// If not near a destination, reset the timer.
		}

		// Set the destination to the patrolWayPoint.
		if (patrolWayPoints.Length > 0) {
			nav.destination = patrolWayPoints [wayPointIndex].position;
		} else {
			nav.destination = transform.position;
		}
	}
}
