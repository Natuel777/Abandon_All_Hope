    using System.Collections.Generic;
    using UnityEngine;

    public struct AudioClipData
    {
        public string name;
        public Vector3? position;

        public AudioClipData(string name, Vector3? position = null)
        {
            this.name = name;
            this.position = position;
        }
    }

    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        private Dictionary<string, AudioClip> _audioClips = new();
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private List<AudioClip> _audioClipsList = new();
        private HashSet<string> _playedClips = new();
        
        private void Awake() 
        {
            if(Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            foreach(AudioClip clip in _audioClipsList)
            {
                _audioClips[clip.name] = clip;
            }
        }

        public void PlaySFX(params object[] parameters)
        {
            if(_sfxSource == null)
            {
                Debug.Log("sfxSource is null in AudioManager.");
                return;
            } 

            _playedClips.Clear();

            foreach(var item in parameters)
            {
                if(item is AudioClipData data && data.position.HasValue == false)
                {
                    if(_playedClips.Contains(data.name)) continue;

                    _playedClips.Add(data.name);
                    
                    if(_audioClips.TryGetValue(data.name, out AudioClip clip))
                    {
                        _sfxSource?.PlayOneShot(clip);
                        Debug.Log($"AudioManager: Clip '{data.name}' played.");
                    }
                        
                    else Debug.Log($"AudioManager: Clip '{data.name}' not found!");
                }
            }
        }

        public void PlaySFXAt(params object[] parameters)
        {
            _playedClips.Clear();

            foreach (var item in parameters)
            {
                if (item is AudioClipData data && data.position.HasValue)
                {
                    if(_playedClips.Contains(data.name)) continue;

                    _playedClips.Add(data.name);

                    if (_audioClips.TryGetValue(data.name, out AudioClip clip))
                        AudioSource.PlayClipAtPoint(clip, data.position.Value);

                    else Debug.Log($"AudioManager: Clip '{data.name}' not found!");
                }
            }
        }
    }
