using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pod : MonoBehaviour {
	Rigidbody rb;
	// thrusters
	[SerializeField] float speedModifier = 3f;
	[SerializeField] float tiltModifier = 3f;
	float speed = 1f;
	float currentAngle = 0f;
	float currentTilt = 0f;
	float[] thrusterIntensity = {0.0f, 0.0f};


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Vector3 oppositeVelocity = -rb.velocity;
   		//rb.AddForce(oppositeVelocity);

		

		if(currentTilt > 0)
			currentTilt -= Time.deltaTime;
		if(currentTilt < 0) 
			currentTilt += Time.deltaTime;
		transform.Rotate(new Vector3(0,0,1) * currentTilt);
	}

	public void Move(float[] intensity) {
		thrusterIntensity[0] = intensity[0];
		thrusterIntensity[1] = intensity[1];
		float angleLeft = intensity[0];
		float angleRight = intensity[1];


		if(angleLeft != 0 || angleRight != 0) {
			if(angleLeft == angleRight) {
				// forward
				speed = angleRight * speedModifier;
			} else {
				if(angleLeft > angleRight) {
					// left higher, so turn right
					currentAngle = (angleLeft - angleRight);
					currentTilt = -(Mathf.Abs(intensity[0] - intensity[1]));
				} else if (angleRight > angleLeft) {
					// right higher, so turn left
					currentAngle = -(angleRight - angleLeft);
					currentTilt = Mathf.Abs(intensity[0] - intensity[1]);
				}

				float tempLeft = angleLeft; float tempRight = angleRight;
				if(tempLeft < 0f) tempLeft = 0f;
				if(tempRight < 0f) tempRight = 0f;
				speed = Mathf.Abs(tempLeft - tempRight)/speedModifier;
				
				transform.Rotate(Vector3.up * currentAngle);
			}
			rb.AddForce(transform.forward * speed, ForceMode.Impulse);
		}
	}
}
