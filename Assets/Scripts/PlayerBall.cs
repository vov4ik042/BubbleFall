using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerBall : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private TrajectoryRenderer trajectory;
    private Rigidbody rigidbody;

    private bool hasCollided = false;
    private bool pressed = false;
    private bool waiting = true;
    private float power = 10.0f;
    private bool AddedToGraph = false;

    private void Awake()
    {
        trajectory = GetComponent<TrajectoryRenderer>();
        rigidbody = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (pressed && waiting == false && AddedToGraph == false)
        {
            trajectory.ShowTrajectory(transform.position, Calculate());
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == gameObject && waiting == false && AddedToGraph == false)
        {
            pressed = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (waiting == false && AddedToGraph == false)
        {
            pressed = false;
            rigidbody.isKinematic = false;
            trajectory.HideTrajectory();
            Rigidbody ball = gameObject.GetComponent<Rigidbody>();
            ball.AddForce(Calculate(), ForceMode.Impulse);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;

        if (collision.gameObject.CompareTag("Wall"))
        {
            Rigidbody rigidbody = GetComponent<Rigidbody>();

            rigidbody.AddForce(Calculate(true), ForceMode.Impulse);
        }
        else
        {
            if (!collision.gameObject.CompareTag("Line"))
            {
                Debug.Log("Tick");

                Ball ballCollision = collision.gameObject.GetComponent<Ball>();
                Ball ball = gameObject.GetComponent<Ball>();

                ball.SetGoingDown(true);

                GameController.Instance.PutNextPlayerBall();

                //Debug.Log("ballCollision.GetRowIndex():" + ballCollision.GetRowIndex());
                //Debug.Log("ballCollision.GetColumnIndex():" + ballCollision.GetColumnIndex());

                int[] cellsFreeAround = GameController.Instance.GetFreeSlotsAroundBall(ballCollision.GetRowIndex(), ballCollision.GetColumnIndex());
                CalculatePositionForAddingBall(collision.gameObject.transform.position, cellsFreeAround, ballCollision, ball);

                GameController.Instance.FindConnectedSameValues(ball.GetRowIndex(), ball.GetColumnIndex());

                AddedToGraph = true;
                hasCollided = true;
            }
        }
    }
    public void ReleaseFromScene()
    {
        hasCollided = false;
        waiting = false;
        AddedToGraph = false;
    }

    private void CalculatePositionForAddingBall(Vector3 ballPos, int[] mas, Ball ballCollider, Ball ball)
    {
        int rowBall = ballCollider.GetRowIndex();
        int columnBall = ballCollider.GetColumnIndex();
        int indexColorBall = ball.IndexColor;

        Vector3 posBall = transform.position;

        Debug.Log($"mas: {mas[0]} {mas[1]} {mas[2]}");

        List<Vector3> possiblePositions = new List<Vector3>();
        List<int> directions = new List<int>(); // Чтобы запомнить, откуда позиция взялась

        if (mas[0] == -1)
        {
            possiblePositions.Add(new Vector3(ballPos.x - 0.5f, ballPos.y, ballPos.z));
            directions.Add(0); // слева
        }
        if (mas[1] == -1)
        {
            possiblePositions.Add(new Vector3(ballPos.x + 0.5f, ballPos.y, ballPos.z));
            directions.Add(1); // справа
        }
        if (mas[2] == -1)
        {
            possiblePositions.Add(new Vector3(ballPos.x, ballPos.y - 0.5f, ballPos.z));
            directions.Add(2); // снизу
        }

        if (possiblePositions.Count > 0)
        {
            Audio.Instance.PlaySFX(1);

            int closestIndex = 0;
            float minDistance = Vector3.Distance(posBall, possiblePositions[0]);

            for (int i = 1; i < possiblePositions.Count; i++)
            {
                float distance = Vector3.Distance(posBall, possiblePositions[i]);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestIndex = i;
                }
            }

            Vector3 closestPosition = possiblePositions[closestIndex];
            int masIndexUsed = directions[closestIndex];

            gameObject.transform.position = closestPosition;

            //Debug.Log("Ближайшая позиция: " + closestPosition);
            Debug.Log("Использованный индекс в mas: " + masIndexUsed); // 0 — слева, 1 — справа, 2 — снизу

            switch(masIndexUsed)
            {
                case 0:
                    {
                        Debug.Log("new1: " + rowBall + (columnBall - 1));
                        GameController.Instance.SetBalltoGraph(rowBall, columnBall - 1, indexColorBall, ball);
                        gameObject.GetComponent<Ball>().SetRowAndColumnIndex(rowBall, columnBall - 1);
                        break;
                    }
                case 1:
                    {
                        Debug.Log("new2: " + rowBall + (columnBall + 1));
                        GameController.Instance.SetBalltoGraph(rowBall, columnBall + 1, indexColorBall, ball);
                        gameObject.GetComponent<Ball>().SetRowAndColumnIndex(rowBall, columnBall + 1);
                        break;
                    }
                case 2:
                    {
                        Debug.Log("new3: " + (rowBall + 1) + columnBall);
                        GameController.Instance.SetBalltoGraph(rowBall + 1, columnBall, indexColorBall, ball);
                        gameObject.GetComponent<Ball>().SetRowAndColumnIndex(rowBall + 1, columnBall);
                        break;
                    }
            }
        }
        else
        {
            Debug.LogWarning("Нет допустимых позиций для добавления шара.");
        }

        rigidbody.isKinematic = true;
    }



    public void SetWaiting(bool waiting)
    {
        this.waiting = waiting;
    }
    public bool GetWaiting()
    {
        return waiting;
    }
    private Vector3 Calculate(bool inverse = false)
    {
        Vector3 fingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        fingerPos.z = -0.2f;

        if (inverse == true)
        {
            fingerPos.x *= -1.0f;
        }

        Vector3 direction = (fingerPos - transform.position).normalized;
        return direction * power;
    }
}
