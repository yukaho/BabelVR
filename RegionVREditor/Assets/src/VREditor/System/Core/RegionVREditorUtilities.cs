using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RegionVREditorUtilities
{
    public static Vector2 ScreenPointToViewPoint(GameObject view, Vector2 point)
    {

        //4 points of corners
        Vector3[] corners = new Vector3[4];
        view.GetComponent<RectTransform>().GetWorldCorners(corners);
        Rect newRect = new Rect(corners[0], corners[2] - corners[0]);
        return new Vector2(point.x - newRect.xMin, newRect.yMax - point.y);

    }

    public static Vector2 ScreenPointToVirtualViewPoint(GameObject view, GameObject virtual_view, Vector3 point)
    {

        //4 points of corners
        Vector3[] corners = new Vector3[4];
        view.GetComponent<RectTransform>().GetWorldCorners(corners);
        Rect newRect = new Rect(corners[0], corners[2] - corners[0]);
        Vector2 world_rect_length = new Vector2(Math.Abs(corners[2].x- corners[0].x), Math.Abs(corners[2].y - corners[0].y));
        Vector2 view_world_point = new Vector2(point.x - newRect.xMin, newRect.yMax - point.y);
        Vector2 raito = new Vector2(view_world_point.x/ world_rect_length.x, view_world_point.y / world_rect_length.y);
        Vector2 delta_size = view.GetComponent<RectTransform>().sizeDelta;
        Vector2 view_real_point = new Vector2(raito.x * delta_size.x, raito.y * delta_size.y);
        view_real_point.y *= -1;
        view_real_point.x -= virtual_view.transform.GetChild(0).position.x;
        view_real_point.x -= (view.GetComponent<RectTransform>().sizeDelta.x / 2);
        view_real_point.y -= virtual_view.transform.GetChild(0).position.y;
        view_real_point.y += (view.GetComponent<RectTransform>().sizeDelta.y / 2);

        return view_real_point;

    }
}

