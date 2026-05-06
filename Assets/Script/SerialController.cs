using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;


public class SerialController : MonoBehaviour
{
    public float speed = 6f;
    SerialPort serialPort;
    bool portOpen = false;

    void Start()
    {
        serialPort = new SerialPort("COM4", 9600);
        serialPort.ReadTimeout = 50;

        try
        {
            serialPort.Open();
            portOpen = true;
            Debug.Log("Puerto Abierto");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error en la comuniación "+ex.Message);
        }
    }

    void Update()
    {
        if (portOpen)
        {
            try
            {
                string[] data = serialPort.ReadLine().Trim().Split('|');
                float x = float.Parse(data[0]);
                float z = float.Parse(data[1]);

                Debug.Log($"X:{x}, Z:{z}");

                Vector3 movement = new Vector3(x,0,z)*speed*Time.deltaTime;
                this.transform.Translate(movement);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error en la lectura"+e.Message);
            }
        }
    }

    private void OnTriggerEnter (Collider other)
    {
        Debug.Log("Colisión con " + other.name);
        serialPort.Write("1");
    }
    private void OnTriggerExit (Collider other)
    {
        Debug.Log("Sale de Colisión con " + other.name);
        serialPort.Write("0");
    }

}
