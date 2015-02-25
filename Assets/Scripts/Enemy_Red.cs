﻿using UnityEngine;
using System.Collections;

public class Enemy_Red : Enemy {
	public float jumpWaitTimeMin = 1.0f; //every 1 seconds enemy can jump
	public float jumpWaitTimeMax = 3.0f;
	public float jumpForce = 100f;

	protected bool canJump = false;
	protected float jumpTimer = 0; 
	
	protected override void FixedUpdate(){
		base.FixedUpdate ();
		
		if (jumpTimer > 0) {
			jumpTimer -= Time.deltaTime;
		}
	}
	
	protected override void MoveOnGround(){
		base.MoveOnGround ();
		if(jumpTimer <= 0){
			LookAtPlayer();
			rigidbody.AddForce(transform.up * jumpForce , ForceMode.Impulse);
			jumpTimer = Random.Range(jumpWaitTimeMin, jumpWaitTimeMax);
		}
		//moving on ground
	}

	protected override void MoveInAir(){
		base.MoveOnGround ();
		base.MoveInAir ();
	}
}