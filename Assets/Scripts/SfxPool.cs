using UnityEngine;

public class SfxPool : MonoBehaviour
{
    public int poolSize = 3;
    
    private AudioSource[] _audioSourcePool;
    private int _poolPointer = 0;

    private void Start()
    {
        _audioSourcePool = new AudioSource[poolSize];
        for (int i = 0; i < _audioSourcePool.Length; i++)
        {
            GameObject audioObj = new GameObject("SfxPool-Source" + i);
            _audioSourcePool[i] = audioObj.AddComponent<AudioSource>();
            _audioSourcePool[i].spatialBlend = 1f; 
            _audioSourcePool[i].playOnAwake = false;
        }
    }

    public void PlayClipAtPosition(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        _poolPointer = (_poolPointer+1) % poolSize;
        var source  = _audioSourcePool[_poolPointer];
        source.Stop();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.transform.position = position;
        source.Play();
    }
}