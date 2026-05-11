using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BLEControl : MonoBehaviour
{
    public float speed;
    public CharacterController character;
    void Start()
    {
        character = GetComponent<CharacterController>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = transform.right * x + transform.forward * z;
        character.SimpleMove(forward*speed);
    }
}
