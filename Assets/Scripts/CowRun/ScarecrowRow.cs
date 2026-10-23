using System.ComponentModel;
using LocomotionCollision;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CowRun
{
    public class ScarecrowRow : MonoBehaviour
    {
#if UNITY_EDITOR
        
        public ScarecrowSpawner prefab;
        public GameObject target;
        public int spawnCount;
        public float distance;
        public float angle;
        public SplineLocomotionCollision roadSpline;

        public void GenerateRow()
        {
            var oldSpawners = this.GetComponentsInChildren<ScarecrowSpawner>();
            foreach (var spw in oldSpawners)
            {
                DestroyImmediate(spw.gameObject);
            } 
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            
            Vector3 spawnPosition = this.transform.position;
            for (int i = 0; i < spawnCount; i++)
            {
                var spawner = Instantiate(prefab, spawnPosition, Quaternion.identity, this.transform);
                spawner.roadSpline = roadSpline;
                spawner.target = target;
                
                var outs = roadSpline.GetClosestPointAndTangent(spawnPosition);
                spawnPosition += (rotation * outs.splineTangent)*distance;
            }
        }

#endif
    }

}
