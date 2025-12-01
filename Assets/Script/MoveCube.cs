using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NumberDetectedEvent : UnityEvent<int, int> {}

public class MoveCube : MonoBehaviour
{
    [SerializeField] private Vector3 moveDirection = Vector3.forward;
    public NumberDetectedEvent onNumberDetected;
    public float targetVolume = 1.5f;

    [SerializeField] private AudioSource audioSource; // 追加：キューブ自身のAudioSource
    [SerializeField] private AudioClip selectSound;   // 追加：再生するAudioClip

    private Rigidbody rb;
    private int cubeNumber;
    private float moveSpeed;
    private int cubeIndex;
    private GameManager gameManager;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        onNumberDetected = new NumberDetectedEvent();
        moveSpeed = Random.Range(1f, 8f);
        audioSource.volume = targetVolume;

        string name = gameObject.name;
        string numberString = name.Replace("Num_Cube_", "");
        numberString = numberString.Replace(" Variant", "");
        numberString = numberString.Replace("(Clone)", "");

        if (int.TryParse(numberString, out cubeNumber)) { }
        else { Debug.LogError("ゲームオブジェクト名から数字を解析できませんでした: " + name); }
        if (rb == null) { Debug.Log("Rigidbodyコンポーネントがアタッチされていません"); }

        // AudioSourceがアタッチされていない場合はエラーログを出力
        if (audioSource == null)
        {
            Debug.LogError("AudioSourceコンポーネントがアタッチされていません。", this);
        }
    }
    
    // --- Initialize()メソッドからsoundPlayの引数を削除 ---
    public void Initialize(GameManager gm, int index)
    {
        gameManager = gm;
        cubeIndex = index;
        
        if (gameManager != null && gameManager.cubeMaterials.Length > cubeIndex)
        {
            GetComponent<MeshRenderer>().material = gameManager.cubeMaterials[cubeIndex];
        }
        onNumberDetected.AddListener(gameManager.ProcessNumber);
    }
    
    public void StartMove()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveDirection.normalized * moveSpeed;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "L_Palm" || other.name == "R_Palm")
        {
            // --- 修正箇所：AudioSourceを使って直接音を再生 ---
            if (audioSource != null && selectSound != null)
            {
                audioSource.PlayOneShot(selectSound);
            }
            onNumberDetected.Invoke(cubeIndex, cubeNumber);
            Destroy(gameObject, 1f);
        }
        else if (other.name == "DeadArea")
        {
            Destroy(gameObject, 1f);
        }
    }
}