using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterLeap : MonoBehaviour
{

    public float forwardLeapSpeed = 20f;
    public float backwardsLeapSpeed = 5f;
    public int dest;
    public bool isLeaping = false;

    public int delay = 15;
    public int delayCounter;

    public Transform[] points = new Transform[2];



    void Start() {
        // //Fetch the Rigidbody from the GameObject with this script attached
        // monsterRb = GetComponent<Rigidbody2D>();

        // needed to deep copy, must be a less gross way...
        Transform trans = new GameObject().transform;
        trans.position = transform.position; 
        points[1] = trans;
        Debug.Log(trans.position);
    }

    public void StartLeap() {

        Debug.Log("starting leap");
        isLeaping = true;
        delayCounter = delay;
    }

    public void Update() {
        
    }

    void FixedUpdate() {
        if (isLeaping) {
            if (delayCounter > 0) {
                delayCounter -= 1;
            }
            else {
                if (dest == 0) {
                    transform.position = Vector2.MoveTowards(transform.position, points[0].position, forwardLeapSpeed * Time.deltaTime);
                    if (Vector2.Distance(transform.position, points[0].position) < 0.2f) {
                        dest = 1;
                        Debug.Log("leap peak");
                        delayCounter = delay;
                    }
                }
                else {
                    transform.position = Vector2.MoveTowards(transform.position, points[1].position, backwardsLeapSpeed * Time.deltaTime);
                    if (Vector2.Distance(transform.position, points[1].position) < 0.2f) {
                        dest = 0;
                        isLeaping = false;
                        Debug.Log("leap finished");
                        Debug.Log(transform.position);
                        Debug.Log(points[1].position);
                    }
                }
            }
        } 
    }
}