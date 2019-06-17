using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]

public class AcePolygon : UIBasicSprite
{
    //public List<AceVertex> mListVerts = new List<AceVertex>();
    public AceVertex[] mListVerts;
    private AceVertex[] draw_v;


    public override void OnFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
    {
        if (mListVerts.Length < 3)
            return;

        int size = mListVerts.Length;
        int cnt = 0;
        int cur = 0;
        if (draw_v == null || draw_v.Length < 4)
        {
            draw_v = new AceVertex[4];
            for (int i = 0; i < 4;i++)
            {
                GameObject newVertex = new GameObject("draw_v " + i.ToString(), typeof(AceVertex));
                newVertex.transform.SetParent(transform);
                //draw_v.Add(newVertex.GetComponent<AceVertex>());
                draw_v[i] = newVertex.GetComponent<AceVertex>();
            }
        }

        for (int i = 0; i < size; i++)
        {
            draw_v[cur] = mListVerts[i];
            cur++;
            if (cur > 3)
            {
                for (int j = 0; j < 4; j++)
                {
                    Vector3 localP = Vector3.zero;
                    Color c = color;
                    if (draw_v[j] != null)
                    {
                        localP = draw_v[j].transform.localPosition;
                        c = draw_v[j].NowColor;
                    }
                    float xx = localP.x;
                    float yy = localP.y;
                    verts.Add(new Vector3(xx, yy));
                    cols.Add(c);
                    uvs.Add(new Vector2(0, 0));
                }
                cur = 0;
            }
        }
        if (cur != 0)
        {
            for (int i = cur; i < 4; i++)
            {
                draw_v[i] = draw_v[cur - 1];
            }
            for (int j = 0; j < 4; j++)
            {
                Vector3 localP = Vector3.zero;
                Color c = color;
                if (draw_v[j] != null)
                {
                    localP = draw_v[j].transform.localPosition;
                    c = draw_v[j].NowColor;
                }
                float xx = localP.x;
                float yy = localP.y;
                verts.Add(new Vector3(xx, yy));
                cols.Add(c);
                uvs.Add(new Vector2(0, 0));
            }
        }

    }



    public override Material material
    {
        get
        {
            var mat = base.material;
            if (mat != null)
                return mat;
            return mMat;
        }
        set
        {
            base.material = value;
        }
    }

#if UNITY_EDITOR

    /// <summary>
    /// Draw a visible orange outline of the bounds.
    /// </summary>

    void OnDrawGizmos()
    {
        if (mListVerts.Length > 1)
        {
            Gizmos.matrix = cachedTransform.localToWorldMatrix;
            Gizmos.color = new Color(1f, 0.0f, 0.0f);
            int i = 0;
            for (; i < mListVerts.Length - 1; i++)
            {
                Gizmos.DrawLine(mListVerts[i].transform.localPosition,
                    mListVerts[i + 1].transform.localPosition);
            }
            Gizmos.DrawLine(mListVerts[i].transform.localPosition,
                mListVerts[0].transform.localPosition);
        }
    }
#endif
}
