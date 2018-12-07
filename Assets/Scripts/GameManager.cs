using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	private static string PREFS_TIME = "TIME";
	private static bool gameOn = false;

	[SerializeField] Text hightimeText;
	[SerializeField] Text countdownText;
	[SerializeField] Text timerText;

	private static float timer = 0f;

	// Use this for initialization
	void Start () {
		hightimeText.text = "Hightime: "+PlayerPrefs.GetFloat(PREFS_TIME).ToString("F2").Replace(",", ".")+"s";
		StartCoroutine(Countdown());
	}
	
	// Update is called once per frame
	void Update () {
		if(gameOn){
			timer += Time.deltaTime;
			timerText.text = timer.ToString("F2").Replace(",", ".")+"s";
		}
	}

	IEnumerator Countdown(){
		for(int i=3; i>0; i--){
			countdownText.text = i.ToString();
			yield return new WaitForSeconds(1);
		}
		gameOn = true;
		countdownText.text = "GO";
		yield return new WaitForSeconds(1);
		countdownText.text = "";
	}

	public static bool GameOn(){
		return gameOn;
	}

	public static void CrossedFinishLine(){
		gameOn = false;
		Debug.Log("Time: "+timer+" Hightime: "+PlayerPrefs.GetFloat(PREFS_TIME));
		if (timer < PlayerPrefs.GetFloat(PREFS_TIME))
        {
            PlayerPrefs.SetFloat(PREFS_TIME, timer);
		}
	}
}
