﻿using UnityEngine;
using System.Collections;
using UnityEditor;

public class Lux_WaterAccumulationDrawer : MaterialPropertyDrawer {


	override public void OnGUI (Rect position, MaterialProperty prop, string label, MaterialEditor editor)
	{

		// 	Needed since Unity 2019
		EditorGUIUtility.labelWidth = 0;

		Vector4 vec4value = prop.vectorValue;

		EditorGUI.BeginChangeCheck();
		GUILayout.Label("Water Accumulation in Cracks");
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(new GUIContent ("- Material Constant", "Lets you specify a 'constant' wetness per material."));
			vec4value.x = EditorGUILayout.Slider("", vec4value.x , 0.0f, 2.0f);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(new GUIContent ("- Global Influence", "Lets you adjust how fast water will accumulate according to the global script controlled water level.\nSet it to '0' in case you do not want any script driven water accumulation."));
			vec4value.y = EditorGUILayout.Slider("", vec4value.y , 0.0f, 4.0f);
		EditorGUILayout.EndHorizontal();

		GUILayout.Label("Water Accumulation in Puddles");
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(new GUIContent ("- Material Constant", "Lets you specify a 'constant' wetness per material."));
			vec4value.z = EditorGUILayout.Slider("", vec4value.z , 0.0f, 2.0f);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(new GUIContent ("- Global Influence", "Lets you adjust how fast water will accumulate according to the global script controlled water level.\nSet it to '0' in case you do not want any script driven water accumulation."));
			vec4value.w = EditorGUILayout.Slider("", vec4value.w , 0.0f, 4.0f);
		EditorGUILayout.EndHorizontal();

		if (EditorGUI.EndChangeCheck ()) {
			prop.vectorValue = vec4value;
		}
	}

	public override float GetPropertyHeight (MaterialProperty prop, string label, MaterialEditor editor)
	{
		return base.GetPropertyHeight (prop, label, editor) * 0.0f;
	}
}
