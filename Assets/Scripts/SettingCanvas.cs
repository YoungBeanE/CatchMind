using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

public class SettingCanvas : MonoBehaviourPun
{
    [SerializeField] Button Red = null;
    [SerializeField] Button Yellow = null;
    [SerializeField] Button Green = null;
    [SerializeField] Button Cyan = null;
    [SerializeField] Button Blue = null;
    [SerializeField] Button Magenta = null;
    [SerializeField] Button Gray = null;
    [SerializeField] Button Black = null;
    [SerializeField] Button White = null;

    Color LineColor = Color.black;
    Camera myCam = null;
    GameObject Lineobj = null;
    Line currentLine = null;
    List<Vector2> fingerPositions = new List<Vector2>();
    List<GameObject> LineRecord = new List<GameObject>();

    private void Awake()
    {
        Red.onClick.AddListener(delegate { Set_Red(); });
        Yellow.onClick.AddListener(delegate { Set_Yellow(); });
        Green.onClick.AddListener(delegate { Set_Green(); });
        Cyan.onClick.AddListener(delegate { Set_Cyan(); });
        Blue.onClick.AddListener(delegate { Set_Blue(); });
        Magenta.onClick.AddListener(delegate { Set_Magenta(); });
        Gray.onClick.AddListener(delegate { Set_Gray(); });
        Black.onClick.AddListener(delegate { Set_Black(); });
        White.onClick.AddListener(delegate { Set_White(); });


        myCam = FindObjectOfType<Camera>();
    }
    public void Myturn()
    {
        StartCoroutine(nameof(DrawLine));
    }
    public void End()
    {
        for(int i = 0; i < LineRecord.Count; i++)
        {
            PhotonNetwork.Destroy(LineRecord[i]);
        }
        LineRecord.Clear();
        StopCoroutine(nameof(DrawLine));
    }
    IEnumerator DrawLine()
    {
        while (true)
        {
            //roll back button
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Z))
            {
                if (LineRecord.Count != 0)
                {
                    PhotonNetwork.Destroy(LineRecord[LineRecord.Count - 1]);
                    LineRecord.RemoveAt(LineRecord.Count - 1);
                }
            }
            yield return null;
            
            //click draw line 
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mouse = myCam.ScreenToWorldPoint(Input.mousePosition);
                if (mouse.y < -3.4f || mouse.y > 3.5f || mouse.x < -8f || mouse.x > 2.4f || (mouse.y > -1 && mouse.x > 1.9) || (mouse.y < -2.7 && mouse.x < -7.7)) continue;
                LineRecord.Add(CreateLine());
            }
            

            if (Input.GetMouseButton(0))
            {
                if (fingerPositions.Count == 0) continue;

                Vector3 temp = myCam.ScreenToWorldPoint(Input.mousePosition);
                if (temp.y < -3.4f || temp.y > 3.5f || temp.x < -8f || temp.x > 2.3f || (temp.y > -1 && temp.x > 1.9) || (temp.y < -2.7 && temp.x < -7.7)) continue;
                temp.z = 0;
                if(Vector2.Distance(temp, fingerPositions[fingerPositions.Count -1]) > 0.1f)
                {
                    UpdateLine(temp);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                fingerPositions.Clear();
            }
        }
    }


    GameObject CreateLine()
    {
        Lineobj = PhotonNetwork.Instantiate(nameof(Line), Vector3.zero, Quaternion.identity);
        Lineobj.TryGetComponent<Line>(out currentLine);
        currentLine.SetColor(LineColor);

        Vector3 StartPos = myCam.ScreenToWorldPoint(Input.mousePosition);
        fingerPositions.Add(StartPos);
        fingerPositions.Add(StartPos);
        currentLine.SetPosition(StartPos);

        return Lineobj;
    }


    void UpdateLine(Vector2 newFingerPos)
    {
        fingerPositions.Add(newFingerPos);
        currentLine.UpdatePosition(newFingerPos);
    }

    #region SetColor
    void Set_Black()
    {
        LineColor = Color.black;
    }
    void Set_White()
    {
        LineColor = Color.white;
    }
    void Set_Red()
    {
        LineColor = Color.red;
    }
    void Set_Yellow()
    {
        LineColor = Color.yellow;
    }
    void Set_Green()
    {
        LineColor = Color.green;
    }
    void Set_Blue()
    {
        LineColor = Color.blue;
    }
    void Set_Cyan()
    {
        LineColor = Color.cyan;
    }
    void Set_Magenta()
    {
        LineColor = Color.magenta;
    }
    void Set_Gray()
    {
        LineColor = Color.gray;
    }
    #endregion

    /*
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(ray.origin.y > -3.5f && Physics.Raycast(ray, out hit)) hit.transform.GetComponent<Line>().Picture(LineColor);
        }
    */
}
