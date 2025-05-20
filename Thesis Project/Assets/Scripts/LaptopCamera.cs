using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaptopCamera : MonoBehaviour
{
    //public RawImage rawImage; // Material for displaying the camera feed
    public GameObject plane;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log("Webcam device: " + devices[i].name);
        }


        if (devices.Length > 0)
        {
            WebCamTexture webcamTexture = new WebCamTexture(devices[1].name);
            plane.GetComponent<Renderer>().material.mainTexture = webcamTexture;
            //rawImage.texture = webcamTexture;
            webcamTexture.Play();
        }
    }
}
