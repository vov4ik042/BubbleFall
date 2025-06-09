using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Camera cam;
    private Vector3 bottomLeft;
    private Vector3 topRight;
    private void Start()
    {
        cam = Camera.main;
        lineRenderer = GetComponent<LineRenderer>();
        bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        topRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        //Debug.Log("bottomLeft: " + bottomLeft);
        //Debug.Log("topRight: " + topRight);
    }

    public void ShowTrajectory(Vector3 startPos, Vector3 speed)
    {
        float timeStep = 0.1f;
        Vector3[] points = new Vector3[2];
        lineRenderer.positionCount = points.Length;

        for (int i = 0; i < points.Length; i++)
        {
            float time = i * timeStep;
            Vector3 point = startPos + speed * time;
            point.z = -0.2f;
            points[i] = point;

            /*if (point.x < bottomLeft.x || point.x > topRight.x)
            {
                point.x *= -1f;
                break;
            }*/

            if (point.y < bottomLeft.y || point.y > topRight.y)
            {
                lineRenderer.positionCount = i + 1;
                break;
            }
        }

        lineRenderer.SetPositions(points);
    }

    public void HideTrajectory()
    {
        lineRenderer.positionCount = 0;
    }
}
