using UnityEngine;
using System.Collections.Generic;

public class RandomSound : MonoBehaviour
{
    [SerializeField] AudioSource[] audioSource;
    [SerializeField] AudioClip lowSound;
    [SerializeField] AudioClip highSound;
    [SerializeField] float lottery_time = 5f;
    private float reset_time = 0f;

    [SerializeField][Range(1, 9)] int maxAudioSources;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxAudioSources = Mathf.Clamp(maxAudioSources, 1, audioSource.Length);
    }

    // Update is called once per frame
    void Update()
    {
        reset_time += Time.deltaTime;
        
        if (reset_time >= lottery_time)
        {
            reset_time = 0f;
            int numToPlay = Random.Range(1, maxAudioSources + 1);

            // 再生したAudioSourceの情報を格納するリスト
            List<string> playedLocations = new List<string>();
            int highSoundCount = 0;
            int lowSoundCount = 0;

            for (int i = 0; i < numToPlay; i++)
            {
                // ここでrandomIndexがmaxAudioSourcesの範囲でしか選ばれていないため、
                // 配列の境界外エラーが発生する可能性があります。
                // audioSource.Lengthに修正するのがより安全です。
                int randomIndex = Random.Range(0, audioSource.Length); 
                
                if (audioSource[randomIndex] != null)
                {
                    AudioClip soundToPlay;
                    if (Random.Range(0, 2) == 0)
                    {
                        soundToPlay = lowSound;
                        lowSoundCount++;
                    }
                    else
                    {
                        soundToPlay = highSound;
                        highSoundCount++;
                    }
                    audioSource[randomIndex].PlayOneShot(soundToPlay);
                    
                    // 「game0bject」を「gameObject」に修正
                    playedLocations.Add($"AudioSource at Index {randomIndex} ({audioSource[randomIndex].gameObject.name})");
                }
                // このelse文は、配列にnull要素が含まれている場合にのみ必要です
                // 現在のロジックでは、ランダムにnullインデックスを選んだ場合にのみ発生します。
                else
                {
                    Debug.Log("AudioSource at index" + randomIndex + "is missing or out of range");
                }
            }
            
            // Debug.Log文をforループの外に移動
            Debug.Log($"<color=cyan> --- サウンド再生イベント --- </color>");
            Debug.Log($"<color=white><b>鳴らしたAudioSourceの総数:</b></color> {numToPlay}個");
            Debug.Log($"<color=green><b>高い音の数:</b></color> {highSoundCount}個");
            Debug.Log($"<color=red><b><低い音の数:</b></color> {lowSoundCount}個");

            Debug.Log("<color=yellow><b>鳴らした場所:</b></color>");
            foreach (string location in playedLocations)
            {
                Debug.Log(location);
            }
        }
    }
}