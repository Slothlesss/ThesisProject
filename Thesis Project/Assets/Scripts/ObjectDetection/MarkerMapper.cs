using UnityEngine;
using System.Collections.Generic;

public class MarkerMapper : MonoBehaviour
{
    public Transform[] unityWorldMarkers = new Transform[4]; // Assigned in the inspector

    private Vector2[] imageMarkerPoints = new Vector2[4];
    private Vector3[] worldMarkerPoints = new Vector3[4];
    public void Initialize()
    {
        MarkerResult[] markers = new MarkerResult[4];

        markers[0] = new MarkerResult { marker_id = 0, x = 45f, y = 61.75f };
        markers[1] = new MarkerResult { marker_id = 1, x = 591f, y = 55f };
        markers[2] = new MarkerResult { marker_id = 2, x = 563f, y = 458f };
        markers[3] = new MarkerResult { marker_id = 3, x = 42f, y = 423f };
        UpdateImageMarkerData(markers);
    }
    public void UpdateImageMarkerData(MarkerResult[] markers)
    {
    
        foreach (MarkerResult marker in markers)
        {
             imageMarkerPoints[marker.marker_id] = new Vector2(marker.x, marker.y);
        }

        for (int i = 0; i < 4; i++)
        {
            worldMarkerPoints[i] = unityWorldMarkers[i].position;
        }
    }

    // Bilinear interpolation based on normalized UV
    public Vector3 MapImagePointToWorld(Vector2 imagePoint)
    {
        Vector2 A = imageMarkerPoints[0]; // top-left
        Vector2 B = imageMarkerPoints[1]; // top-right
        Vector2 C = imageMarkerPoints[2]; // bottom-right
        Vector2 D = imageMarkerPoints[3]; // bottom-left

        // Solve for (u, v) such that imagePoint = bilinear(A,B,C,D,u,v)
        Vector2 uv = InverseBilinear(imagePoint, A, B, C, D);

        float u = Mathf.Clamp01(uv.x);
        float v = Mathf.Clamp01(uv.y);

        // Bilinear interpolation in 3D space
        Vector3 top = Vector3.Lerp(worldMarkerPoints[0], worldMarkerPoints[1], u);
        Vector3 bottom = Vector3.Lerp(worldMarkerPoints[3], worldMarkerPoints[2], u);
        return Vector3.Lerp(top, bottom, v);
    }

    // Approximate inverse bilinear interpolation using numerical optimization
    private Vector2 InverseBilinear(Vector2 P, Vector2 A, Vector2 B, Vector2 C, Vector2 D, int iterations = 10)
    {
        float u = 0.5f;
        float v = 0.5f;

        for (int i = 0; i < iterations; i++)
        {
            Vector2 Q = Bilinear(A, B, C, D, u, v);
            Vector2 dQdu = (B - A) * (1 - v) + (C - D) * v;
            Vector2 dQdv = (D - A) * (1 - u) + (C - B) * u;

            Vector2 error = Q - P;

            // 2x2 linear system to solve for delta u, delta v
            float det = dQdu.x * dQdv.y - dQdu.y * dQdv.x;
            if (Mathf.Abs(det) < 1e-5f) break;

            float invDet = 1f / det;

            float du = invDet * (dQdv.y * error.x - dQdv.x * error.y);
            float dv = invDet * (-dQdu.y * error.x + dQdu.x * error.y);

            u -= du;
            v -= dv;

            if (error.sqrMagnitude < 0.001f) break;
        }

        return new Vector2(u, v);
    }

    private Vector2 Bilinear(Vector2 A, Vector2 B, Vector2 C, Vector2 D, float u, float v)
    {
        Vector2 top = Vector2.Lerp(A, B, u);
        Vector2 bottom = Vector2.Lerp(D, C, u);
        return Vector2.Lerp(top, bottom, v);
    }
}
