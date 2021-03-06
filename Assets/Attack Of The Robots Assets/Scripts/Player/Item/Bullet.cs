﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	public float lifeTime = 1.0f;
	public GameObject ragdoll;
	public GameObject splatter;

	void OnCollisionEnter(Collision other){


		if (other.transform.tag == "Enemy" && GetComponent<Rigidbody>().useGravity == false) {
			//paint splatter effect
			RaycastHit hitInfo;
			if(Physics.Raycast(transform.position, Vector3.down, out hitInfo)){ //if raycast hits something
				Instantiate(splatter, hitInfo.point + hitInfo.normal * 0.001f, Quaternion.FromToRotation(Vector3.up, hitInfo.normal)); //TODO Layer mask
			}
					
			//ragdoll effect
			Vector3 pos = other.transform.position;
			Quaternion rotation = other.transform.rotation;
			GameObject.Destroy(other.gameObject);
			Instantiate(ragdoll, pos, rotation);
		}
		GetComponent<Rigidbody>().useGravity = true;
	}

	void Update(){
		if (GetComponent<Rigidbody>().useGravity) {
			lifeTime -= Time.deltaTime;
		}

		if (lifeTime < 0) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
