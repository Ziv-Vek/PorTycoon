using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransBackground : MonoBehaviour
{
    public List<Image> images;
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            transform.parent.Find("BackGround").GetComponent<Image>().color = new Color(transform.parent.Find("BackGround").GetComponent<Image>().color.r, transform.parent.Find("BackGround").GetComponent<Image>().color.g, transform.parent.Find("BackGround").GetComponent<Image>().color.b, 0.3f);
            foreach(Image image in images)
            {
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.6f);
            }
        }
    }  
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Player")
        {
            transform.parent.Find("BackGround").GetComponent<Image>().color += new Color(0, 0, 0, 0.7f);
            foreach (Image image in images)
            {
                image.color += new Color(0, 0, 0, 0.7f);
            }
        }
    }
}
