using UnityEngine;

public class SwitchAudio : MonoBehaviour {
    [SerializeField]
    private AudioSource[] _audios;
    
    /// <summary>
    /// BGMの混ぜ具合。0ならSound1、1ならSound2になる
    /// </summary>
    [Range(0, 1)]
    public float _mixRate = 0;
    
    public void Play() {
        _audios[0].Play();
        _audios[1].Play();
    }
    
    private void Update () {
        _audios[0].volume = 1f - _mixRate;
        _audios[1].volume = _mixRate;
    }
}
