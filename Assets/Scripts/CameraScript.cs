using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    [SerializeField]
    public Transform player;
    [SerializeField]
    public GameObject t_camera;

    [Header("Paramètres")]
    //parametres gerant la position et la rotation de la camera
    [SerializeField]
    private float distance = 10.0f;
    [SerializeField]
    private float hauteur = 2.0f;
    [SerializeField]
    private float decalage = 2.0f;
    [SerializeField]
    private float currentX = 0.0f;
    [SerializeField]
    private float currentY = 0.0f;
    [SerializeField]
    private float mouseSensitivityX = 50.0f;
    [SerializeField]
    private float sensitivityY = 1.0f;

    //Marge d'angle de rotation sur l'axe Y 
    private const float Y_ANGLE_MIN = -50.0f;
    private const float Y_ANGLE_MAX = 50.0f;

    [Header("Collision_Camera")]


    [Header("Tranparency")]
    [SerializeField]
    private bool changeTranparency = true;
    [SerializeField]
    private MeshRenderer targetRender;

    [Header("Speeds")]
    [SerializeField]
    private float moveSpeed = 5;
    [SerializeField]
    private float returnSpeed = 9;
    [SerializeField]
    private float wallPush = 0.7f;

    [Header("Distances")]
    [SerializeField]
    private float closestDistanceToPlayer = 2;
    [SerializeField]
    private float evenCloserDistanceToPlayer = 1;

    [Header("Mask")]
    [SerializeField]
    private LayerMask collisionMask;

    private bool pitchLock;

    private void FixedUpdate()
    {
        Quaternion rotation;
        if (!pitchLock)
        {
            //On recuperre la position de la souris
            currentX += Input.GetAxis("Mouse X");
            //currentY += Input.GetAxis("Mouse Y");

            //cette variable a pour valeur toute valeur comprise dans la marge decrite plus haut
            currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

            rotation = Quaternion.Euler(currentY, currentX * mouseSensitivityX * Time.deltaTime, 0); //Quaternion qui represente la rotation de la camera par rapport au joueur
        }
        else
        {
            currentX += Input.GetAxis("Mouse X");
            //currentY = Y_ANGLE_MIN;
            rotation = Quaternion.Euler(currentY, currentX * mouseSensitivityX * Time.deltaTime, 0); //Quaternion qui represente la rotation de la camera par rapport au joueur
        }

        Vector3 dir = new Vector3(decalage, hauteur, -distance); //vector3 qui represente la position de la camera par rapport au joueur
        CollisionCheck(player.position + rotation * dir); //changement de position de la camera
        transform.LookAt(player.position); //la camera scrute toujours le joeur
        //Pour viser
        if (Input.GetMouseButton(1))
        {
            distance = 1.0f;
            hauteur = 0.25f;
            Color temp = targetRender.sharedMaterial.color;
            temp.a = Mathf.Lerp(temp.a, 0.2f, moveSpeed * Time.deltaTime);

            targetRender.sharedMaterial.color = temp;
        }
        //Arrete de viser
        else
        {
            distance = 5.0f;
            hauteur = 1.0f;
            Color temp = targetRender.sharedMaterial.color;
            temp.a = Mathf.Lerp(temp.a, 1f, moveSpeed * Time.deltaTime);

            targetRender.sharedMaterial.color = temp;
        }
    }

    private void WallCheck()
    {
        Ray ray = new Ray(player.position, -player.forward);
        RaycastHit hit;

        if (Physics.SphereCast(ray, 0.5f, out hit, collisionMask))
        {
            pitchLock = true;
        }
        else
        {
            pitchLock = false;
        }
    }

    private void CollisionCheck(Vector3 retPoint)
    {
        RaycastHit hit;

        if (Physics.Linecast(player.position, retPoint, out hit, collisionMask))
        {
            Vector3 norm = hit.normal * wallPush;
            Vector3 p = hit.point + norm;

            TranparencyCheck();

            if (Vector3.Distance(Vector3.Lerp(transform.position, p, moveSpeed * Time.deltaTime), player.position) <= evenCloserDistanceToPlayer)
            {

            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, p, moveSpeed * Time.deltaTime);
            }

            return;

        }

        FullTransparency();

        transform.position = Vector3.Lerp(transform.position, retPoint, returnSpeed * Time.deltaTime);
        pitchLock = false;
    }

    private void TranparencyCheck()
    {
        if (changeTranparency)
        {
            if (Vector3.Distance(transform.position, player.position) <= closestDistanceToPlayer)
            {
                Color temp = targetRender.sharedMaterial.color;
                temp.a = Mathf.Lerp(temp.a, 0.2f, moveSpeed * Time.deltaTime);

                targetRender.sharedMaterial.color = temp;

            }
            else
            {
                if (targetRender.sharedMaterial.color.a <= 0.99f)
                {
                    Color temp = targetRender.sharedMaterial.color;
                    temp.a = Mathf.Lerp(temp.a, 1f, moveSpeed * Time.deltaTime);

                    targetRender.sharedMaterial.color = temp;
                }
            }
        }
    }

    private void FullTransparency()
    {
        if (changeTranparency)
        {
            if (targetRender.sharedMaterial.color.a <= 0.99f)
            {
                Color temp = targetRender.sharedMaterial.color;
                temp.a = Mathf.Lerp(temp.a, 1f, moveSpeed * Time.deltaTime);

                targetRender.sharedMaterial.color = temp;
            }
        }
    }

}
