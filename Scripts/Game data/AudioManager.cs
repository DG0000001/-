using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//音频管理器
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            // 如果实例不存在，则查找场景中是否存在该类型的对象
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();

                // 如果场景中不存在该类型的对象，则创建一个新的实例
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<AudioManager>();
                    singletonObject.name = typeof(AudioManager).ToString() + " (SingletonAudioManager)";

                    // 保证在场景切换时不被销毁
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }
    // 整个游戏中，总的音源数量
    private const int AUDIO_CHANNEL_NUM = 8;
    private struct CHANNEL
    {
        public AudioSource channel;
        public float keyOnTime; //记录最近一次播放音乐的时刻
    };
    private CHANNEL[] m_channels;
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        m_channels = new CHANNEL[AUDIO_CHANNEL_NUM];
        for (int i = 0; i < AUDIO_CHANNEL_NUM; i++)
        {
            //每个频道对应一个音源
            m_channels[i].channel = gameObject.AddComponent<AudioSource>();
            m_channels[i].keyOnTime = 0;
        }
    }
    private void Start()
    {
        PlayLoop(Resources.Load<AudioClip>("Music/m_sys_void_combine"), 100f, 0f);
    }
    //公开方法：播放一次，参数为音频片段、音量、左右声道、速度
    //这个方法主要用于音效，因此考虑了音效顶替的逻辑
    public int PlayOneShot(AudioClip clip, float volume, float pan, float pitch = 1.0f)
    {
        for (int i = 0; i < m_channels.Length; i++)
        {
            //如果正在播放同一个片段，而且刚刚才开始，则直接退出函数
            if (m_channels[i].channel.isPlaying &&
                 m_channels[i].channel.clip == clip &&
                 m_channels[i].keyOnTime >= Time.time - 0.03f)
                return -1;
        }
        //遍历所有频道，如果有频道空闲直接播放新音频，并退出
        //如果没有空闲频道，先找到最开始播放的频道（oldest），稍后使用
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
        //运行到这里说明没有空闲频道。让新的音频顶替最早播出的音频
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
    //公开方法：循环播放，用于播放长时间的背景音乐，处理方式相对简单一些
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

    //公开方法：停止所有音频
    public void StopAll()
    {
        foreach (CHANNEL channel in m_channels)
            channel.channel.Stop();
    }
    //公开方法：根据频道ID停止音频
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