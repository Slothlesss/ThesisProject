
using UnityEngine;
using System.Collections;
using OpenCvSharp;
using UnityEngine.UI;

public class ContoursDetector : MonoBehaviour
{
	WebCamTexture camTexture;

	// Use this for initialization
	void Start()
	{

		WebCamDevice[] devices = WebCamTexture.devices;
		camTexture = new WebCamTexture(devices[0].name);
		GetComponent<Renderer>().material.mainTexture = camTexture;
		camTexture.Play();
	}

	// Update is called once per frame
	void Update()
	{
		Mat frame = OpenCvSharp.Unity.TextureToMat(camTexture);
		DetectShapes(frame);
		DisplayFinal(frame);
	}

	void DetectShapes(Mat frame)
	{

		//Gray scale image
		Mat grayMat = new Mat();
		Cv2.CvtColor(frame, grayMat, ColorConversionCodes.BGR2GRAY);

		Mat thresh = new Mat();
		Cv2.Threshold(grayMat, thresh, 127, 255, ThresholdTypes.BinaryInv);


		// Extract Contours
		Point[][] contours;
		HierarchyIndex[] hierarchy;
		Cv2.FindContours(thresh, out contours, out hierarchy, RetrievalModes.Tree, ContourApproximationModes.ApproxNone, null);
		foreach (Point[] contour in contours)
		{
			double length = Cv2.ArcLength(contour, true);
			Point[] approx = Cv2.ApproxPolyDP(contour, length * 0.01, true);
			string shapeName = null;
			Scalar color = new Scalar();


			if (approx.Length == 3)
			{
				shapeName = "Triangle";
				color = new Scalar(0, 255, 0);
			}
			else if (approx.Length == 4)
			{
				OpenCvSharp.Rect rect = Cv2.BoundingRect(contour);
				if (rect.Width / rect.Height <= 0.1)
				{
					shapeName = "Square";
					color = new Scalar(0, 125, 255);
				}
				else
				{
					shapeName = "Rectangle";
					color = new Scalar(0, 0, 255);
				}
			}
			else if (approx.Length == 10)
			{
				shapeName = "Star";
				color = new Scalar(255, 255, 0);
			}
			else if (approx.Length >= 15)
			{
				shapeName = "Circle";
				color = new Scalar(0, 255, 255);
			}

			if (shapeName != null)
			{
				Moments m = Cv2.Moments(contour);
				int cx = (int)(m.M10 / m.M00);
				int cy = (int)(m.M01 / m.M00);

				Cv2.DrawContours(frame, new Point[][] { contour }, 0, color, -1);
				Cv2.PutText(frame, shapeName, new Point(cx - 50, cy), HersheyFonts.HersheySimplex, 1.0, new Scalar(0, 0, 0));
			}
		}


	}
	void DisplayFinal(Mat frame)
	{
		Texture texture = OpenCvSharp.Unity.MatToTexture(frame);
		GetComponent<Renderer>().material.mainTexture = texture;

	}

}
