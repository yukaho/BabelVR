using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


//
//This script is only for inspector only, it has nothing to do with internal system.
//-Tamar
[CustomEditor(typeof(VRPlayerCore))]
public class VRPlayerCoreInspector : Editor
{
    //get reference of core script
    VRPlayerCore core;

    //ROI Visibility
    bool ROI_visbility_toggle = true;

    //toggle internal attribute
    bool internal_attribute = false;


    //Serialize
    SerializedProperty currentNode_info;
    public void OnEnable()
    {
        //get reference of core script
        core = (VRPlayerCore)target;

        //find properties from vr player core
        currentNode_info = serializedObject.FindProperty("currentNode_info");


    }

    
  

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //show ROI Settings
        GUIROISettings();

        //show scene node
        GUISceneNode();

        //show video in current node
        GUIVideo();

        //show Internal Attribute GUI
        GUIInternalAttribute();

    }



    public void GUIROISettings()
    {
        if (!Application.isPlaying)
            return;

        //RoI Setting
        EditorGUILayout.LabelField("ROI Settings", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("All roi will be sorted in descending order by score in roi list.", EditorStyles.label);

        //ROI Visibility Toggle
        ROI_visbility_toggle = EditorGUILayout.Toggle("ROI Visibility", ROI_visbility_toggle);
        core.setROIVisibility(ROI_visbility_toggle);

    }
    public void GUISceneNode()
    {
        if (!Application.isPlaying)
            return;

        //Current Scene Monitior
        EditorGUILayout.LabelField("Current Scene Node", EditorStyles.boldLabel);

        //create text bg
        Texture2D bg = new Texture2D(100, 15);
        FillTextureColor(bg, new Color(0.0f, 0.0f, 0.0f));

        //set new style for node
        GUIStyle node_custom = new GUIStyle();
        //set text color
        node_custom.normal.textColor = new Color(0, 0.8f, 0);
        node_custom.normal.background = bg;
        node_custom.fontStyle = FontStyle.Bold;

        //set new stye for shot
        GUIStyle shot_custom = new GUIStyle();
        shot_custom.normal.textColor = new Color(0, 0.8f, 0);


        //}

        //show label
        GUIContent content = new GUIContent(currentNode_info.stringValue);
        EditorGUILayout.LabelField(content, node_custom, GUILayout.Height(node_custom.CalcHeight(content, EditorGUIUtility.fieldWidth)));
    }
    public void GUIInternalAttribute()
    {
        if (!Application.isPlaying)
        {
            base.OnInspectorGUI();
            return;
        }

        //all internal attributes
        EditorGUILayout.LabelField("Internal Attributes", EditorStyles.boldLabel);

        //Description
        EditorGUILayout.LabelField("Showing internal linkings for object. Option For Developer Only");

        if (internal_attribute)
        {
            if (GUILayout.Button("Hide Internal Attribute"))
            {
                //Toggle
                if (internal_attribute)
                {
                    internal_attribute = false;
                }
                else
                {
                    internal_attribute = true;
                }
            }
            base.OnInspectorGUI();
        }
        else
        {
            if (GUILayout.Button("Show Internal Attribute"))
            {
                //Toggle
                if (internal_attribute)
                {
                    internal_attribute = false;
                }
                else
                {
                    internal_attribute = true;
                }
            }
        }



    }

    public void GUIVideo()
    {
        foreach (Transform v in core.VideoList)
        {

        }
    }
    public void FillTextureColor(Texture2D t, Color color)
    {
        //get pixels
        Color[] bg_colors = t.GetPixels();

        //fill with color
        for (int i = 0; i < bg_colors.Length; i++)
        {
            bg_colors[i].r = color.r;
            bg_colors[i].g = color.g;
            bg_colors[i].b = color.b;
        }

        //set pixels, write color
        t.SetPixels(bg_colors);
    }

}
