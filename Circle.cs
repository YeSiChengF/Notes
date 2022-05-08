using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///
///</summary>

public class Circle : MonoBehaviour
{
    public GameObject CircleImg;
    public int Count = 6;
    private float angle = 0;
    private float changeAngle = 0;
    private void Start()
    {
        CircleImg = Resources.Load("CircleImg") as GameObject;
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        Vector2 size = rectTransform.sizeDelta;
        float r = size.x / 2;
        changeAngle = 360 / Count;

        for (int i = 0; i < Count; i++)
        {
            Vector3 center = transform.position;
            GameObject cube = (GameObject)Instantiate(CircleImg, this.transform);
            float hudu = (angle / 180) * Mathf.PI;
            float xx = center.x + r * Mathf.Cos(hudu);
            float yy = center.y + r * Mathf.Sin(hudu);
            cube.transform.position = new Vector3(xx, yy, 0);
            angle += changeAngle;
        }
    }

}
