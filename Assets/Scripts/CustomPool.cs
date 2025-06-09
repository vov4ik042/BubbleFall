using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class CustomPool : MonoBehaviour
{
    [SerializeField] private GameObject prefaBall;

    public static CustomPool Instance;
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
            GameObject gameObject = Instantiate(prefaBall, this.gameObject.transform);
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
    public void SetStopOrGoAllBalls(bool value)
    {
        for (int i = 0; i < listPrefabs.Count; i++)
        {
            if (listPrefabs[i].activeInHierarchy)
            {
                listPrefabs[i].GetComponent<Ball>().SetGoingDown(value);
            }
        }
    }
}
