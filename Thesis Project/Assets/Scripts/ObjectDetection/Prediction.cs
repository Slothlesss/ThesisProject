using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Prediction
{
    public ObjectResult object_result;
    public List<MarkerResult> marker_result;
}

[System.Serializable]
public class MarkerResult
{
    public int marker_id;
    public float x;
    public float y;
}

[System.Serializable]
public class ObjectResult
{
    public List<PredictionItem> predictions;
}

[System.Serializable]
public class Point2D
{
    public float x;
    public float y;
}

[System.Serializable]
public class PredictionItem
{
    public int tracker_id;
    public int class_id;
    public List<Point2D> obb;
    //public float width;
    //public float height;
    //public float confidence;
}
