using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int poolSize = 21;

    private Stack<GameObject> pool = new Stack<GameObject>(); 

    public static SkullObjectPool instance;


    [SerializeField] private Transform skullParent;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab, skullParent);
            obj.SetActive(false);
            pool.Push(obj);
        }
    }

    public GameObject GetObject()
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Pop();
        }
        else
        {
            obj = Instantiate(prefab);
        }
        obj.SetActive(true);
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Push(obj);
    }

    public void ReturnAllObjects() 
    {
        for (int i = skullParent.childCount - 1; i >= 0; i--)
        {
            ReturnObject(skullParent.GetChild(i).gameObject);
        }
    }
}
