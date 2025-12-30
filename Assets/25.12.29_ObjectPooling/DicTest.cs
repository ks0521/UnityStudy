using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPooling
{
    public class DicTest : MonoBehaviour
    {
        Dictionary<string, Color> colorDic;
        Dictionary<string, KeyCode> idDic;
        MeshRenderer render;
        // Start is called before the first frame update
        void Start()
        {
            idDic = new Dictionary<string, KeyCode>();
            render = GetComponent<MeshRenderer>();
            colorDic = new Dictionary<string, Color>();
            colorDic.Add("Red", Color.red);
            colorDic.Add("Blue", Color.blue);
            colorDic.Add("Green", Color.green);

            idDic.Add("A", KeyCode.A);
            idDic.Add("B", KeyCode.B);

            render.material.color = colorDic["Green"];
            Debug.Log(idDic["A"]);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                render.material.color = colorDic["Red"];
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                render.material.color = colorDic["Blue"];
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                render.material.color = colorDic["Green"];
            }
        }
    }

}
