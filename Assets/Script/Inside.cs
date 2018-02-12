using System.Collections;
using System.Timers;
using UnityEngine;

public class Inside : MonoBehaviour
{
    public GameObject Ball;

    private IEnumerator Oui()
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log("500ms");
        //GetComponent<Rigidbody>().MovePosition(Vector3.up);
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void OnTriggerEnter(Collider collider)
    {
        StartCoroutine(Oui());
    }

    private void OnTriggerStay(Collider collider)
    {
    }

    private void OnTriggerExit(Collider collider)
    {
        StopCoroutine(Oui());
    }
}