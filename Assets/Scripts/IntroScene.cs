using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leap;

public class IntroScene : MonoBehaviour {

	[SerializeField] GameObject titleBanner;
	[SerializeField] GameObject handModels;
	[SerializeField] GameObject canvasGO;

	[SerializeField] GameObject fadeQuad;

	bool loadTutorial;
	Color fadeColor;
	CanvasGroup canvasGroup;

	// Particle GameObject
	[SerializeField] ParticleSystem particleRight;
	[SerializeField] ParticleSystem particleLeft;

	float[] intensity = {0f, 0f};
	
	// Pod
	Pod pod;

	// Leap
	Leap.Controller leapController;
	Leap.Frame frame;

	// Use this for initialization
	void Start () {
		loadTutorial = false;
		pod = GetComponent<Pod>();
		leapController = new Leap.Controller();
		fadeColor = fadeQuad.GetComponent<MeshRenderer>().material.color;
		canvasGroup = canvasGO.GetComponent<CanvasGroup>();
		StartCoroutine(FadeIn());
		intensity[0] = 1;
		intensity[1] = 1;
	}
	
	// Update is called once per frame
	void Update () {
		frame = leapController.Frame();
		if(frame.Hands.Count == 2) {
			if(Vector3.Distance(new Vector3(frame.Hands[0].PalmPosition.x, frame.Hands[0].PalmPosition.y, frame.Hands[0].PalmPosition.z), 
								new Vector3(frame.Hands[1].PalmPosition.x, frame.Hands[1].PalmPosition.y, frame.Hands[1].PalmPosition.z)) < 30f){
				Debug.Log("Loading Tutorial Scene");
				loadTutorial = true;
			}
		}

		if(loadTutorial){			
			// The screen turns black
			if(fadeColor.a < 1){
				fadeColor.a += Time.deltaTime;
				canvasGroup.alpha -= Time.deltaTime;
				fadeQuad.GetComponent<MeshRenderer>().material.color = fadeColor;
			}
			// We switch scenes
			else if(fadeColor.a > 1){
				SceneManager.LoadScene(1);
			}			
		}

		pod.Move(intensity);

		// particle
		particleLeft.Emit(Mathf.RoundToInt(Mathf.Abs(intensity[0] * 1000)));
		particleRight.Emit(Mathf.RoundToInt(Mathf.Abs(intensity[1] * 1000)));

		particleLeft.GetComponent<ParticleSystemRenderer>().pivot = new Vector3(intensity[0], 0, 0);
		particleRight.GetComponent<ParticleSystemRenderer>().pivot = new Vector3(intensity[1], 0, 0);
	}

	IEnumerator FadeIn(){
		// This time we start from black and go transparent
		fadeColor.a = 1;
		fadeQuad.GetComponent<MeshRenderer>().material.color = fadeColor;
		particleLeft.gameObject.SetActive(false);
		particleRight.gameObject.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		particleLeft.gameObject.SetActive(true);
		particleRight.gameObject.SetActive(true);
		while(fadeColor.a > 0){
			fadeColor.a -= Time.deltaTime / 3f;
			fadeQuad.GetComponent<MeshRenderer>().material.color = fadeColor;
			yield return null;
		}
		yield return null;
	}

	IEnumerator DriveToTitle(){
		while(intensity[0] <= 1f){
			intensity[0] += 0.1f;
			intensity[1] += 0.1f;
			yield return new WaitForSeconds(0.25f);
		}

		intensity[1] = 0.5f;
		yield return new WaitForSeconds(1.5f);
		intensity[0] = 0.5f;

		yield return new WaitForSeconds(0.1f);
		intensity[0] = 1f;
		intensity[1] = 1f;
		
		yield return new WaitForSeconds(1.5f);
		intensity[1] = 0.5f;
		
		yield return new WaitForSeconds(1.5f);
		intensity[1] = 1f;

		yield return new WaitForSeconds(3f);
		Debug.Log("Turn: "+transform.position);
		intensity[0] = 0f;
		intensity[1] = 1f;
		
		yield return new WaitForSeconds(3f);
		intensity[0] = 0f;
		intensity[1] = 0f;
	}

	private void OnTriggerEnter(Collider collider) {
		if(collider.name.Equals("BannerTrigger"))
			titleBanner.SetActive(true);
		else if(collider.name.Equals("HandTrigger"))
			handModels.SetActive(true);
		else if(collider.name.Equals("TurnTrigger"))
			intensity[0] = 0;
		else if(collider.name.Equals("LoseTrigger"))
			intensity[1] = 0;
		else if(collider.name.Equals("ClapTrigger"))
			canvasGO.SetActive(true);
	}
}
