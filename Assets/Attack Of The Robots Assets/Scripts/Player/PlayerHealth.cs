using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float health = 50f;							// How much health the player has left.
	public float resetAfterDeathTime = 5f;				// How much time from the player dying to the level reseting.
	public AudioClip deathClip;							// The sound effect of the player dying.
	private Player player;
	
	private Animator anim;								// Reference to the animator component.
	private PlayerAnimationMovement playerMovement;			// Reference to the player movement script.
	private AnimatorHashIDs hash;							// Reference to the HashIDs.
	private SceneFadeInOut sceneFadeInOut;			// Reference to the SceneFadeInOut script.
	private GlobalLastPlayerSighting lastPlayerSighting;	// Reference to the LastPlayerSighting script.
	private float timer;								// A timer for counting to the reset of the level once the player is dead.
	private bool playerDead;							// A bool to show if the player is dead or not.
	
	
	void Awake ()
	{
		// Setting up the references.
		player = GetComponent<Player>();
		anim = GetComponent<Animator>();
		playerMovement = GetComponent<PlayerAnimationMovement>();
		hash = GameObject.FindGameObjectWithTag(InGameTags.gameController).GetComponent<AnimatorHashIDs>();
		sceneFadeInOut = GameObject.FindGameObjectWithTag(InGameTags.fader).GetComponent<SceneFadeInOut>();
		lastPlayerSighting = GameObject.FindGameObjectWithTag(InGameTags.gameController).GetComponent<GlobalLastPlayerSighting>();
	}
	
	
    void Update ()
	{
		// If health is less than or equal to 0...
		if(health <= 0f)
		{
			// ... and if the player is not yet dead...
//			if(!playerDead)
//				// ... call the PlayerDying function.
//				PlayerDying();
//			else
//			{
//				// Otherwise, if the player is dead, call the PlayerDead and LevelReset functions.
//				PlayerDead();
//				LevelReset();
//			}
//TODO
			health = player.MAXHEALTH;
		}
	}
	
	
	void PlayerDying ()
	{
		// The player is now dead.
		playerDead = true;
		
		// Set the animator's dead parameter to true also.
		anim.SetBool(hash.deadBool, playerDead);
		
		// Play the dying sound effect at the player's location.
		AudioSource.PlayClipAtPoint(deathClip, transform.position);
	}
	
	
	void PlayerDead ()
	{
		// If the player is in the dying state then reset the dead parameter.
		if(anim.GetCurrentAnimatorStateInfo(0).nameHash == hash.dyingState)
			anim.SetBool(hash.deadBool, false);
		
		// Disable the movement.
		anim.SetFloat(hash.speedFloat, 0f);
//		playerMovement.enabled = false;
		//TODO
		// Reset the player sighting to turn off the alarms.
		lastPlayerSighting.position = lastPlayerSighting.resetPosition;
		
		// Stop the footsteps playing.
		GetComponent<AudioSource>().Stop();
	}
	
	
	void LevelReset ()
	{
		// Increment the timer.
		timer += Time.deltaTime;
		
		//If the timer is greater than or equal to the time before the level resets...
//		if(timer >= resetAfterDeathTime)
//			// ... reset the level.
//			sceneFadeInOut.EndScene();
		//TODO comment for player to work
	}
	
	
	public void TakeDamage (float amount)
    {
		// Decrement the player's health by amount.
        health -= amount;
        print (player.gameObject.name);
        player.health = (int)health;
    }
}
