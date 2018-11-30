using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pod : MonoBehaviour {
	Rigidbody rb;
	// thrusters
	[SerializeField] float speedModifier = 3f;
	[SerializeField] float rotateModifier = 10f;
	float speed = 1f;
	float currentAngle = 0f;
	float[] thrusterIntensity = {0.0f, 0.0f};


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Vector3 oppositeVelocity = -rb.velocity;
   		//rb.AddForce(oppositeVelocity);
	}

	public void Move(float[] intensity) {
		float angleLeft = intensity[0];
		float angleRight = intensity[1];


		if(angleLeft != 0 || angleRight != 0) {
			if(angleLeft == angleRight) {
				// forward
				speed = angleRight * speedModifier;

				rb.AddForce(transform.forward * speed, ForceMode.Impulse);
			} else {
				if(angleLeft > angleRight) {
					// left higher, so turn right
					currentAngle = (angleLeft - angleRight);
				} else if (angleRight > angleLeft) {
					// right higher, so turn left
					currentAngle = -(angleRight - angleLeft);
				}

				float tempLeft = angleLeft; float tempRight = angleRight;
				if(tempLeft < 0f) tempLeft = 0f;
				if(tempRight < 0f) tempRight = 0f;
				speed = Mathf.Abs(tempLeft - tempRight)/speedModifier;
				
				rb.AddForce(transform.forward * speed, ForceMode.Impulse);
				transform.Rotate(transform.up * currentAngle);
			}
		}

		
	}
}
