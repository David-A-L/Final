using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
	public float patrolSpeed = 2f;							// The nav mesh agent's speed when patrolling.
	public float chaseSpeed = 5f;							// The nav mesh agent's speed when chasing.
	public float chaseWaitTime = 5f;						// The amount of time to wait when the last sighting is reached.
	public float patrolWaitTime = 1f;						// The amount of time to wait when the patrol way point is reached.
	public Transform[] patrolWayPoints;						// An array of transforms for the patrol route.
	public bool patroling = false;							//debugging

	private EnemyDetectPlayer enemyDetectPlayer;						// Reference to the EnemySight script.
	private NavMeshAgent nav;								// Reference to the nav mesh agent.
	private float chaseTimer;								// A timer for the chaseWaitTime.
	public float patrolTimer;								// A timer for the patrolWaitTime.
	public int wayPointIndex;								// A counter for the way point array.

	void Awake () {
		enemyDetectPlayer = GetComponent<EnemyDetectPlayer>();
		nav = GetComponent<NavMeshAgent>();
	}
	
	
	void Update () {
		if (enemyDetectPlayer.HasValidTarget()) {// If enemy is aware of the player but not in sight
			Chasing ();
		} else { //enemy has not found the player
			Patrolling (); 
		}
	}
	
	void Chasing () {
		patroling = false;
		// Create a vector from the enemy to the last sighting of the player.
		Vector3 sightingDeltaPos = enemyDetectPlayer.personalLastKnownLocation - transform.position;

		if (sightingDeltaPos.sqrMagnitude > 4f) { // If the the last personal sighting of the player is not close...
//			print (enemyDetectPlayer.personalLastKnownLocation);
			// ... set the destination for the NavMeshAgent to the last personal sighting of the player.
			nav.destination = enemyDetectPlayer.personalLastKnownLocation;
			nav.Resume();
		} else {
			nav.Stop();
		}

		nav.speed = chaseSpeed;// Set the appropriate speed for the NavMeshAgent.

		if (nav.remainingDistance < nav.stoppingDistance) { // If near the last personal sighting...
			chaseTimer += Time.deltaTime; // ... increment the timer.

			if (chaseTimer >= chaseWaitTime) { // If the timer exceeds the wait time...
				chaseTimer = 0f;// ... reset last global sighting, the last personal sighting and the timer.
			}
		} else {
			chaseTimer = 0f;// If not near the last sighting personal sighting of the player, reset the timer.
		}
	}

	
	void Patrolling (){
		patroling = true;
		// Set an appropriate speed for the NavMeshAgent.
		nav.speed = patrolSpeed;
		nav.Resume ();
		
		// If near the next waypoint...
		if (nav.remainingDistance < 1.0f) { //if within a meter
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
