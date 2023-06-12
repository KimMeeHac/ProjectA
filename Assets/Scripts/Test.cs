using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;


public class Test : MonoBehaviour
{
    public float wantTime = 1;
    public Image image;

    private float timer = 0.0f;

    private void Start()
    {
        setResolution();
    }

    private void setResolution()
    {
        float targetRatio = 9.0f / 16.0f;
        float currentSize = (float)Screen.width / Screen.height;
        float scaleHight = currentSize / targetRatio;
        float fixedWidth = (float)Screen.width / scaleHight;
        Screen.SetResolution((int)fixedWidth, Screen.height, true);
    }

    private void Update()
    {
        if (image.color.a < 1)//�ణ�� ���İ� ���ְų� Ȥ�� ����ȭ �Ȼ����϶� 
        {
            timer += Time.deltaTime;
            Color color = image.color;
            color.a += (1 / wantTime) * Time.deltaTime;
            image.color = color;
            if (image.color.a >= 1)
            {
                Debug.Log($"���� �ð��� = {timer}");
            }
        }
    }

}
