using UnityEngine;

public class SfxPool : MonoBehaviour
{
    [SerializeField] private int poolSize = 4;
    
    private AudioWorker[] _audioSourcePool;
    
    private void Awake()
    {
        _audioSourcePool = new AudioWorker[poolSize];
        for (int i = 0; i < _audioSourcePool.Length; i++)
        {
            GameObject audioObj = new GameObject("SfxPool-Source" + i);
            audioObj.transform.SetParent(transform);
            var source = audioObj.AddComponent<AudioSource>();
            source.spatialBlend = 1;
            source.playOnAwake = false;
            _audioSourcePool[i] = new AudioWorker(source, -1);
        }
    }

    private AudioWorker GetAudioWorker(int newPriority)
    {
        AudioWorker chosen = _audioSourcePool[0];
        if(!chosen.source.isPlaying)return chosen;
        var chosenRemainingTime = chosen.GetRemainingTime();
        for (int i = 1; i < _audioSourcePool.Length; i++)
        {
            AudioWorker current = _audioSourcePool[i];
            if(!current.source.isPlaying) return current;// not playing ==> insta choose
                
            if (chosen.priority > current.priority)
            {
                chosen = current; // choose the one with higher priority
                chosenRemainingTime = current.GetRemainingTime();
                continue;
            }

            var currentRemainingTime = current.GetRemainingTime();
            if (chosen.priority == current.priority && chosenRemainingTime > currentRemainingTime)
            {
                chosen = current; // choose the one who is closer to finish
                chosenRemainingTime = currentRemainingTime;
                continue;
            }
        }
        if(chosen.priority > newPriority) return null;
        return chosen;
    }

    public void PlayClipAtPosition(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f, int priority = 0)
    {
        var worker = GetAudioWorker(priority);
        if(worker == null) return;//no worker usable (all have higher priority
        worker.priority = priority;
        var source = worker.source;
        source.Stop();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.transform.position = position;
        source.Play();
    }


    private class AudioWorker
    {
        public readonly AudioSource source;
        public int priority;
        
        public AudioWorker(AudioSource source, int priority)
        {
            this.source = source;
            this.priority = priority;
        }
        
        public float GetRemainingTime()
        {
            if (!source.clip) return 0f;
            float clipPitch = source.pitch == 0 ? 1f : Mathf.Abs(source.pitch);
            return (source.clip.length - source.time) / clipPitch;
            
        }
    } 
}

