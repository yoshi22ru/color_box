using UnityEngine;

public class SoundPlay : MonoBehaviour
{
    // シングルトンインスタンス
    public static SoundPlay Instance;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip selectSound;
    [SerializeField] AudioClip throwSound;

    public void selectBall()
    {
        audioSource.PlayOneShot(selectSound);
    }

    public void throwBall()
    {
        audioSource.PlayOneShot(throwSound);
    }
}