using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersHealth : MonoBehaviour {

    [SerializeField]
    private int health = 100;
    private Text txtHealth;
    private Text txtHealthOther;
    private PhotonView pv;
    private Text txtEndGame;
    private bool lost;
    private bool endGame;


	// Use this for initialization
	void Start () {
        txtHealth = GameObject.Find("TextHealth").GetComponent<Text>();
        txtHealthOther = GameObject.Find("TextHealthOther").GetComponent<Text>();
        pv = GetComponent<PhotonView>();
        txtEndGame = GameObject.Find("TextEndGame").GetComponent<Text>();
        endGame = false;
        lost = false;
    }
	
	//Points de vies enlevés quand on est touché par la balle d'un joueur.
	void OnCollisionEnter (Collision col) {
        if (col.gameObject.layer == 8 && pv.isMine && !endGame)
        {
            Debug.Log("yeees");
            health -= 10;
            Debug.Log("Health : " + health);
            txtHealth.text = "My Heath : " + health + "%";
            //On fait un appel à la RPC qui informe les autres que l'on a perdu de la vie.
            pv.RPC("UpdateHealth", PhotonTargets.Others, health);

            if(health == 0)
            {
                //Loose
                txtEndGame.GetComponent<Text>().enabled = true;
                txtEndGame.text = "You Loose";
                Deplacement.CantPlay();
                pv.RPC("Win", PhotonTargets.Others);
                lost = true;
                StartCoroutine(GameEnd());
            }
        }
	}

    [PunRPC]
    private void UpdateHealth(int health)
    {
        txtHealthOther.text = "Health Other : " + health + "%";
    }

    [PunRPC]
    private void Win()
    {
        Deplacement.CantPlay();
        txtEndGame.text = "You Win";
        txtEndGame.GetComponent<Text>().enabled = true;
        lost = false;
        StartCoroutine(GameEnd());
    }

    IEnumerator GameEnd()
    {
        endGame = true;
        Cursor.lockState = CursorLockMode.None;
        if (lost)
        {
            yield return new WaitForSeconds(6);
            PhotonNetwork.Disconnect();
        }
        else
        {
            yield return new WaitForSeconds(5);
            PhotonNetwork.Disconnect();
        }
    }
}
