using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class RoomManager : MonoBehaviourPunCallbacks
{

    public static RoomManager instance;

    public int winner = 0;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if(scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate("PhotonPrefabs/PlayerManager", Vector3.zero, Quaternion.identity);
            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/GameManager", Vector3.zero, Quaternion.identity);
        }
        else if (scene.buildIndex == 2)
        {
            PhotonNetwork.InstantiateRoomObject("PhotonPrefabs/WinScreenManager", Vector3.zero, Quaternion.identity);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
