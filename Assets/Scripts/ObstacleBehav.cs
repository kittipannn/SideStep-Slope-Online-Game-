using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleBehav : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float speed = 50f;
    [SerializeField] float startSpeed = 50f;
    bool startGame = true;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        StartCoroutine(delay());
    }
    IEnumerator delay() 
    {
        yield return new WaitForSeconds(0.25f);
        startGame = false;
    }

    void moveMent() 
    {
        if (startGame)
        {
            rb.AddForce(Vector3.back * startSpeed * Time.deltaTime, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(Vector3.back * speed * Time.deltaTime, ForceMode.Impulse);
        }
    }
    private void FixedUpdate()
    {
        moveMent();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DestroyZone"))
        {
            Destroy(gameObject);
        }
    }
}
