using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public float speed = 2;
	CapsuleCollider playerCollider;
	// Use this for initialization
	void Start () {
		playerCollider = this.gameObject.GetComponent<CapsuleCollider> ();
	}

	// Update is called once per frame
	void Update () {
		PlayerMovement ();
	}
	void OnCollisionEnter2D(Collision2D col) {
		print("There has been a collision!!");
	}
	void PlayerMovement() {
		if (Input.GetKey (KeyCode.UpArrow)) {
			
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.up * speed);
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			//this.gameObject.transform.localPosition += (Vector3.back * 1);
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.down * speed);
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			//this.gameObject.transform.localPosition += (Vector3.left * 1);
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.left * speed);
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			//this.gameObject.transform.localPosition += (Vector3.right * 1);
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.right * speed);
		}
		if (Input.GetKey (KeyCode.Space)) {
			//			this.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * (gravity + (speed * 2)));
			this.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector3.up * 1000);
		}

	}
}


