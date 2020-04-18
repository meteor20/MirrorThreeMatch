using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class demo : MonoBehaviour {

	string gameserver = "";

	const int left = 90;
	const int height = 50;
	const int top = 60;
	int width = Screen.width - left * 2;
	const int step = 60;
	
	void OnGUI() {
		
		int i = 0;
		GUI.Box(new Rect(10, 10, Screen.width - 20, Screen.height - 20), "Demo Menu");
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Set User")) {
			var abc = Application.internetReachability;
			TapDB.setUser("uid123456",TGTUserType.TGTTypeRegistered,TGTUserSex.TGTSexMale,18,"Bolt");
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Set Level")) {
			TapDB.setLevel(100);
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Set Server")) {
			TapDB.setServer("S100");
		}

		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Set Name")) {
			TapDB.setName("testName");
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Charge Request")) {
			TapDB.onChargeRequest("order01", "iap", 100, "CH", 10, "Alipay");
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Charge Success")) {
			TapDB.onChargeSuccess("order01");
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Charge Fail")) {
			TapDB.onChargeFail("order02", "fail reason");
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Charge Only Success")) {
			TapDB.onChargeOnlySuccess("order03", "iap", 100, "CH", 10, "Apple Pay");
		}

		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Charge Success new")) {
			TapDB.onChargeSuccess("order03", "iap", 100, "CH", "Apple Pay");
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "On Start")) {
			TapDB.onStart("mytest", "abc", "v1.0");
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "On Event")) {
			Dictionary<string, object> properties = new Dictionary<string, object>();
			properties.Add("abc", 123);
			properties.Add("def", "xyz");
			properties.Add("xyz", "中文");
			TapDB.onEvent("eventCode", properties);
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "On Event null")) {
			TapDB.onEvent("eventCode", null);
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Set Host")) {
			TapDB.setHost("https://e.tapdb.net/");
		}
		
		if (GUI.Button(new Rect(left, top + step * i++, width, height), "Set Custom Host")) {
			// TapDB.setCustomEventHost("https://ce.tapdb.net/");
			//test
			TapDB tapDB = new TapDB();
			//Debug.Log("taptapinstall:"+ tapDB.isTapTapInstall());
		}
	}
	
	void Start () {
		Debug.Log("start...!!!!!!!!!!");
		TapDB.onStart("mytest", "11111111111111", "v1.0");
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.Escape)) {
			Application.Quit();
		}
	}
	
	void OnApplicationPause( bool pauseStatus )
	{
		if(!pauseStatus){
			TapDB.onResume();
		} else {
			TapDB.onStop();
		}
		Debug.Log ("OnApplicationPause");
	}

	void OnApplicationQuit() {
		TapDB.onStop ();
		Debug.Log("OnApplicationQuit");
	}

	void OnDestroy (){
		Debug.Log("onDestroy");
	}
	
	void Awake () {
		Debug.Log("Awake");
	}
	
	void OnEnable () {
		Debug.Log("OnEnable");
	}
	
	void OnDisable () {
		Debug.Log("OnDisable");
	}
}
