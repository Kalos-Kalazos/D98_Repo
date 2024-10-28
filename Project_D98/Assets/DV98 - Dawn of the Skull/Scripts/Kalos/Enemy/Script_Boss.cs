using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

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
    public float health;
    [SerializeField]
    public float skullHealth;
    [SerializeField]
    public float maxSkullHealth;
    [SerializeField]
    private bool noTurrets;
    [SerializeField]
    public Transform player, shootPointFront, pivot;
    [SerializeField]
    public GameObject bulletPrefab;
    [SerializeField]
    GameObject maskL;
    [SerializeField]
    GameObject maskR;
    [SerializeField]
    GameObject bigLaser;
    [SerializeField]
    GameObject eyeLaserR;
    [SerializeField]
    GameObject eyeLaserL;

    [SerializeField]
    PlayableDirector timeLine;

    Script_Spaceship playerControl;

    Rigidbody rb;

    public delegate void BossHealthChanged(float skullHealth);
    public event BossHealthChanged OnBossHealthChanged;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;

        playerControl = player.GetComponent<Script_Spaceship>();

        rb = GetComponent<Rigidbody>();

        rb.angularDrag = angularDrag;
        rb.drag = linearDrag;

        health = 3;

        skullHealth = maxSkullHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health==1 && !noTurrets)
        {
            noTurrets = true;
            AnimatorActivate();
        }

        if (skullHealth <= 0)
        {
            health = 0;
        }

        if (health <= 0)
        {
            SceneManager.LoadScene("Scene_Victory");
        }

    }
    private void FixedUpdate()
    {
        PursuePlayer();

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Bullet") && noTurrets)
        {
            TakeDamage(playerControl.damage);
        }
    }

    public void TakeDamage(int damage)
    {
        skullHealth -= damage;
        skullHealth = Mathf.Clamp(skullHealth, 0, maxSkullHealth);

        if (OnBossHealthChanged != null)
        {
            OnBossHealthChanged(skullHealth);
        }
    }
    public void ChangeMask()
    {
        maskL.GetComponent<MeshCollider>().enabled = true;
        maskR.GetComponent<MeshCollider>().enabled = true;

        eyeLaserL.GetComponentInParent<Transform>().gameObject.SetActive(false);
        eyeLaserR.GetComponentInParent<Transform>().gameObject.SetActive(false);

        maskL.transform.SetParent(null);
        maskR.transform.SetParent(null);
    }

    public void AnimatorDeactivate()
    {
        gameObject.GetComponent<Animator>().enabled = false;
        timeLine.Pause();
    }
    public void AnimatorActivate()
    {
        gameObject.GetComponent<Animator>().enabled = true;
        timeLine.Resume();
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
