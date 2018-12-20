using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Leap;

public class IntroScene : MonoBehaviour {

	[SerializeField] GameObject fadeQuad;

	[SerializeField] GameObject titleBanner;
	[SerializeField] GameObject handModels;

	// Particle GameObject
	[SerializeField] ParticleSystem particleLeft;
	[SerializeField] ParticleSystem particleRight;

	// Pod Handles
	[SerializeField] GameObject handleLeft;
	[SerializeField] GameObject handleRight;

	Color fadeColor;

	float[] intensity = {0f, 0f};
	
	// Pod
	Pod pod;

	// Use this for initialization
	void Start () {
		pod = GetComponent<Pod>();
		fadeColor = fadeQuad.GetComponent<MeshRenderer>().material.color;
		StartCoroutine(FadeIn());
		intensity[0] = 1;
		intensity[1] = 1;
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space))
			StartCoroutine(LoadLevel());

		handleLeft.transform.rotation = Quaternion.RotateTowards(handleLeft.transform.rotation, transform.rotation * Quaternion.Euler(20f * intensity[0], 0f, 0f), 1f);
		handleRight.transform.rotation = Quaternion.RotateTowards(handleRight.transform.rotation, transform.rotation * Quaternion.Euler(20f * intensity[1], 0f, 0f), 1f);

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

	IEnumerator LoadLevel(){
		//yield return new WaitForSeconds(3);
		// The screen turns black
		while(fadeColor.a < 1){
			fadeColor.a += Time.deltaTime;
			fadeQuad.GetComponent<MeshRenderer>().material.color = fadeColor;
			yield return null;
		}
		// We switch scenes
		SceneManager.LoadScene("LevelScene");
		yield return null;
	}

	private void OnTriggerEnter(Collider collider) {
		if(collider.name.Equals("BannerTrigger"))
			titleBanner.SetActive(true);
		else if(collider.name.Equals("LevelTrigger"))
			StartCoroutine(LoadLevel());
		else if(collider.name.Equals("TurnTrigger"))
			intensity[0] = 0f;
		else if(collider.name.Equals("LoseTrigger"))
			intensity[1] = 0f;
	}
}
