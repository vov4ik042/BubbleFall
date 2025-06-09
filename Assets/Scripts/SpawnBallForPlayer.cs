using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallForPlayer : MonoBehaviour
{
    [SerializeField] private GameObject ball;

    public static SpawnBallForPlayer Instance;
    private List<GameObject> listPrefabs = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public void CreateObjectsInPool(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject gameObject = Instantiate(ball, this.gameObject.transform);
            gameObject.SetActive(false);
            listPrefabs.Add(gameObject);
        }
    }
    public GameObject GetPoolObject()
    {
        for (int i = 0; i < listPrefabs.Count; i++)
        {
            if (!listPrefabs[i].activeInHierarchy)
            {
                return listPrefabs[i];
            }
        }
        return null;
    }
    public GameObject GetPlayerFromQueryObject()
    {
        for (int i = 0; i < listPrefabs.Count; i++)
        {
            if (listPrefabs[i].GetComponent<PlayerBall>().GetWaiting() == true)
            {
                return listPrefabs[i];
            }
        }
        return null;
    }
}
