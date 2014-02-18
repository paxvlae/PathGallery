using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PathElement
{
    public Texture Image;
    public GameObject GalleryObject;
    public bool IsImage { get { return Image != null; }}
    public bool IsObject { get { return GalleryObject != null; } }
}

public class PathGalleryMaker : MonoBehaviour
{

    public List<PathElement> galleryElements;
    public Material ImageMaterial;
    public float imageScaleMultiplifier;
    

	// Use this for initialization
    void Start ()
    {
        var gallerySlotsUnorder = GameObject.FindGameObjectsWithTag("PathElement");
        var gallerySlots = (from element in gallerySlotsUnorder
            let number = int.Parse(element.name.Split('_')[1])
            orderby number
            select element.transform).ToArray();
        for (int index = 0; index < galleryElements.Count && index < gallerySlots.Length; index++)
        {
            var element = galleryElements[index];
            if (element.IsImage)
            {
                var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.renderer.material = ImageMaterial;
                plane.renderer.material.SetTexture("_MainTex", element.Image);
                plane.transform.localScale = GetImageAspect(element.Image)*imageScaleMultiplifier;
                plane.transform.parent = gallerySlots[index].transform;
                plane.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
                plane.transform.localPosition = Vector3.zero;
            }
            else if (element.IsObject)
            {
                var obj = Instantiate(element.GalleryObject) as GameObject;
                obj.transform.parent = gallerySlots[index].transform;
            }
        }
    }

    Vector3 GetImageAspect(Texture texture)
    {
        float widthAspect;
        float heightAspect;

        if (texture.width > texture.height)
        {
            widthAspect = (float)texture.width / (float)texture.height;
            heightAspect = 1;
        }
        else
        {
            widthAspect = 1;
            heightAspect = (float)texture.height / (float)texture.width;
        }
        return new Vector3(widthAspect, 1, heightAspect);
    }
}
