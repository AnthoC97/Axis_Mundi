using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deplacement : MonoBehaviour {
    [SerializeField]
    private float speed = 5;
    [SerializeField]
    private float strafeSpeed = 3;
    [SerializeField]
    private float mouseXSensitivityX = 50;
    [SerializeField]
    private float mouseYSensitivityY = 50;
    /*[SerializeField]
    private float dash;*/
    private Vector3 direction;
    private PhotonView pv;
    [SerializeField]
    private GameObject projectil;
    [SerializeField]
    private int force = 200;
    [SerializeField]
    private float firerate = 1;
    float nextFire = 0;
    private Rigidbody rb;
    [SerializeField]
    private float dashSpeed;
    private int dashDirection;
    private float dashTime;
    [SerializeField]
    private float startDashTime;
    private float dashDelay;
    [SerializeField]
    private float startDashDelay;
    private bool canDash = false;
    
    

    private bool canInstantiateProjectil = false;

    // Use this for initialization
    void Start () {
        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        dashTime = startDashTime;
        dashDelay = startDashDelay;
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(dashTime);
        direction = Vector3.zero;
        if (canDash)
        {
            dashDelay -= Time.deltaTime;
        }
        if(dashDelay <= 0)
        {
            dashDelay = startDashDelay;
            canDash = false;
        }
        if (pv.isMine)
        {
           if (Input.GetKey(KeyCode.Z))
            {
                if(canDash == false)
                {
                    direction += Vector3.forward;
                    canDash = true;
                }
                else
                {
                    dashDirection = 2;
                    Debug.Log("yes");
                }
            }
            if (Input.GetKey(KeyCode.Q))
            {
                if (canDash == false)
                {
                    direction += Vector3.left;
                    canDash = true;
                }
                else
                {
                    dashDirection = 1;
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (canDash == false)
                {
                    direction += Vector3.back;
                    canDash = true;
                }
                else
                {
                    dashDirection = 8;
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (canDash == false)
                {
                    direction += Vector3.right;
                    canDash = true;
                }
                else
                {
                    dashDirection = 4;
                }
            }
            //Limiter la cadence de tir
            if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
            {
                nextFire = Time.time + firerate;
                canInstantiateProjectil = true;
            }
            if (dashDirection != 0)
            {
                if(dashTime <= 0)
                {
                    dashDirection = 0;
                    dashTime = startDashTime;
                    rb.velocity = Vector3.zero;
                }
                else
                {
                    dashTime -= Time.deltaTime;
                    if(dashDirection == 1)
                    {
                        rb.velocity = transform.rotation * Vector3.left * dashSpeed;
                        canDash = false;
                    }
                    if(dashDirection == 2)
                    {
                        rb.velocity = transform.rotation * Vector3.forward * dashSpeed;
                        canDash = false;
                    }
                    if(dashDirection == 4)
                    {
                        rb.velocity = transform.rotation * Vector3.right * dashSpeed;
                        canDash = false;
                    }
                    if (dashDirection == 8)
                    {
                        rb.velocity = transform.rotation * Vector3.back * dashSpeed;
                        canDash = false;
                    }
                }
            }
        }
	}

    private void FixedUpdate()
    {
        if (pv.isMine)
        {
            transform.position += transform.rotation * direction.normalized * speed * Time.deltaTime;
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * mouseXSensitivityX * Time.deltaTime);
            if (canInstantiateProjectil)
            {
                //On fait appel à la RPC qui instanciera sur chaque client (et non sur le reseau) une balle
                pv.RPC("Shoot", PhotonTargets.All, transform.Find("ProjectilInstanciationPlace").transform.position);
                canInstantiateProjectil = false;
            }
            /*if(dashDirection == 1)
            {
                rb.velocity = Vector3.left * dashSpeed;
            }
            if(dashDirection == 2)
            {
                rb.velocity = Vector3.forward * dashSpeed;
            }
            if (dashDirection == 4)
            {
                rb.velocity = Vector3.right * dashSpeed;
            }
            if(dashDirection == 8)
            {
                rb.velocity = Vector3.back * dashSpeed;
            }*/
        }
    }

    [PunRPC]
    private void Shoot(Vector3 pos)
    {
        GameObject prjtl = Instantiate(projectil, pos, Quaternion.identity) as GameObject;
        prjtl.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * force);
    }
}
