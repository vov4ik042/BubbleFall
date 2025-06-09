using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineTriger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.gameObject.GetComponent<Ball>();
        if (ball != null && ball.GetGoingDown() == true)
        {
            ResultWindow.Instance.Show(GameController.Instance.GetScore(), false);
        }
    }
}
