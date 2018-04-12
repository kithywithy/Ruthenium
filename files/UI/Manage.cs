﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manage : MonoBehaviour {

	[Header("Panels")]
	public GameObject managePanel;

	[Header("InputFields")]
	public InputField changeUsername;
	public InputField changePassword;
	public InputField userID;
	public InputField userPK;

	[Header("Text")]
	public Text warningText;

	[Header("Images")]
	public Image passwordStrength;

	// Use this for initialization
	void Start () {
		warningText.text = "";
		HideMenu ();
	}

	public void CheckPasswordStrength(){
		passwordStrength.fillAmount = changePassword.text.Length / 25f;

		if (changePassword.text.Contains ("password")) {
			passwordStrength.fillAmount -= 0.50f;
		}
		if (changePassword.text.Contains ("pass")) {
			passwordStrength.fillAmount -= 0.325f;
		}
		if (changePassword.text.Contains ("123")) {
			passwordStrength.fillAmount -= 0.25f;
		}
		if (changePassword.text.Contains ("321")) {
			passwordStrength.fillAmount -= 0.25f;
		}
		if (changePassword.text.Contains ("312")) {
			passwordStrength.fillAmount -= 0.125f;
		}
		if (changePassword.text.Contains ("213")) {
			passwordStrength.fillAmount -= 0.125f;
		}
		if (changePassword.text.Contains ("231")) {
			passwordStrength.fillAmount -= 0.125f;
		}
		if (changePassword.text.Contains ("132")) {
			passwordStrength.fillAmount -= 0.125f;
		}

		foreach(char c in changePassword.text){
			if (char.IsControl (c)) {
				passwordStrength.fillAmount += 0.125f;
			} else if (char.IsDigit (c)) {
				passwordStrength.fillAmount += 0.025f;
			} else if (char.IsHighSurrogate (c)) {
				passwordStrength.fillAmount += 0.05f;
			} else if (char.IsLowSurrogate (c)) {
				passwordStrength.fillAmount += 0.05f;
			} else if (char.IsSurrogate(c)){
				passwordStrength.fillAmount += 0.0325f;
			} else if (char.IsPunctuation(c)){
				passwordStrength.fillAmount += 0.075f;
			} else if (char.IsSeparator(c)){
				passwordStrength.fillAmount += 0.0325f;
			} else if (char.IsSymbol(c)){
				passwordStrength.fillAmount += 0.0225f;
			} else if (char.IsWhiteSpace(c)){
				passwordStrength.fillAmount += 0.025f;
			}
		}

		// COLORS
		if (passwordStrength.fillAmount > 0.5f) {
			passwordStrength.color = new Color (0f, 1f, 0f);
		} else if (passwordStrength.fillAmount >= 0.25f) {
			passwordStrength.color = new Color (1f,0.9f,0f);
		} else if (passwordStrength.fillAmount >= 0.125f) {
			passwordStrength.color = new Color (1f,0.65f,0f);
		} else {
			passwordStrength.color = new Color (1f, 0f, 0f);
		}
	}

	public void ShowMenu () {
		if(!GameController.current.permissions.Contains("settings.manage.self")){
			changeUsername.readOnly = true;
			changePassword.readOnly = true;
		}

		managePanel.SetActive (true);
		changeUsername.text = GameController.current.GetUsername();
		changePassword.text = GameController.current.GetPassword();
		userID.text = GameController.current.GetUserID ().ToString();
		userPK.text = (int.Parse(GameController.current.GetUserID ().ToString()) / System.DateTime.DaysInMonth(System.DateTime.Now.Year, System.DateTime.Now.Month)) + "";
	}
	public void HideMenu () {
		managePanel.SetActive (false);
		warningText.text = "";
		changeUsername.text = GameController.current.GetUsername();
		changePassword.text = GameController.current.GetPassword();
	}

	public void SaveChanges(){
		if(!GameController.current.permissions.Contains("settings.manage.self")){
			string log1 = "Manage::SaveChanges: Cannot make changes to the account \'" + GameController.current.GetUsername() + "\'. (insufficient permissions)";
			Logger.WriteLog (log1);
			GameController.nm.ShowNotification ("Insufficient permissions: settings.manage.self.");
			return;
		}

		string u = changeUsername.text;
		string p = changePassword.text;

		if (u == "" && p == "") {
			warningText.text = "Username and password are required.";
			return;
		} else if (u == "") {
			warningText.text = "Username is required.";
			return;
		} else if (p == "") {
			warningText.text = "Password is required.";
			return;
		}

		for (int i = 0; i < GameController.current.dataBase.Count; i++) {
			if (GameController.current.dataBase [i].Username == GameController.current.GetUsername ()) {
				GameController.current.dataBase [i].Username = u;
				GameController.current.dataBase [i].Password = p;
				GameController.current.SetUsername(u);
				GameController.current.SetPassword(p);
				GameController.sl.StartSave ();

				warningText.text = "";

				Debug.Log ("Manage::SaveChanges: Changed username/password for user \'<b>" + GameController.current.GetUsername () + "</b>\'.");
				Logger.WriteLog ("Manage::SaveChanges: Changed username/password for user \'" + GameController.current.GetUsername () + "\'.");
			}
		}
	}
}
