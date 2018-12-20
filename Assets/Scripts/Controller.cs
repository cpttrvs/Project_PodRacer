using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Pod))]
public class Controller : MonoBehaviour {

	// Particle GameObject
	[SerializeField] ParticleSystem particleRight;
	[SerializeField] ParticleSystem particleLeft;

    // Handles
    [SerializeField] GameObject handleLeft;
    [SerializeField] GameObject handleRight;

    // Leap
    Leap.Controller leapController;
	Leap.Frame frame;
    float leftRaw = 0f;
    float rightRaw = 0f;

	// Inputs
	float[] intensity = {0f, 0f}; // between -1 and 1
	bool boost = false;

	// Pod
	Pod pod;

	// Use this for initialization
	void Start () {
		pod = GetComponent<Pod>();
        leapController = new Leap.Controller();
	}
	
	// Update is called once per frame
	void Update () {
		// inputs
		if(leapController.IsConnected) {
			LeapMotionInput();
		}
		else {
			KeyboardInput();
		}

		if(GameManager.GameOn()){
			//reduce speed
			intensity[0] -= Time.deltaTime;
			intensity[1] -= Time.deltaTime;

            intensity[0] = Round(intensity[0], 1);
            intensity[1] = Round(intensity[1], 1);

            // gameplay
            pod.Move(intensity);
			pod.Boost(boost);

			// particle
			particleLeft.Emit(Mathf.RoundToInt(Mathf.Abs(intensity[0] * 1000)));
			particleRight.Emit(Mathf.RoundToInt(Mathf.Abs(intensity[1] * 1000)));

			particleLeft.GetComponent<ParticleSystemRenderer>().pivot = new Vector3(intensity[0], 0, 0);
			particleRight.GetComponent<ParticleSystemRenderer>().pivot = new Vector3(intensity[1], 0, 0);

            handleLeft.transform.rotation = Quaternion.RotateTowards(handleLeft.transform.rotation, transform.rotation * Quaternion.Euler(20f * intensity[0], 0f, 0f), 1f);
            handleRight.transform.rotation = Quaternion.RotateTowards(handleRight.transform.rotation, transform.rotation * Quaternion.Euler(20f * intensity[1], 0f, 0f), 1f);

        }
	}

    float Round(float value, int digits) {
        float mult = Mathf.Pow(10.0f, (float)digits);
        return Mathf.Round(value * mult) / mult;
    }

    void KeyboardInput() {
		if(GameManager.GameOn()){
			intensity[0] = Input.GetAxis("LeftPod");
			intensity[1] = Input.GetAxis("RightPod");
			boost = Input.GetButton("Fire1");
			if(Input.GetKeyDown(KeyCode.R)){
				FindObjectOfType<GameManager>().SendMessage("RestartLevel");
			}
		}
	}

	void LeapMotionInput() {
        float range = 50f;
        frame = leapController.Frame();
		// We retrieve the hand position information
        if(frame.Hands.Count == 1) {
            if (frame.Hands[0].IsLeft) {
                leftRaw = frame.Hands[0].PalmPosition.z;
            }
            else {
                rightRaw = frame.Hands[0].PalmPosition.z;
            }
        }
        else if(frame.Hands.Count == 2) {
			// We can restart the level by clapping
			if(Vector3.Distance(new Vector3(frame.Hands[0].PalmPosition.x, frame.Hands[0].PalmPosition.y, frame.Hands[0].PalmPosition.z), 
								new Vector3(frame.Hands[1].PalmPosition.x, frame.Hands[1].PalmPosition.y, frame.Hands[1].PalmPosition.z)) < 50f){
									FindObjectOfType<GameManager>().SendMessage("RestartLevel");
			}
			// For the boost we make fists with our hands, we only check on the index finger 'cause less costly and works fine
			boost = (GameManager.GameOn() && 
					Vector3.Cross(new Vector3(frame.Hands[0].Fingers[1].Direction.x, frame.Hands[0].Fingers[1].Direction.y, frame.Hands[0].Fingers[1].Direction.z),
												new Vector3(frame.Hands[0].PalmNormal.x, frame.Hands[0].PalmNormal.y, frame.Hands[0].PalmNormal.z)).x > 0.5f &&
					Vector3.Cross(new Vector3(frame.Hands[1].Fingers[1].Direction.x, frame.Hands[1].Fingers[1].Direction.y, frame.Hands[1].Fingers[1].Direction.z),
												new Vector3(frame.Hands[1].PalmNormal.x, frame.Hands[1].PalmNormal.y, frame.Hands[1].PalmNormal.z)).x > 0.5f);
            if (frame.Hands[0].IsLeft) {
                leftRaw = frame.Hands[0].PalmPosition.z;
                rightRaw = frame.Hands[1].PalmPosition.z;
            }
            else {
                leftRaw = frame.Hands[1].PalmPosition.z;
                rightRaw = frame.Hands[0].PalmPosition.z;
            }
        }      
		// We cap the values so that the user only has to make a hand motion in a certain range
        if(leftRaw < -range || leftRaw > range) {
            leftRaw = range * Mathf.Sign(leftRaw);
        }
        if (rightRaw < -range || rightRaw > range) {
            rightRaw = range * Mathf.Sign(rightRaw);
        }
		if(GameManager.GameOn()){
			intensity[0] = - leftRaw / range;
			intensity[1] = - rightRaw / range;
		}
    }
}
