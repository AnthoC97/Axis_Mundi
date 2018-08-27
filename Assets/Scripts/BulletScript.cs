using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {


	// Use this for initialization
	void Start () {
        Destroy(gameObject, 2f);
	}
	
	void OnCollisionEnter(Collision col) {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<SphereCollider>().enabled = false;
	}
}
