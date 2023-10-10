using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum SoundType
{
    BGM,
    SFX,
    MaxCount,  // �ƹ��͵� �ƴ�. �׳� Sound enum�� ���� ���� ���� �߰�. (0, 1, '2' �̷��� 2��) 
}

public class SoundManager : MonoBehaviour
{ 
    #region �̱���
    private static SoundManager instance = null;
    public static SoundManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    #endregion

    AudioSource[] audioSources = new AudioSource[(int)SoundType.MaxCount];
    Dictionary<string, AudioClip> BGMs = new Dictionary<string, AudioClip>();
    Dictionary<string, AudioClip> SFXs = new Dictionary<string, AudioClip>();

    float pitchSpeed = 0.25f;


    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void Start()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i] = this.transform.GetChild(i).GetComponent<AudioSource>();
        }
        GetOrAddAudioClip("Title", SoundType.BGM);
    }

    //����� �÷��� �Լ�
    /// <summary>
    /// ����� ��� ��(AudioClip)
    /// </summary>
    /// <param name="audioClip">�Ҹ� ���� �ֱ� ����</param>
    /// <param name="type">SoundManager�� Ÿ�� ����</param>
    public void Play(AudioClip audioClip, SoundType type)
    {
        if (audioClip == null)
            return;

        if (type == SoundType.BGM) // BGM ������� ���
        {
            AudioSource audioSource = audioSources[(int)SoundType.BGM];
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else // SFX ȿ���� ���
        {
            AudioSource audioSource = audioSources[(int)SoundType.SFX];
            audioSource.PlayOneShot(audioClip);
        }
    }
    /// <summary>
    /// �Ҹ� ���� ���丮 ��� string �Է�(Resoures/Sounds/.. BGM or SFX)
    /// </summary>
    /// <param name="path">���丮�� string���� �Է�</param>
    /// <param name="type">SoundManager�� Ÿ�� ����</param>
    public void Play(string path, SoundType type)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type);
    }

    public AudioSource LoofSFX(AudioClip audioClip)
    {
        if (audioClip == null)
            return null;

        AudioSource audioSource = Instantiate(audioSources[(int)SoundType.SFX]);
        audioSource.transform.parent = this.transform;
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();
        return audioSource;
    }
    public AudioSource LoofSFX(string path)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, SoundType.SFX);
        return LoofSFX(audioClip);
    }

    /// <summary>
    /// �Ҹ����� ��ųʸ��� �߰���
    /// </summary>
    /// <param name="path"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    AudioClip GetOrAddAudioClip(string path, SoundType type)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{type}/{path}"; // Sound ���� �ȿ� ����� �� �ֵ���, $���̸� �����Ϸ����� {}�� �����ΰ� �˾Ƽ� ������

        AudioClip audioClip = null;

        if (type == SoundType.BGM) // BGM ������� Ŭ�� ���̱�
        {
            if (BGMs.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                BGMs.Add(path, audioClip);
            }
        }
        else // Effect ȿ���� Ŭ�� ���̱�
        {
            if (SFXs.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Resources.Load<AudioClip>(path);
                SFXs.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }
    public void BGMFaster(float pitch)
    {
        AudioSource _bgm = audioSources[(int)SoundType.BGM];
        _bgm.pitch = pitch;
    }

    public void BGMPitchReset()
    {
        AudioSource _bgm = audioSources[(int)SoundType.BGM];
        _bgm.pitch = 1.0f;
    }
}
