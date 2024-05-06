// Ce script code l'affichage du Script DronesUDP dans la fen�tre inspector de Unity

using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using Polytech.CoHoMa.Scene;

[CustomEditor(typeof(DronesUDP))]
public class DronesUDPEditor : Editor
{
    SerializedProperty useMaterialProp;
    SerializedProperty displayMaterialProp;
    SerializedProperty displayRawImageProp;
    SerializedProperty IpInputFieldProp;
    SerializedProperty A_buttonProp;
    SerializedProperty B_buttonProp;
    SerializedProperty Grip_buttonPropL;
    SerializedProperty Grip_buttonPropR;

    void OnEnable()
    {
        // Reliez vos propri�t�s ici
        useMaterialProp = serializedObject.FindProperty("useMaterial");
        displayMaterialProp = serializedObject.FindProperty("displayMaterial");
        displayRawImageProp = serializedObject.FindProperty("displayRawImage");
        IpInputFieldProp = serializedObject.FindProperty("IpInputField");
        A_buttonProp = serializedObject.FindProperty("A_button");
        B_buttonProp = serializedObject.FindProperty("B_button");
        Grip_buttonPropL = serializedObject.FindProperty("Grip_buttonL");
        Grip_buttonPropR = serializedObject.FindProperty("Grip_buttonR");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update(); // Mise � jour de l'objet s�rialis�

        EditorGUILayout.PropertyField(useMaterialProp); // Toujours afficher useMaterial

        // Afficher conditionnellement displayMaterial ou displayRawImage
        if (useMaterialProp.boolValue)
        {
            EditorGUILayout.PropertyField(displayMaterialProp, new GUIContent("Display Material"));
        }
        else
        {
            EditorGUILayout.PropertyField(displayRawImageProp, new GUIContent("Display Raw Image"));
        }

        EditorGUILayout.PropertyField(IpInputFieldProp, new GUIContent("Ip InputField")); // Toujours afficher IpInputField
        EditorGUILayout.PropertyField(A_buttonProp, new GUIContent("A_button"));
        EditorGUILayout.PropertyField(B_buttonProp, new GUIContent("B_button"));
        EditorGUILayout.PropertyField(Grip_buttonPropL, new GUIContent("Grip_buttonL"));
        EditorGUILayout.PropertyField(Grip_buttonPropR, new GUIContent("Grip_buttonR"));

        serializedObject.ApplyModifiedProperties(); // Appliquer les modifications
    }
}
