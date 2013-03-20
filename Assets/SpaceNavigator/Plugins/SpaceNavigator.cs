﻿using System;
using UnityEngine;

public abstract class SpaceNavigator : IDisposable {
	// Public API
	public static Vector3 Translation {
		get { return Instance.GetTranslation(); }
	}
	public static Quaternion Rotation {
		get { return Instance.GetRotation(); }
	}
	public static Quaternion RotationInLocalCoordSys(Transform coordSys) {
		return coordSys.rotation * Rotation * Quaternion.Inverse(coordSys.rotation);
	}
	public static bool LockTranslationX, LockTranslationY, LockTranslationZ, LockTranslationAll;
	public static bool LockRotationX, LockRotationY, LockRotationZ, LockRotationAll;

	// Abstract members
	public abstract Vector3 GetTranslation();
	public abstract Quaternion GetRotation();

	// Sensitivity settings
	public const float TransSensScale = 0.001f, RotSensScale = 0.05f;
	public const float TransSensDefault = 10f, TransSensMinDefault = 0.001f, TransSensMaxDefault = 50f;
	public const float RotSensDefault = 1, RotSensMinDefault = 0.001f, RotSensMaxDefault = 5f;
	public float TransSens = TransSensDefault, TransSensMin = TransSensMinDefault, TransSensMax = TransSensMaxDefault;
	public float RotSens = RotSensDefault, RotSensMin = RotSensMinDefault, RotSensMax = RotSensMaxDefault;

	// Setting storage keys
	private const string TransSensKey = "Translation sensitivity";
	private const string TransSensMinKey = "Translation sensitivity minimum";
	private const string TransSensMaxKey = "Translation sensitivity maximum";
	private const string RotSensKey = "Rotation sensitivity";
	private const string RotSensMinKey = "Rotation sensitivity minimum";
	private const string RotSensMaxKey = "Rotation sensitivity maximum";

	#region - Singleton -
	public static SpaceNavigator Instance {
		get {
			if (_instance == null) {
				switch (Application.platform) {
					case RuntimePlatform.OSXEditor:
					case RuntimePlatform.OSXPlayer:
						Debug.LogError("Mac version of the SpaceNavigator driver is not yet implemented, sorry");
						_instance = SpaceNavigatorNoDevice.SubInstance;
						break;
					case RuntimePlatform.WindowsEditor:
					case RuntimePlatform.WindowsPlayer:
						_instance = SpaceNavigatorWindows.SubInstance;
						break;
				}
			}

			return _instance;
		}
		set { _instance = value; }
	}
	private static SpaceNavigator _instance;
	#endregion - Singleton -

	#region - IDisposable -
	public abstract void Dispose();
	#endregion - IDisposable -

	public virtual void OnGUI() {
		GUILayout.Space(10);
		GUILayout.Label("Lock");
		GUILayout.Space(4);

		GUILayout.BeginHorizontal();
		LockTranslationAll = GUILayout.Toggle(LockTranslationAll, "Translation\t");
		GUI.enabled = !LockTranslationAll;
		LockTranslationX = GUILayout.Toggle(LockTranslationX, "X");
		LockTranslationY = GUILayout.Toggle(LockTranslationY, "Y");
		LockTranslationZ = GUILayout.Toggle(LockTranslationZ, "Z");
		GUI.enabled = true;
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		LockRotationAll = GUILayout.Toggle(LockRotationAll, "Rotation\t\t");
		GUI.enabled = !LockRotationAll;
		LockRotationX = GUILayout.Toggle(LockRotationX, "X");
		LockRotationY = GUILayout.Toggle(LockRotationY, "Y");
		LockRotationZ = GUILayout.Toggle(LockRotationZ, "Z");
		GUI.enabled = true;
		GUILayout.EndHorizontal();

		GUILayout.Space(10);
		GUILayout.Label("Sensitivity");
		GUILayout.Space(4);

		string input;
		float newValue;

		GUILayout.BeginHorizontal();
		GUILayout.Label(String.Format("Translation\t {0:0.00000}", TransSens));
		#region Textfield minimum
		input = GUILayout.TextField(TransSensMin.ToString());
		if (float.TryParse(input, out newValue))
			TransSensMin = newValue;
		#endregion Textfield minimum
		TransSens = GUILayout.HorizontalSlider(TransSens, TransSensMin, TransSensMax);
		#region Textfield maximum
		input = GUILayout.TextField(TransSensMax.ToString());
		if (float.TryParse(input, out newValue))
			TransSensMax = newValue;
		#endregion Textfield maximum
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label(String.Format("Rotation\t\t {0:0.00000}", RotSens));
		#region Textfield minimum
		input = GUILayout.TextField(RotSensMin.ToString());
		if (float.TryParse(input, out newValue))
			RotSensMin = newValue;
		#endregion Textfield minimum
		RotSens = GUILayout.HorizontalSlider(RotSens, RotSensMin, 5f);
		#region Textfield maximum
		input = GUILayout.TextField(RotSensMax.ToString());
		if (float.TryParse(input, out newValue))
			RotSensMax = newValue;
		#endregion Textfield maximum
		GUILayout.EndHorizontal();
	}

	#region - Settings -
	/// <summary>
	/// Reads the settings.
	/// </summary>
	public void ReadSettings() {
		TransSens = PlayerPrefs.GetFloat(TransSensKey, TransSensDefault);
		TransSensMin = PlayerPrefs.GetFloat(TransSensMinKey, TransSensMinDefault);
		TransSensMax = PlayerPrefs.GetFloat(TransSensMaxKey, TransSensMaxDefault);

		RotSens = PlayerPrefs.GetFloat(RotSensKey, RotSensDefault);
		RotSensMin = PlayerPrefs.GetFloat(RotSensMinKey, RotSensMinDefault);
		RotSensMax = PlayerPrefs.GetFloat(RotSensMaxKey, RotSensMaxDefault);
	}
	/// <summary>
	/// Writes the settings.
	/// </summary>
	public void WriteSettings() {
		PlayerPrefs.SetFloat(TransSensKey, TransSens);
		PlayerPrefs.SetFloat(TransSensMinKey, TransSensMin);
		PlayerPrefs.SetFloat(TransSensMaxKey, TransSensMax);

		PlayerPrefs.SetFloat(RotSensKey, RotSens);
		PlayerPrefs.SetFloat(RotSensMinKey, RotSensMin);
		PlayerPrefs.SetFloat(RotSensMaxKey, RotSensMax);
	}
	#endregion - Settings -
}
