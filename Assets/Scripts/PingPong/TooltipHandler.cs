using System;
using UnityEngine;
using UnityEngine.Serialization;

public partial class TooltipHandler : MonoBehaviour
{
    [SerializeField] private GameObject tooltipContinue;
    [SerializeField] private GameObject tooltipToss;

    

    private void Awake()
    {
        tooltipContinue.SetActive(false);
        tooltipToss.SetActive(false);
    }

    public  void ShowContinue(bool show = true)
    {
        tooltipContinue.SetActive(show);
    }
    
    public void ShowToss(bool show = true)
    {
        tooltipToss.SetActive(show);
    }
}
