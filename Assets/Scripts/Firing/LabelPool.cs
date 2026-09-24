using TMPro;
using UnityEngine;
namespace Firing
{
    public class LabelPool : MonoBehaviour
    {
        [SerializeField] private int poolSize = 6;
        public FiringPointLabel labelPrefab;
        
        private FiringPointLabel[] _labelPool;
    
        private void Awake()
        {
            _labelPool = new FiringPointLabel[poolSize];
            for (int i = 0; i < _labelPool.Length; i++)
            {
                _labelPool[i] = Instantiate(labelPrefab, transform);
            }
        }

        private FiringPointLabel GetLabel()
        {
            var chosen =  _labelPool[0];
            foreach (var current in _labelPool)
            {
                if(chosen.GetRemainingTime() <= 0) return chosen;
                if(current.GetRemainingTime() < chosen.GetRemainingTime())
                    chosen = current;
            }
            return chosen;
        }


        public void ShowPointLabel(int score, Vector3 position)
        {
            FiringPointLabel label = GetLabel();
            label.ShowLabel(score, position);
        }
        
    }
}
