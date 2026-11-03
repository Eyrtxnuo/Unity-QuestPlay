using UnityEngine;

public class alternateChild : MonoBehaviour
{
    public float delay = 1f;
        
    private int _index = 0;
    private float _timer = 0f;
    
    private GameObject _currentlyActive;

    void Start()
    {
        if (this.transform.childCount == 0) return;
        //disabilita tutti i gameobject diversi da _index
        for (int i = 0; i < transform.childCount; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(i == _index);
        } 
        
        _currentlyActive = transform.GetChild(_index).gameObject;
    }

    void Update()
    {
        _timer  += Time.deltaTime;
        if (_timer < delay)return;
        if (this.transform.childCount <= 1) return;
        _timer -= delay;
        _index = (_index + 1) % transform.childCount;
        _currentlyActive.SetActive(false);
        _currentlyActive = this.transform.GetChild(_index).gameObject;
        _currentlyActive.SetActive(true);
    }
}
