using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mapBlock : MonoBehaviour {

    [HideInInspector]
    public bool selected;

    /// <summary>
    /// 블록의 디폴트값은 선택되어있는 빨간색 블록
    /// </summary>
    void Start () {
        selected = true;
        GetComponent<MeshRenderer>().material.color = Color.red;
	}

    /// <summary>
    /// 클릭시 선택 여부가 바뀌며, 선택 안된 블록은 하얀색으로 처리
    /// </summary>
    void OnMouseDown()
    {
        if (selected)
        {
            GetComponent<MeshRenderer>().material.color = Color.white;
        }
        else
        {
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
        selected = !selected;
    }
}
