using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	float volume = .75f;

	public static AudioManager instance;
	List<AudioSource> sources = new List<AudioSource>();

    private void Awake()
    {
		instance = this;
    }

    public void PlaySound(string name)
	{
		AudioSource SFX = gameObject.AddComponent<AudioSource>();

		try
		{
			SFX.clip = LoadClip(name);
		}
		catch (System.Exception)
		{
			Debug.LogWarning($"Can't load SFX \"{name}\"");
			Destroy(SFX);
			return;
		}

		SFX.volume = volume;
		sources.Add(SFX);
		SFX.Play();

		StartCoroutine(RemoveSFX());
		IEnumerator RemoveSFX()
		{
			yield return new WaitForSeconds(SFX.clip.length);
			sources.Remove(SFX);
			Destroy(SFX);
		}
	}

	AudioClip LoadClip(string name)
	{
		AudioClip clip = Resources.Load<AudioClip>($"Sound/{name}");
		if (clip == null)
		{
			throw new System.IO.FileNotFoundException($"Couldn't find an AudioClip at Resources/Sound/{name}");
		}
		return clip;
	}

	public float GetVolume() => volume;
	public void SetVolume(float volume)
	{
		this.volume = volume;
		foreach (var src in sources)
		{
			src.volume = this.volume;
		}
	}
}
