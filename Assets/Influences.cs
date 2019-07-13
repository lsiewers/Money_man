using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Influences : MonoBehaviour
{
    public List<string> activeInfluences;
    private static Influences _instance;
    public static Influences Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void AddInfluence(GameObject obj)
    {
        if (activeInfluences.Contains(obj.name))
        {
            print(obj.name + " to be added");
            activeInfluences.Add(obj.name);
            Instantiate(obj, transform);
            obj.transform.localScale = new Vector3(30, 30, 0);
            obj.transform.localPosition = new Vector3(50 + -50 * transform.childCount, 0, 0);
            obj.transform.rotation = Quaternion.Euler(Vector3.forward * Random.Range(-25, 25));
            obj.transform.parent = GameManager.Instance.player.GetComponent<Accessoires>().top.transform;
        }
    }

    public void RemoveInfluence(string obj)
    {
        if (activeInfluences.Contains(obj))
        {
            print(obj + " removed");
            Destroy(transform.Find(obj + "(Clone)").gameObject);
        }
    }
}
