using System.Collections.Generic;
using UnityEngine;

public class CycleAndChangeWorld : MonoBehaviour
{
    public float timeForEach = 2f;
    public string worldName;
    
    private float startTime;
    private int idx;
    private List<GameObject> items;
    
    void Start()
    {
        startTime = Time.time;
        items = new List<GameObject>();
        foreach (Transform child in this.transform)
        {
            items.Add(child.gameObject);
        }
        items[idx].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - startTime > timeForEach)
        {
            if (idx == items.Count -1)
            {
                startTime = int.MaxValue;
                //go
                
                StartCoroutine(MainMenu.LoadSceneAsyncRoutine(worldName));
                return;
            }
            startTime = Time.time;
            items[idx].SetActive(false);
            idx++;
            items[idx].SetActive(true);
            
        }
    }
}
