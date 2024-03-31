using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//��Ƶ������
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            // ���ʵ�������ڣ�����ҳ������Ƿ���ڸ����͵Ķ���
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();

                // ��������в����ڸ����͵Ķ����򴴽�һ���µ�ʵ��
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<AudioManager>();
                    singletonObject.name = typeof(AudioManager).ToString() + " (SingletonAudioManager)";

                    // ��֤�ڳ����л�ʱ��������
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }
    // ������Ϸ�У��ܵ���Դ����
    private const int AUDIO_CHANNEL_NUM = 8;
    private struct CHANNEL
    {
        public AudioSource channel;
        public float keyOnTime; //��¼���һ�β������ֵ�ʱ��
    };
    private CHANNEL[] m_channels;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        m_channels = new CHANNEL[AUDIO_CHANNEL_NUM];
        for (int i = 0; i < AUDIO_CHANNEL_NUM; i++)
        {
            //ÿ��Ƶ����Ӧһ����Դ
            m_channels[i].channel = gameObject.AddComponent<AudioSource>();
            m_channels[i].keyOnTime = 0;
        }
    }
    private void Start()
    {
        PlayLoop(Resources.Load<AudioClip>("Music/m_sys_void_combine"), 100f, 0f);
    }
    //��������������һ�Σ�����Ϊ��ƵƬ�Ρ������������������ٶ�
    //���������Ҫ������Ч����˿�������Ч������߼�
    public int PlayOneShot(AudioClip clip, float volume, float pan, float pitch = 1.0f)
    {
        for (int i = 0; i < m_channels.Length; i++)
        {
            //������ڲ���ͬһ��Ƭ�Σ����Ҹողſ�ʼ����ֱ���˳�����
            if (m_channels[i].channel.isPlaying &&
                 m_channels[i].channel.clip == clip &&
                 m_channels[i].keyOnTime >= Time.time - 0.03f)
                return -1;
        }
        //��������Ƶ���������Ƶ������ֱ�Ӳ�������Ƶ�����˳�
        //���û�п���Ƶ�������ҵ��ʼ���ŵ�Ƶ����oldest�����Ժ�ʹ��
        int oldest = -1;
        float time = 10000000.0f;
        for (int i = 0; i < m_channels.Length; i++)
        {
            if (m_channels[i].channel.loop == false &&
               m_channels[i].channel.isPlaying &&
               m_channels[i].keyOnTime < time)
            {
                oldest = i;
                time = m_channels[i].keyOnTime;
            }
            if (!m_channels[i].channel.isPlaying)
            {
                m_channels[i].channel.clip = clip;
                m_channels[i].channel.volume = volume;
                m_channels[i].channel.pitch = pitch;
                m_channels[i].channel.panStereo = pan;
                m_channels[i].channel.loop = false;
                m_channels[i].channel.Play();
                m_channels[i].keyOnTime = Time.time;
                return i;
            }
        }
        //���е�����˵��û�п���Ƶ�������µ���Ƶ�������粥������Ƶ
        if (oldest >= 0)
        {
            m_channels[oldest].channel.clip = clip;
            m_channels[oldest].channel.volume = volume;
            m_channels[oldest].channel.pitch = pitch;
            m_channels[oldest].channel.panStereo = pan;
            m_channels[oldest].channel.loop = false;
            m_channels[oldest].channel.Play();
            m_channels[oldest].keyOnTime = Time.time;
            return oldest;
        }
        return -1;
    }
    //����������ѭ�����ţ����ڲ��ų�ʱ��ı������֣�����ʽ��Լ�һЩ
    public int PlayLoop(AudioClip clip, float volume, float pan, float pitch = 1.0f)
    {
        for (int i = 0; i < m_channels.Length; i++)
        {
            if (!m_channels[i].channel.isPlaying)
            {
                m_channels[i].channel.clip = clip;
                m_channels[i].channel.volume = volume;
                m_channels[i].channel.pitch = pitch;
                m_channels[i].channel.panStereo = pan;
                m_channels[i].channel.loop = true;
                m_channels[i].channel.Play();
                m_channels[i].keyOnTime = Time.time;
                return i;
            }
        }
        return -1;
    }

    //����������ֹͣ������Ƶ
    public void StopAll()
    {
        foreach (CHANNEL channel in m_channels)
            channel.channel.Stop();
    }
    //��������������Ƶ��IDֹͣ��Ƶ
    public void Stop(string clipName)
    {
        foreach (var item in m_channels)
        {
            if (item.channel.clip.name!= null && clipName == item.channel.clip.name)
            {
                item.channel.Stop();
                return;
            }
        }
    }
}