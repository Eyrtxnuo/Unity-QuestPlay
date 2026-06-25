using UnityEngine;

public class ThumbsUpPoseTest : MonoBehaviour
{ 
    public GameObject colorChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorChange.GetComponent<Renderer>().material.color = Color.blue;
    }

    public void onPoseStart()
    {
        colorChange.GetComponent<Renderer>().material.color = Color.green;
    }

    public void onPoseEnd()
    {
        colorChange.GetComponent<Renderer>().material.color = Color.red;
    }
}
