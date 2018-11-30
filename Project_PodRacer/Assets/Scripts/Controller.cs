using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Pod))]
public class Controller : MonoBehaviour {

	//UI
	[SerializeField] Text leftLabel;
	[SerializeField] Text rightLabel;

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


		//if(intensity[0] != 0 || intensity[1] != 0) Debug.Log("Controller : L(" + intensity[0] +") R("+ intensity[1] + ")");
		pod.Move(intensity);

		leftLabel.text = intensity[0].ToString();
		rightLabel.text = intensity[1].ToString();
	}

	void KeyboardInput() {
		intensity[0] = Input.GetAxis("LeftPod");
		intensity[1] = Input.GetAxis("RightPod");
	}

	void LeapMotionInput() {
	}
}
