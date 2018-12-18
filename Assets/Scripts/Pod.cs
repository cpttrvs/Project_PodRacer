using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class Pod : MonoBehaviour {
	// speed and angles
	[SerializeField] float speedModifier = 3f;
	[SerializeField] float tiltModifier = 2f;
	[SerializeField] float tiltSmoothness = 0.5f;
	[SerializeField] float maxInclineAngle = 15f;

	// boost
	[SerializeField] RectTransform boostBarContainer;
	[SerializeField] RectTransform boostBarFill;
	[SerializeField] float boostQuantity = 1000f;
	[SerializeField] float boostModifier = 4f;
	[SerializeField] float boostRegeneration = 3f;
	[SerializeField] float boostUsedPerFrame = 10f;
	
	Rigidbody rb;
	float maxBoostBarWidth;
	float maxBoostBarHeight;

	// values at runtime
	public float currentBoost = 0f;
	float currentAngle = 0f;
	float currentTilt = 0f;
	public float currentSpeed = 0f;
	
	// values storage
	float speed = 1f;
	float[] thrusterIntensity = {0.0f, 0.0f};


	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();
		currentSpeed = speedModifier;
		currentBoost = boostQuantity;
		// If it's not the intro (doesn't have UI)
		if(SceneManager.GetActiveScene().buildIndex != 0){
			// RectTransform width
			maxBoostBarWidth = boostBarContainer.sizeDelta.x;
			// RectTransform height
			maxBoostBarHeight = boostBarContainer.sizeDelta.y;
			// Mask top offset
			maxBoostBarHeight += -boostBarFill.transform.parent.GetComponent<RectTransform>().offsetMax.y;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Debug.Log(currentBoost);
		//Debug.Log("velocity: "+rb.velocity+" KM/H: "+rb.velocity.z/1000*60*60);

		Quaternion desiredRotation = transform.rotation;
		// Not tilting
		if(thrusterIntensity[0] == thrusterIntensity[1]){
			desiredRotation.eulerAngles = new Vector3(desiredRotation.eulerAngles.x, desiredRotation.eulerAngles.y, 0);
		}
		// Tilting right
		else if(currentTilt < 0){
			desiredRotation.eulerAngles = new Vector3(desiredRotation.eulerAngles.x, desiredRotation.eulerAngles.y, -maxInclineAngle);
		}
		// Tilting left
		else if(currentTilt > 0){
			desiredRotation.eulerAngles = new Vector3(desiredRotation.eulerAngles.x, desiredRotation.eulerAngles.y, maxInclineAngle);
		}
		transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, tiltSmoothness);
	}

	public void Move(float[] intensity) {
		thrusterIntensity[0] = intensity[0];
		thrusterIntensity[1] = intensity[1];
		float angleLeft = intensity[0];
		float angleRight = intensity[1];


		if(angleLeft != 0 || angleRight != 0) {
			if(angleLeft == angleRight) {
				// forward
				speed = angleRight * currentSpeed;
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
				speed = Mathf.Abs(tempLeft - tempRight) * currentSpeed;

				transform.RotateAround(transform.position, Vector3.up, currentAngle*tiltModifier);
			}
			rb.AddForce(transform.forward * speed, ForceMode.Impulse);
		}
	}

	public void Boost(bool value){
		if(value) {
			currentBoost -= boostUsedPerFrame;
			if(currentBoost > 0) {
				currentSpeed = speedModifier * boostModifier;
			} else {
				currentSpeed = speedModifier;
				currentBoost = 0f;
			}
		} else {
			currentSpeed = speedModifier;
			// add boost
			currentBoost += boostRegeneration;
			if(currentBoost > boostQuantity)
				currentBoost = boostQuantity;
		}
		// If it's not the intro (doesn't have UI)
		if(SceneManager.GetActiveScene().buildIndex != 0)
			UpdateBoostBarUI();
	}

	private void UpdateBoostBarUI(){
		float ratio = 1f - currentBoost / boostQuantity;
		// Left and Right offset
		float hor = - maxBoostBarWidth / 2 * ratio;
		// Top offset
		float ver = maxBoostBarHeight / 2 * ratio;
		// New Vector2(left, bottom)
		boostBarFill.offsetMin = new Vector2(hor, 0); 
		// New Vector2(-right, -top)
		boostBarFill.offsetMax = new Vector2(-hor, -ver);
	}

	private void OnTriggerEnter(Collider collider) {
		if(collider.tag.Equals("FINISH"))
			FindObjectOfType<GameManager>().SendMessage("CrossedFinishLine");
	}
}
