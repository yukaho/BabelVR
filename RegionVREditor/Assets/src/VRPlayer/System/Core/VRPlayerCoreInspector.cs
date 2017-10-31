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


    //Serialized properties
    SerializedProperty currentNode_info;


    SerializedProperty currentNode_lastSeenROI_info;
    SerializedProperty currentNode_firstSeenROI_info;
    SerializedProperty currentNode_info_roi_score_list;

    SerializedProperty currentNode_endAction;
    SerializedProperty currentNode_currentframes;
    public void OnEnable()
    {
        //get reference of core script
        core = (VRPlayerCore)target;

        //find properties from vr player core
        currentNode_info = serializedObject.FindProperty("currentNode_info");

        //ROI Properties
        currentNode_lastSeenROI_info = serializedObject.FindProperty("currentNode_info_lastSeen_roi");
        currentNode_firstSeenROI_info = serializedObject.FindProperty("currentNode_info_firstSeen_roi");
        currentNode_info_roi_score_list = serializedObject.FindProperty("currentNode_info_roi_score_list");

        //current frames
        currentNode_currentframes = serializedObject.FindProperty("currentNode_currentframes");
        currentNode_endAction = serializedObject.FindProperty("currentNode_endAction");
    }




    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //show scene node
        GUISceneNode();

        //show ROI Settings
        GUIROISettings();

        //show ROI Info
        GUIROI();

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

    public void GUIROI()
    {
        if (!Application.isPlaying)
            return;


        EditorGUILayout.LabelField("Passive ROIs Information", EditorStyles.boldLabel);

        //create text bg
        Texture2D bg = new Texture2D(100, 15);
        FillTextureColor(bg, new Color(0.0f, 0.0f, 0.0f));

        //set new style for node
        GUIStyle roi_display = new GUIStyle();
        //set text color
        roi_display.normal.textColor = new Color(0, 0.8f, 0);
        roi_display.normal.background = bg;
        roi_display.fontStyle = FontStyle.Bold;

        //show label
        GUIContent content = new GUIContent(
            "Last Seen ROI: " + currentNode_lastSeenROI_info.stringValue + "\n" +
            "First Seen ROI: " + currentNode_firstSeenROI_info.stringValue + "\n"+ 
            currentNode_info_roi_score_list.stringValue
           );
        EditorGUILayout.LabelField(content, roi_display, GUILayout.Height(roi_display.CalcHeight(content, EditorGUIUtility.fieldWidth)));
    }
    public void GUISceneNode()
    {
        if (!Application.isPlaying)
            return;

        //create text bg
        Texture2D bg = new Texture2D(100, 15);
        FillTextureColor(bg, new Color(0.0f, 0.0f, 0.0f));

        //Current Scene Monitior
        EditorGUILayout.LabelField("Current Scene Node", EditorStyles.boldLabel);

        GUIStyle end_action_style = new GUIStyle();
        end_action_style.normal.textColor = new Color(0, 0.8f, 0);
        end_action_style.normal.background = bg;

        EditorGUILayout.LabelField("End Action : ", currentNode_endAction.stringValue, end_action_style);
        //Movie Slider
        EditorGUILayout.Slider(new GUIContent("Video Timecode(frames) :"),currentNode_currentframes.intValue, 0, core.current_node.total_frames);

   

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
