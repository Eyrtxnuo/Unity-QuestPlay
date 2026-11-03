using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PanelPhasesHandler : MonoBehaviour
{
    public List<PanelPhase> phases = new();
    public Toggle buttonToggle;
    public TextMeshProUGUI buttonLabel;
    
    private int _phaseIndex = 0;
    
    void Start()
    {
        EndPhase();
        LoadPhase(_phaseIndex);
    }

    void EndPhase()
    {
        if(transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
        if (_phaseIndex >= phases.Count)
        {
            foreach (GameObject disable in phases[_phaseIndex].enable)
            {
                disable.SetActive(false);
            }
        }
    }

    void LoadPhase(int index)
    {
        EndPhase();
        if (index >= phases.Count) return;
        _phaseIndex = index;
        var phase = phases[_phaseIndex];
        foreach (GameObject enable in phase.enable)
        {
            enable.SetActive(true);
        }

        buttonToggle.onValueChanged = phase.action;
        buttonLabel.SetText(phase.buttonText);
    }
    
    public void NextPhase()
    {
        LoadPhase(_phaseIndex+1);
    }
    
    [Serializable]
    public struct PanelPhase
    {
        public GameObject[] enable;
        public string buttonText;
        public Toggle.ToggleEvent action;
    }
}
