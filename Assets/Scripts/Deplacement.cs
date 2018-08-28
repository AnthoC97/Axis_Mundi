using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deplacement : MonoBehaviour
{
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
    private bool canDashForward = false;
    private bool canDashLeft = false;
    private bool canDashBack = false;
    private bool canDashRight = false;
    private int i = 0;
    private bool isMoving = false;
    [SerializeField]
    private float vitesseCourse = 1f;
    private bool isRunning = false;
    private static bool canPlay;
    [SerializeField]
    private float startNextDash;
    private float nextDash;
    private bool hasDashed = true;
    [SerializeField]
    private GameObject mainCam;
    [SerializeField]
    private GameObject targetCam;


    private bool canInstantiateProjectil = false;

    // Use this for initialization
    void Start()
    {
        pv = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        dashTime = startDashTime;
        dashDelay = startDashDelay;
        nextDash = startDashDelay;
        canPlay = true;
    }

    // Update is called once per frame
    void Update()
    {
        direction = Vector3.zero;
        //Des que le joueur peut dash, il a un delai de 0.1 secondes
        if (canDashForward || canDashLeft || canDashBack || canDashRight)
        {
            dashDelay -= Time.deltaTime;
        }
        if (dashDelay <= 0)
        {
            dashDelay = startDashDelay;
            canDashForward = false;
            canDashLeft = false;
            canDashBack = false;
            canDashRight = false;
        }
        if (hasDashed)
        {
            nextDash -= Time.deltaTime;
        }
        if(nextDash <= 0)
        {
            hasDashed = false;
            nextDash = startNextDash;
        }
        if (pv.isMine && canPlay)
        {
            if (Input.GetKey(KeyCode.Z))
            {
                if (canDashForward == false)
                {
                    direction += Vector3.forward;
                    isMoving = true;
                }
                //Si le joueur appuie et que canDash = true, il dash dans la direction represente par un nombre.
                else if(canDashForward && !hasDashed)
                {
                    hasDashed = true;
                    dashDirection = 2;
                }
            }
            //Des que le joueur leve le doigt pour arreter de bouger, il a un delai de 0.1 pour dash à partir de la frame suivante
            if (Input.GetKeyUp(KeyCode.Z) && isMoving)
            {
                isMoving = false;
                canDashForward = true;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                if (canDashLeft == false)
                {
                    direction += Vector3.left;
                    isMoving = true;
                }
                else
                {
                    dashDirection = 1;
                }
            }
            if (Input.GetKeyUp(KeyCode.Q) && isMoving)
            {
                isMoving = false;
                canDashLeft = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (canDashBack == false)
                {
                    direction += Vector3.back;
                    isMoving = true;
                }
                else
                {
                    dashDirection = 8;
                }
            }
            if (Input.GetKeyUp(KeyCode.S) && isMoving)
            {
                isMoving = false;
                canDashBack = true;
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (canDashRight == false)
                {
                    direction += Vector3.right;
                    isMoving = true;
                }
                else
                {
                    dashDirection = 4;
                }
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                isRunning = false;
            }
            if (Input.GetKeyUp(KeyCode.D) && isMoving)
            {
                isMoving = false;
                canDashRight = true;
            }
            //Limiter la cadence de tir
            if (Input.GetMouseButtonDown(0) && Time.time > nextFire)
            {
                nextFire = Time.time + firerate;
                canInstantiateProjectil = true;
            }
           /* if (Input.GetMouseButton(1))
            {
                Debug.Log("yees");
                targetCam.SetActive(true);
                mainCam.SetActive(false);
            }
            else
            {
                targetCam.SetActive(false);
                mainCam.SetActive(true);
            }*/
        }
    }

    private void FixedUpdate()
    {
        if (pv.isMine)
        {
            if (isRunning)
            {
                transform.position += transform.rotation * direction.normalized * speed * vitesseCourse * Time.deltaTime;
            }
            else
            {
                transform.position += transform.rotation * direction.normalized * speed * Time.deltaTime;
            }
            transform.Rotate(Vector3.up, Input.GetAxis("Mouse X") * mouseXSensitivityX * Time.deltaTime);
            if (canInstantiateProjectil)
            {
                //On fait appel à la RPC qui instanciera sur chaque client (et non sur le reseau) une balle
                pv.RPC("Shoot", PhotonTargets.All, transform.Find("ProjectilInstanciationPlace").transform.position);
                canInstantiateProjectil = false;
            }
            if (pv.isMine)
            {
                if (dashDirection != 0)
                {
                    if (dashTime <= 0)
                    {
                        dashDirection = 0;
                        dashTime = startDashTime;
                        rb.velocity = Vector3.zero;
                    }
                    else
                    {
                        dashTime -= Time.deltaTime;
                        if (dashDirection == 1)
                        {
                            rb.velocity = transform.rotation * Vector3.left * dashSpeed;
                            canDashLeft = false;
                            hasDashed = true;
                        }
                        //Dash Physique
                        if (dashDirection == 2)
                        {
                            rb.velocity = transform.rotation * Vector3.forward * dashSpeed;
                            canDashForward = false;
                            hasDashed = true;
                        }
                        if (dashDirection == 4)
                        {
                            rb.velocity = transform.rotation * Vector3.right * dashSpeed;
                            canDashRight = false;
                            hasDashed = true;
                        }
                        if (dashDirection == 8)
                        {
                            rb.velocity = transform.rotation * Vector3.back * dashSpeed;
                            canDashBack = false;
                            hasDashed = true;
                        }
                    }
                }
            }
        }
    }

    [PunRPC]
    private void Shoot(Vector3 pos)
    {
        GameObject prjtl = Instantiate(projectil, pos, Quaternion.identity) as GameObject;
        prjtl.GetComponent<Rigidbody>().velocity = transform.TransformDirection(Vector3.forward * force);
    }

    public static void CantPlay()
    {
        canPlay = false;
    }
}
