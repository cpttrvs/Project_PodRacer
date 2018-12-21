using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // PlayerPrefs storage
	private static string PREFS_TUTORIAL_TIME = "TUTORIAL_TIME";
	private static string PREFS_LEVEL_TIME = "LEVEL_TIME";
    // Game status
	private static bool gameOn;
	private static bool levelFinished;
	private static float timer;
	private static bool firstLoad = true;

    // UI for fade purposes
	[SerializeField] GameObject fadeQuad;
	[SerializeField] GameObject canvas;

	// Gameplay UI
	[SerializeField] GameObject gameplayUI;
	[SerializeField] TextMesh hightime3DText;
	[SerializeField] Text countdownText;
	[SerializeField] TextMesh timer3DText;
	[SerializeField] Text tipText;

	// Finish UI
	[SerializeField] GameObject finishUI;
	[SerializeField] Text finishTimeText;
	[SerializeField] Text newHightimeText;

	// Use this for initialization
	void Start () {
		SetHighTime();
        // If we came for the intro scene we fade in
		if(firstLoad){
			firstLoad = false;
			StartCoroutine(FadeIn());
		}
        // Else we just go straight to the countdown
		else{
			StartCoroutine(Countdown());
		}
		gameOn = false;
		levelFinished = false;
		timer = 0f;
	}
	
	void FixedUpdate () {
        // If the game is on we update the timer and show it
		if(gameOn){
			timer += Time.deltaTime;
			timer3DText.text = "\n"+FormatTime(timer);
		}
	}

	// Fades the game in
	IEnumerator FadeIn(){
		Color fadeColor = fadeQuad.GetComponent<MeshRenderer>().material.color;
		CanvasGroup canvasGroup = canvas.GetComponent<CanvasGroup>();
		// This time we start from black and go transparent
		fadeColor.a = 1;
		canvasGroup.alpha = 0;
		fadeQuad.GetComponent<MeshRenderer>().material.color = fadeColor;
		bool countdownStarted = false;
		while(fadeColor.a > 0){
			fadeColor.a -= Time.deltaTime / 3;
			canvasGroup.alpha += Time.deltaTime / 3;
			fadeQuad.GetComponent<MeshRenderer>().material.color = fadeColor;
			if(fadeColor.a > 0.75f && !countdownStarted){
				countdownStarted = true;
				tipText.gameObject.SetActive(true);
				StartCoroutine(Countdown());
			}
			yield return null;
		}
		yield return null;
	}

	// Race 3 2 1 countdown
	IEnumerator Countdown(){
		countdownText.gameObject.SetActive(true);
		for(int i=3; i>0; i--){
			countdownText.text = i.ToString();
			if(i == 1)
				tipText.gameObject.SetActive(false);
			yield return new WaitForSeconds(1);
		}
		gameOn = true;
		countdownText.text = "GO";
		yield return new WaitForSeconds(1);
		countdownText.text = "";
	}

	// If a new high time is set a blinking text appears
	IEnumerator Hightime(){
		newHightimeText.gameObject.SetActive(true);
		while(true){
			newHightimeText.gameObject.SetActive(true);
			yield return new WaitForSeconds(1f);
			newHightimeText.gameObject.SetActive(false);
			yield return new WaitForSeconds(1f);
		}
	}

	public static bool GameOn(){
		return gameOn;
	}

	public static bool LevelFinished(){
		return levelFinished;
	}

	// On crossing the finish line we need to update the different timers
	public void CrossedFinishLine(){
		gameOn = false;
		levelFinished = true;
		finishTimeText.text = FormatTime(timer);
		bool newTime = false;
		// Tutorial new high time
		if(SceneManager.GetActiveScene().buildIndex == 1 && 
			(timer < PlayerPrefs.GetFloat(PREFS_TUTORIAL_TIME) || PlayerPrefs.GetFloat(PREFS_TUTORIAL_TIME) == 0)){
				PlayerPrefs.SetFloat(PREFS_TUTORIAL_TIME, timer);
				newTime = true;
		}
		// Level new high time
		else if(SceneManager.GetActiveScene().buildIndex == 2 && 
			(timer < PlayerPrefs.GetFloat(PREFS_LEVEL_TIME) || PlayerPrefs.GetFloat(PREFS_LEVEL_TIME) == 0)){
				PlayerPrefs.SetFloat(PREFS_LEVEL_TIME, timer);
				newTime = true;
		}
		// Set the new high time and show the blinking text
		if(newTime){
			SetHighTime();
			StartCoroutine(Hightime());
		}
		gameplayUI.SetActive(false);
		finishUI.SetActive(true);
	}

	private string FormatTime(float time){
		return time.ToString("F2").Replace(",", ".")+"s";
	}

	public void RestartLevel(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	private void SetHighTime(){
		float time = 0f;
		// Tutorial
		if(SceneManager.GetActiveScene().buildIndex == 1){
			time = PlayerPrefs.GetFloat(PREFS_TUTORIAL_TIME);
		}
		// Level
		else if(SceneManager.GetActiveScene().buildIndex == 2){
			time = PlayerPrefs.GetFloat(PREFS_LEVEL_TIME);
		}

		if(time > 0){
			hightime3DText.text = "Best time\n"+FormatTime(time);
		}
		// No hightime yet
		else{
			hightime3DText.text = "";
		}
	}
}
