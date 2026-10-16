using System;
using System.Collections;
using PingPong;
using TMPro;
using UnityEngine;
namespace Firing
{
    [RequireComponent(typeof(TextMeshPro))]
    public class FiringPointLabel : MonoBehaviour
    {
        
        private static readonly Color EarnColor = new Color32(50, 150, 250, 255);
        private static readonly Color LossColor = new Color32(255, 50, 50, 255);
        private static readonly Color ZeroColor = new Color32(150, 150, 150, 255);
        private static readonly float ShowTimeLen = 1.7f;
        
        private TextMeshPro _tooltipText;
        private int _score = 0;
        
        private float _hideTime = 0;
            
        
        void Start()
        {
            _tooltipText =  GetComponent<TextMeshPro>();
            this.gameObject.SetActive(false);
        }

        private void TooltipUpdate()
        {
            _tooltipText.color = SelectColor(_score);
            
            _tooltipText.text = (_score > 0)? "+"+_score : _score.ToString();
        }

        private static Color SelectColor(int score)
        {
            return score switch
            {
                > 0 => EarnColor,
                < 0 => LossColor,
                _ => ZeroColor
            };
        }

        public void ShowLabel(int score, Vector3 position, Quaternion? rotation = null)
        {
            this._score = score;
            TooltipUpdate();
            _hideTime = Time.time + ShowTimeLen;
            this.transform.position = position;
            if(rotation != null) this.transform.rotation = rotation.Value;
            this.gameObject.SetActive(true);
            StartCoroutine(HideLabelRoutine());
        }

        public float GetRemainingTime()
        {
            return _hideTime - Time.time;
        }

        public void HideLabel()
        {
            this.gameObject.SetActive(false);
        }

        private IEnumerator HideLabelRoutine()
        {
            yield return WaitsCache.GetWaitForSeconds(GetRemainingTime());
            HideLabel();
        }
    }
}
