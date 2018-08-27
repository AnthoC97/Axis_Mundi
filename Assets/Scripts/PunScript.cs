using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PunScript : MonoBehaviour {

    [SerializeField]
    private GameObject prefabPlayer;
    [SerializeField]
    private Transform spawnTransform1;
    [SerializeField]
    private Transform spawnTransform2 ;
    [SerializeField]
    private Transform spawnTransform3;
    [SerializeField]
    private Transform spawnTransform4;
    [SerializeField]
    private Text txtInfos;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private GameObject loadCamera;

    // Use this for initialization
    void Start () {
        PhotonNetwork.ConnectUsingSettings("version1");
        Cursor.lockState = CursorLockMode.Locked;
	}
	
	// Update is called once per frame
	void Update () {
        if(PhotonNetwork.connectionStateDetailed.ToString() != "Joined")
        {
            txtInfos.text = PhotonNetwork.connectionStateDetailed.ToString();
        }
        else
        {
            txtInfos.text = "Connected to the room:" + PhotonNetwork.room.name + " player(s) Online:" + PhotonNetwork.room.playerCount + " Master: "+ PhotonNetwork.isMasterClient;
        }
        //Debug.Log(PhotonNetwork.playerList.Length);
	}

    void OnJoinedLobby() //Des qu'un joueur se connecte au lobby (ouverture du jeu pour l'instant)
    {
        RoomOptions myRoomOption = new RoomOptions();
        myRoomOption.MaxPlayers = 4; //nb de joueur max
        PhotonNetwork.JoinOrCreateRoom("arene",myRoomOption,TypedLobby.Default); //Creation d'une room
    }

    void OnJoinedRoom()//Des que la room est creee on instantie les joueurs
    {
        loadCamera.SetActive(false);
        mainCamera.gameObject.SetActive(true);
        StartCoroutine(SpawnMyPlayer());
    }

    void OnDisconnectedFromPhoton()
    {
        Debug.Log("left photon");
        SceneManager.LoadScene(0);
    }

    //Coroutine pour instancuer les joueurs
    IEnumerator SpawnMyPlayer()
    {
        Transform sp;
        //Differents spawns
        if (PhotonNetwork.playerList.Length < 2)
        {
            sp = spawnTransform1;
        }
        else if(PhotonNetwork.playerList.Length < 3)
        {
            sp = spawnTransform2;
        }
        else if(PhotonNetwork.playerList.Length < 4)
        {
            sp = spawnTransform3;
        }
        else
        {
            sp = spawnTransform4;
        }
        GameObject myPlayer = PhotonNetwork.Instantiate(prefabPlayer.name, sp.position, Quaternion.identity, 0) as GameObject; //Instantiation du joueur
        mainCamera.GetComponent<CameraScript>().player = myPlayer.transform; //On attache une camera au joueur
        yield return new WaitForSeconds(5);
        /*myPlayer.AddComponent<Rigidbody>(); //On ajoute un rigidbody au joueur au bout de 5 minutes.
        myPlayer.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        myPlayer.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;*/
    }
}
