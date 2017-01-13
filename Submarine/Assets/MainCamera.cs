using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		this.gameObject.transform.position = new Vector3 (player.transform.position.x,
			player.transform.position.y, this.gameObject.transform.position.z);
		//this.gameObject.transform.position.z -= 10;
		
	}
}
