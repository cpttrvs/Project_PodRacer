using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Pod))]
public class Controller : MonoBehaviour {

	//UI
	[SerializeField] Text leftLabel;
	[SerializeField] Text rightLabel;
	[SerializeField] Text boostLabel;

	// Particle GameObject
	[SerializeField] ParticleSystem particleRight;
	[SerializeField] ParticleSystem particleLeft;

	//inputs
	bool leapMotion = false;
	float[] intensity = {0f, 0f}; // between 0 and 1
	bool boost = false;

	//pod
	Pod pod;

	// Use this for initialization
	void Start () {
		pod = GetComponent<Pod>();
	}
	
	// Update is called once per frame
	void Update () {
		if(GameManager.GameOn()){
		//reduce speed
		intensity[0] -= Time.deltaTime;
		intensity[1] -= Time.deltaTime;

		// inputs
		if(leapMotion) {
			LeapMotionInput();
		} else {
			KeyboardInput();
		}

		// gameplay
		pod.Move(intensity);
		pod.Boost(boost);

		// ui
		leftLabel.text = intensity[0].ToString();
		rightLabel.text = intensity[1].ToString();
		boostLabel.text = pod.currentBoost.ToString();

		// particle
		particleLeft.Emit(Mathf.RoundToInt(Mathf.Abs(intensity[0] * 1000)));
		particleRight.Emit(Mathf.RoundToInt(Mathf.Abs(intensity[1] * 1000)));

		particleLeft.GetComponent<ParticleSystemRenderer>().pivot = new Vector3(intensity[0], 0, 0);
		particleRight.GetComponent<ParticleSystemRenderer>().pivot = new Vector3(intensity[1], 0, 0);
		}
	}

	void KeyboardInput() {
		intensity[0] = Input.GetAxis("LeftPod");
		intensity[1] = Input.GetAxis("RightPod");
		boost = Input.GetButton("Fire1");
	}

	void LeapMotionInput() {
	}
}
