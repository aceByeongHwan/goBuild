using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class AceVertex : MonoBehaviour {

	public AcePolygon m_Polygon;
	public Color m_Color = Color.white;
	public bool m_Use_Color = true;
	bool m_useColor = true;
	Vector3 m_v3 = Vector3.zero;
	Color m_rgba = Color.white;


	public Color NowColor
	{
		get 
		{ 
			return m_Use_Color == true ? m_Color : m_Polygon.color; 
		} 
	}

	void Awake()
	{
		m_Polygon = GetComponentInParent<AcePolygon> ();
	}
	void Start()
	{
		if (m_Polygon == null)
			return;

		m_v3 = transform.localPosition;
		m_Polygon.Invalidate (false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_Polygon == null)
			return;

		if (!m_v3.Equals (transform.localPosition) ||
			!m_rgba.Equals (m_Color) ||
			!m_useColor.Equals (m_Use_Color) ) 
		{
			m_v3 = transform.localPosition;
			m_rgba = m_Color;
			m_useColor = m_Use_Color;
			m_Polygon.Invalidate (false);
		}

	}
}
