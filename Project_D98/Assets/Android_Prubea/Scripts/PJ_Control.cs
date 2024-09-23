using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PJ_Control : MonoBehaviour
{
    //Referencias Privadas
    Rigidbody playerRb;
    PlayerInput playerInput;
    Vector2 moveInput;

    [Header("Player Stats")]
    public float speed;
    public float jumpForce;
    [SerializeField] bool isGrounded;

    [Header("Combat Parameters")]
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform shootingPoint;
    [SerializeField] int ammo;
    public int maxAmmo;



    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        ammo = maxAmmo;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        playerRb.velocity = new Vector3(moveInput.x * speed, playerRb.velocity.y, 0);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PowerUp"))
        {
            ammo += 1;
            other.gameObject.SetActive(false);
        }
    }

    //public void NombreMetodo (InputAction.CallbackContext)
    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            isGrounded = false;
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void Action(InputAction.CallbackContext context)
    {

        if (context.started && ammo>0)
        {
            ammo -= 1;
            Instantiate(bulletPrefab, shootingPoint.position, Quaternion.identity);
        }
        else
        {
            
        }
    }

}
