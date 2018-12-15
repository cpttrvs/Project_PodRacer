using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap;

[RequireComponent(typeof(Pod))]
public class Controller : MonoBehaviour {

	//UI
	[SerializeField] Text leftLabel;
	[SerializeField] Text rightLabel;
	[SerializeField] Text boostLabel;

	// Particle GameObject
	[SerializeField] ParticleSystem particleRight;
	[SerializeField] ParticleSystem particleLeft;

    //Leap
    Leap.Controller leapController;

	//inputs
	bool leapMotion = true;
	float[] intensity = {0f, 0f}; // between 0 and 1
	bool boost = false;

	//pod
	Pod pod;

	// Use this for initialization
	void Start () {
		pod = GetComponent<Pod>();
        leapController = new Leap.Controller();
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
			}
			else {
				KeyboardInput();
			}

            intensity[0] = Round(intensity[0], 1);
            intensity[1] = Round(intensity[1], 1);

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

    float Round(float value, int digits) {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    void KeyboardInput() {
		intensity[0] = Input.GetAxis("LeftPod");
		intensity[1] = Input.GetAxis("RightPod");
		boost = Input.GetButton("Fire1");
	}

    float leftRaw = 0f;
    float rightRaw = 0f;

	void LeapMotionInput() {
        float range = 50f;
        Leap.Frame frame = leapController.Frame();
        if(frame.Hands.Count == 1) {
            if (frame.Hands[0].IsLeft) {
                leftRaw = frame.Hands[0].PalmPosition.z;
            }
            else {
                rightRaw = frame.Hands[0].PalmPosition.z;
            }
        }
        else if(frame.Hands.Count == 2) {
            if (frame.Hands[0].IsLeft) {
                leftRaw = frame.Hands[0].PalmPosition.z;
                rightRaw = frame.Hands[1].PalmPosition.z;
            }
            else {
                leftRaw = frame.Hands[1].PalmPosition.z;
                rightRaw = frame.Hands[0].PalmPosition.z;
            }
        }      
        if(leftRaw < -range || leftRaw > range) {
            leftRaw = range * Mathf.Sign(leftRaw);
        }
        if (rightRaw < -range || rightRaw > range) {
            rightRaw = range * Mathf.Sign(rightRaw);
        }
        intensity[0] = - leftRaw / range;
        intensity[1] = - rightRaw / range;
        boost = Input.GetButton("Fire1");
    }
}
