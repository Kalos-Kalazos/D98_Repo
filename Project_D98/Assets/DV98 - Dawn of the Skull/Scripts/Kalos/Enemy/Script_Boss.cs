using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Boss : MonoBehaviour
{
    [Header("=== Boss Movement Settings ===")]
    [SerializeField]
    private float moveSpeed = 10;
    [SerializeField]
    private float rotationSpeedBody = 5f;
    [SerializeField]
    private float angularDrag = 0.1f;
    [SerializeField]
    private float linearDrag = 0.1f;

    [Header("=== Boss Combat Settings ===")]
    [SerializeField]
    public Transform player, shootPointFront, pivot;
    [SerializeField]
    public GameObject bulletPrefab;

    Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        rb = GetComponent<Rigidbody>();

        rb.angularDrag = angularDrag;
        rb.drag = linearDrag;
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate()
    {
        PursuePlayer();

    }


    private void PursuePlayer()
    {
        //Calculo la direccion de los pivotes del jugador y añado una fuerza hacia el

        if (player != null)
        {

            Vector3 directionToPlayer = (player.position - transform.position).normalized;


            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeedBody * Time.deltaTime);


            rb.AddForce(transform.forward * moveSpeed * Time.deltaTime, ForceMode.VelocityChange);
        }
    }
}
