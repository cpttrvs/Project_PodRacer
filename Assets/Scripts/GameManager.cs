using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	private static string PREFS_TIME = "TIME";
	private static bool gameOn;
	private static bool levelFinished;
	private static float timer;
	private static bool firstLoad = true;

	[SerializeField] GameObject fadeQuad;
	[SerializeField] GameObject canvas;

	// Gameplay UI
	[SerializeField] GameObject gameplayUI;
	[SerializeField] Text hightimeText;
	[SerializeField] Text countdownText;
	[SerializeField] Text timerText;
	[SerializeField] Text tipText;

	// Finish UI
	[SerializeField] GameObject finishUI;
	[SerializeField] Text finishTimeText;
	[SerializeField] Text newHightimeText;

	// Use this for initialization
	void Start () {
		SetHighTime();
		if(firstLoad){
			firstLoad = false;
			StartCoroutine(FadeIn());
		}
		else{
			StartCoroutine(Countdown());
		}
		gameOn = false;
		levelFinished = false;
		timer = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(gameOn){
			timer += Time.deltaTime;
			timerText.text = FormatTime(timer);
		}
		// We finished the level
		else if(!gameOn && timer > 0){
			if(Input.GetKeyDown(KeyCode.Space)){
				RestartLevel();
			}
		}
	}

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

	public void CrossedFinishLine(){
		gameOn = false;
		levelFinished = true;
		finishTimeText.text = FormatTime(timer);
		// If it's 0 it means that we don't have a hightime yet
		if (timer < PlayerPrefs.GetFloat(PREFS_TIME) || PlayerPrefs.GetFloat(PREFS_TIME) == 0)
        {
            PlayerPrefs.SetFloat(PREFS_TIME, timer);
			SetHighTime();
			StartCoroutine(Hightime());
		}
		gameplayUI.SetActive(false);
		finishUI.SetActive(true);
	}

	private string FormatTime(float time){
		return time.ToString("F2").Replace(",", ".")+"s";
	}

	private void SetHighTime(){
		float time = PlayerPrefs.GetFloat(PREFS_TIME);
		if(time != 0){
			hightimeText.text = "Hightime: "+FormatTime(time);
		}
		// No hightime yet
		else{
			hightimeText.text = "Hightime: NA";
		}
	}

	public void RestartLevel(){
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}
}
