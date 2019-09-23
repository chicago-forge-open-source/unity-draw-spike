using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Windows;

public class SwipeTrail : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    public RenderTexture RTexture;
    private int frameCount = 0;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        frameCount++;
        if (frameCount % 60 == 0)
        {
            _lineRenderer.Simplify(1);

            var positions = new Vector3[_lineRenderer.positionCount];
            _lineRenderer.GetPositions(positions);

            var output = "";

            foreach (var position in positions)
            {
                output += position.x + "," + position.y + "," + position.z + "\n";
            }

            Debug.Log("size of output is " + output.Length);
        }

        if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
             || Input.GetMouseButton(0)))
        {
            Plane objPlane = new Plane(Camera.main.transform.forward * -1, this.transform.position);
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (objPlane.Raycast(mRay, out rayDistance))
            {
                var positions = new Vector3[_lineRenderer.positionCount + 1];
                _lineRenderer.GetPositions(positions);

                positions[_lineRenderer.positionCount] = mRay.GetPoint(rayDistance);

                _lineRenderer.positionCount = positions.Length;
                _lineRenderer.SetPositions(positions);

//				this.transform.position = mRay.GetPoint(rayDistance);
                Debug.Log(positions.Length);
            }
        }
    }
    
    public void Save()
    {
        StartCoroutine(CoSave());
    }

    private IEnumerator CoSave()
    {
        //wait for rendering
        yield return new WaitForEndOfFrame();
        Debug.Log(Application.dataPath + "/savedImage.png");

        //set active texture
        RenderTexture.active = RTexture;

        //convert rendering texture to texture2D
        var texture2D = new Texture2D(RTexture.width, RTexture.height);
        texture2D.ReadPixels(new Rect(0, 0, RTexture.width, RTexture.height), 0, 0);
        texture2D.Apply();

        //write data to file
        var data = texture2D.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/savedImage.png", data);
    }
}