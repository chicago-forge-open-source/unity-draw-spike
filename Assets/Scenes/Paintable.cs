using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class Paintable : MonoBehaviour
{
    public GameObject Brush;
    public float BrushSize = 0.1f;
    public RenderTexture RTexture;

    private bool[,] bitmap = new bool[_texture_size, _texture_size];
    private static int _texture_size = 100;

    // Update is called once per frame
    void Update()
    {
        
//        GetComponent<Renderer>().material.mainTexture = texture;


        if (Input.GetMouseButton(0))
        {
            //cast a ray to the plane
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(Ray, out hit))
            {
                Vector3 hitVector = hit.point;
                int x = (int) (Math.Round(hitVector.x) + 50);
                int z = (int) (Math.Round(hitVector.z) + 50);
                if (x < _texture_size && x >= 0 && z < _texture_size && z >= 0)
                {
                    Debug.Log("hi!!" + x + " " + z);
                    bitmap[x, z] = true;
                }

                Debug.Log(x + " " + z);

//                //instanciate a brush
//                var go = Instantiate(Brush, hit.point + Vector3.up * 0.1f, Quaternion.identity, transform);
//                go.transform.localScale = Vector3.one * BrushSize;
            }
        }

        var texture = RenderBitmap();

        var meshRenderer = this.GetComponent<MeshRenderer>();
        var materials = new List<Material>();
        meshRenderer.GetMaterials(materials);
        materials[0].mainTexture = texture;
    }

    private Texture2D RenderBitmap()
    {
        Texture2D texture = new Texture2D(_texture_size, _texture_size);
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                var bit = bitmap[_texture_size - 1 - x, _texture_size - 1 - y];
                Color color = (bit ? Color.black : Color.white);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
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

     
        var texture = RenderBitmap();
        
        //write data to file
        var data = texture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/savedImage.png", data);
    }
}