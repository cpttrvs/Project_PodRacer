using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pod : MonoBehaviour {
	Rigidbody rigidBody;
	// thrusters
	[SerializeField] float speed = 1f;
	[SerializeField] float tiltSpeed = 10f;
	[SerializeField] float thrusterSpacing = 1.0f;
	float[] thrusterIntensity = {0.0f, 0.0f};


	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Vector3 oppositeVelocity = -rigidBody.velocity;
   		rigidBody.AddRelativeForce(oppositeVelocity);

		Vector3 oppositeAngularVelocity = -rigidBody.angularVelocity;
		rigidBody.AddRelativeTorque(oppositeAngularVelocity);
	
		//ClampRotation();
	}

	public void Move(float[] intensity) {
		Vector3 direction = new Vector3(0,0,0);
		if(intensity[0] == intensity[1]) {
			direction = new Vector3(0, 0, intensity[0]);
			direction = transform.forward * intensity[0];
			//transform.position += transform.forward * intensity[0] * speed;
			rigidBody.AddRelativeForce(direction * speed,ForceMode.Impulse);
		}

		if(intensity[0] > intensity[1]) {
			direction = new Vector3(0, -intensity[0], -intensity[0]);
			rigidBody.AddRelativeTorque(direction * tiltSpeed);
		}

		if(intensity[1] > intensity[0]) {
			direction = new Vector3(0, intensity[1], intensity[1]);
			rigidBody.AddRelativeTorque(direction * tiltSpeed);
		}

		Debug.Log("Pod : " + direction.ToString());

	}

	void ClampRotation() {
		Vector3 currentRotation = transform.localRotation.eulerAngles;
		currentRotation.y = Mathf.Clamp(currentRotation.y, -40, 40);
		transform.localRotation = Quaternion.Euler(currentRotation);
	}
}
