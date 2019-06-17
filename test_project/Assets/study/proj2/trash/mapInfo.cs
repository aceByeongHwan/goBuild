using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//각 블록의 좌표와 사용 여부(게임 활성화)를 저장해서
//dondestroy를 통해 게임 신에 데이터 전달
public class mapInfo : MonoBehaviour {
    public List<List<Vector2>> mapPos = new List<List<Vector2>>();
    public List<List<bool>> mapUse = new List<List<bool>>();
}
