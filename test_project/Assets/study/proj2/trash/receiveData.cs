using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class receiveData : MonoBehaviour {

    public GameObject candyBlock;
    public static mapInfo mapData;
    public static List<List<GameObject>> candys = new List<List<GameObject>>();

    private Transform myTransform = null;

    private void Awake()
    {
        myTransform = GetComponent<Transform>();
    }

    void Start ()
    {
        //다시 메인으로 갔다올 시 남아있을수 있어서 초기화
        //메인 신에서 가져온 mapSaver의 정보를 통해 사탕의 위치와 활성화 여부를 정한다
        //x, y삾은 캔디 리스트의 인덱스이고 name도 인덱스 기준으로 정해짐
        candys.Clear();
        try
        {
            mapData = GameObject.Find("mapSaver").GetComponent<mapInfo>();

            for (int i = 0; i < mapData.mapPos.Count; i++)
            {
                candys.Add(new List<GameObject>());
                for (int j = 0; j < mapData.mapPos[i].Count; j++)
                {
                    candys[i].Add(Instantiate(candyBlock, new Vector3(mapData.mapPos[i][j].x, mapData.mapPos[i][j].y, 0), transform.rotation, transform));
                    candys[i][j].SetActive(mapData.mapUse[i][j]);
                    candys[i][j].name = "candy" + i.ToString() + j.ToString();
                    candys[i][j].GetComponent<myCandy>().x = i;
                    candys[i][j].GetComponent<myCandy>().y = j;
                }
            }
        }
        catch (System.NullReferenceException) { }
    }
    /*
    public myCandy GetMyCandyComponent(string name)
    {
        //return myTransform.Find(name);
    }
    */

}
