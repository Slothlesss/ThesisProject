using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
public class FaceDetector : MonoBehaviour
{
    WebCamTexture camTexture;
    CascadeClassifier cascade;
    OpenCvSharp.Rect myFace;
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        camTexture = new WebCamTexture(devices[0].name);
        GetComponent<Renderer>().material.mainTexture = camTexture;
        camTexture.Play();
        cascade = new CascadeClassifier(Application.dataPath + @"/haarcascade_frontalface_default.xml");
    }

    // Update is called once per frame
    void Update()
    {
        Mat frame = OpenCvSharp.Unity.TextureToMat(camTexture);
        FindNewFace(frame);
        DisplayFace(frame);
        //DisplayFinal(frame);
    }


    void FindNewFace(Mat frame)
    {
        var faces = cascade.DetectMultiScale(frame, 1.1, 2, HaarDetectionType.ScaleImage);

        if (faces.Length >= 1)
        {
            Debug.Log(faces[0].Location);
            myFace = faces[0];
        }
    }

    void DisplayFace(Mat frame)
    {
        if (myFace != null)
        {
            frame.Rectangle(myFace, new Scalar(102, 255, 181), 4);

            Point textPosition = new Point(myFace.X, myFace.Y - 10); // Slightly above the rectangle
            Cv2.PutText(
                frame,
                "My Face",
                textPosition,
                HersheyFonts.HersheySimplex,
                1.2,                     // Font scale
                new Scalar(102, 255, 181), // White color
                3                        // Thickness
            );
        }
        Texture texture = OpenCvSharp.Unity.MatToTexture(frame);
        GetComponent<Renderer>().material.mainTexture = texture;

    }
    void DisplayFinal(Mat frame)
    {
        Texture texture = OpenCvSharp.Unity.MatToTexture(frame);
        GetComponent<Renderer>().material.mainTexture = texture;

    }
}
