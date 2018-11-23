using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pod))]
public class Controller : MonoBehaviour {

	//inputs
	bool leapMotion = false;
	float[] intensity = {0f, 0f}; // between 0 and 1

	//pod
	Pod pod;

	// Use this for initialization
	void Start () {
		pod = GetComponent<Pod>();
	}
	
	// Update is called once per frame
	void Update () {
		//reduce speed
		intensity[0] -= Time.deltaTime;
		intensity[1] -= Time.deltaTime;

		if(leapMotion) {
			LeapMotionInput();
		} else {
			KeyboardInput();
		}

		intensity[0] = (intensity[0] * 10f) / 10f;
		intensity[1] = (intensity[1] * 10f) / 10f;

		if(intensity[0] > 1) intensity[0] = 1f;
		if(intensity[0] < 0) intensity[0] = 0f;
		if(intensity[1] > 1) intensity[1] = 1f;
		if(intensity[1] < 0) intensity[1] = 0f;

		Debug.Log("Controller : L(" + intensity[0] +") R("+ intensity[1] + ")");
		pod.Move(intensity);
	}

	void KeyboardInput() {
		intensity[0] = Input.GetAxis("LeftPod");
		intensity[1] = Input.GetAxis("RightPod");
	}

	void LeapMotionInput() {
	}
}
