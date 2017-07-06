using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace OMobile.EstimoteUnity
{
	[CustomEditor (typeof(EstimoteUnity))]
	public class EstimoteUnityInspector : Editor
	{

		#region Private Variables

		// GUI
		private GUIStyle mTitleStyle = null;
		private GUIStyle mSubTitleStyle = null;

		// Properties
		private SerializedProperty mBeaconsUUIDProperty;
		private SerializedProperty mScanPeriodProperty;
		private SerializedProperty mWaitTimeProperty;

		#endregion


		#region Unity Methods

		void OnEnable ()
		{
			mBeaconsUUIDProperty = this.serializedObject.FindProperty ("BeaconsUUID");
			mScanPeriodProperty = this.serializedObject.FindProperty ("ScanPeriodMillis");
			mWaitTimeProperty = this.serializedObject.FindProperty ("WaitTimeMillis");
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			EditorGUI.BeginChangeCheck ();

			DrawHeading ();

			EditorGUILayout.Space ();

			CheckIfPrefab ();

			EditorGUILayout.Space ();

			DrawProperties ();

			EditorGUILayout.Space ();

			DrawWarnings ();

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();

			DrawAbout ();

			EditorGUILayout.Space ();

			serializedObject.ApplyModifiedProperties ();

			if (EditorGUI.EndChangeCheck ()) {
				EditorSceneManager.MarkSceneDirty (EditorSceneManager.GetActiveScene ());
			}
		}

		#endregion

		#region Private Methods

		private void DrawHeading ()
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label ("Estimote Unity", GetTitleStyle ());
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label ("by Oakley Mobile Ltd.", GetSubTitleStyle ());
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
		}

		private void CheckIfPrefab ()
		{
			if (PrefabUtility.GetPrefabParent (((EstimoteUnity)target).gameObject) != null && PrefabUtility.GetPrefabObject (((EstimoteUnity)target).gameObject) != null) {
				GUI.color = Color.red;
				EditorGUILayout.HelpBox ("This GameObject cannot be a prefab. Please disconnect the prefab instance or some of your settings will not be saved properly.", MessageType.Error);
				GUI.color = Color.white;
			}
		}

		private void DrawProperties ()
		{
			EditorGUILayout.LabelField ("Properties", EditorStyles.boldLabel);

			EditorGUILayout.HelpBox ("This is the Proximity UUID for the beacons you wish to detect." +
			"\nNOTE: Currently only a single Proximity UUID is supported.", MessageType.Info);
			EditorGUILayout.PropertyField (mBeaconsUUIDProperty, true);

			EditorGUILayout.Space ();

			EditorGUILayout.HelpBox ("These values change the scan period and wait times (in millis) for scanning BLE Beacons. Android only.", MessageType.Info);
			EditorGUILayout.PropertyField (mScanPeriodProperty, new GUIContent ("Scan Period (millis)"), true);
			if (mScanPeriodProperty.longValue < 200) {
				mScanPeriodProperty.longValue = 200;
			}
			EditorGUILayout.PropertyField (mWaitTimeProperty, new GUIContent ("Wait Time (millis)"), true);
			if (mWaitTimeProperty.longValue < 0) {
				mWaitTimeProperty.longValue = 0;
			}
				
			EditorUtility.SetDirty ((EstimoteUnity)target);

			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		}

		private void DrawWarnings ()
		{
			EditorGUILayout.LabelField ("Warnings", EditorStyles.boldLabel);

			// Check Android Status
			if (!EstimoteUnityEditorUtils.CheckAndroidStatus ()) {
				EditorGUILayout.Space ();
				GUI.color = Color.red;
				EditorGUILayout.HelpBox ("Android setup is not complete!" +
				" To complete the setup go to Window/O-Mobile/Estimote Unity/Setup and follow the instructions.", MessageType.Info);
				GUI.color = Color.white;
			}

			// Check iOS Status
			if (!EstimoteUnityEditorUtils.CheckIOSStatus ()) {
				EditorGUILayout.Space ();
				GUI.color = Color.red;
				EditorGUILayout.HelpBox ("iOS setup is not complete!" +
				" To complete the setup go to Window/O-Mobile/Estimote Unity/Setup and follow the instructions.", MessageType.Info);
				GUI.color = Color.white;
			}

			// Show Button
			if (!EstimoteUnityEditorUtils.CheckAndroidStatus () || !EstimoteUnityEditorUtils.CheckIOSStatus ()) {
				EditorGUILayout.Space ();
				if (GUILayout.Button ("Open Estimote Unity Setup")) {
					EstimoteUnityEditorSetup.OpenWindow ();
				}
			}
		}

		private void DrawAbout ()
		{
			// Estimotes Developer Portal
			if (GUILayout.Button ("Visit Estimote Developer Portal")) {
				Application.OpenURL ("http://developer.estimote.com/");
			}

			EditorGUILayout.Space ();

			// O-Mobile's website
			if (GUILayout.Button ("Visit Oakley Mobile's Website")) {
				Application.OpenURL ("http://www.o-mobile.co.uk/");
			}
		}

		private GUIStyle GetTitleStyle ()
		{
			if (mTitleStyle == null) {
				mTitleStyle = new GUIStyle (EditorStyles.largeLabel);
				mTitleStyle.fontStyle = FontStyle.Bold;
				mTitleStyle.fontSize = 20;
			}
			return mTitleStyle;
		}

		private GUIStyle GetSubTitleStyle ()
		{
			if (mSubTitleStyle == null) {
				mSubTitleStyle = new GUIStyle (EditorStyles.largeLabel);
			}
			return mSubTitleStyle;
		}

		#endregion

	}
}