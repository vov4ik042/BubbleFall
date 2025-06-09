using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private float velocity = 0.0015f;
	private int indexColor;
	private int currentRow;
	private int indexRow;
	private int indexColumn;
    private bool goingDown = true;
    private Vector3 leftSide;
    public int IndexColor
	{
		get { return indexColor; }
		set { indexColor = value; }
	}
    private void Awake()
    {
        leftSide = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
    }
    private void FixedUpdate()
    {
		UpdatePos();
    }

    private void UpdatePos()
	{
        if (goingDown)
        {
		    Vector3 pos = gameObject.transform.position;
		    pos.y -= velocity;
            gameObject.transform.position = pos;
        }
        if (transform.position.y < leftSide.y)
        {
            ReleaseFromScene();
        }
	}
	public void SetRow(int value)
	{
		currentRow = value;
	}
    public int GetRow()
    {
		return currentRow;
    }
    public void SetRowAndColumnIndex(int row, int column)
    {
        //Debug.Log("SetRowAndColumnIndex row " + row + " column: " + column);
        indexRow = row;
        indexColumn = column;
    }
    public int GetRowIndex()
    {
        return indexRow;
    }
    public int GetColumnIndex()
    {
        return indexColumn;
    }
    public void SetGoingDown(bool waiting)
    {
        goingDown = waiting;
    }
    public bool GetGoingDown()
    {
        return goingDown;
    }
    public void ReleaseFromScene()
    {
        goingDown = false;
        gameObject.SetActive(false);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        //GameController.Instance.PutNextPlayerBall();
    }
}
