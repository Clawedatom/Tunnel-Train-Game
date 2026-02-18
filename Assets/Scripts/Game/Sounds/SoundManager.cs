using UnityEngine;

public class SoundManager : MonoBehaviour
{
	private static SoundManager _instance;

	AudioSource source;

	public static SoundManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = FindFirstObjectByType<SoundManager>();	
				if ( _instance == null)
				{
					Debug.LogError("Sound Manager has not been assigned");
				}
			}
			return _instance;
		}
	}

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    private void Start()
    {
		source.Play();
    }
    public void PlaySoundAtPos(AudioClip clip, Vector3 pos)
	{
		AudioSource.PlayClipAtPoint(clip, pos);
	}
}
