using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class Line : MonoBehaviourPun
{
    [SerializeField] LineRenderer lineRenderer = null;
    [SerializeField] EdgeCollider2D edgeCollider = null;

    List<Vector2> fingerPositions = new List<Vector2>();
    Color color = new Color(0, 0, 0, 1);

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        edgeCollider = GetComponent<EdgeCollider2D>();
    }

    public void SetColor(Color color)
    {
        photonView.RPC(nameof(RPC_SetColor), RpcTarget.All, color.r, color.g, color.b);
    }
    [PunRPC]
    void RPC_SetColor(float r, float g, float b)
    {
        color.r = r;
        color.g = g;
        color.b = b;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void SetPosition(Vector2 Pos)
    {
        photonView.RPC(nameof(RPC_SetPosition), RpcTarget.All, Pos);
    }
    [PunRPC]
    void RPC_SetPosition(Vector2 Pos)
    {
        fingerPositions.Add(Pos);
        fingerPositions.Add(Pos);
        lineRenderer.SetPosition(0, Pos);
        lineRenderer.SetPosition(1, Pos);
        edgeCollider.points = fingerPositions.ToArray();
    }

    public void UpdatePosition(Vector2 addPos)
    {
        photonView.RPC(nameof(RPC_UpdatePosition), RpcTarget.All, addPos);
    }
    [PunRPC]
    void RPC_UpdatePosition(Vector2 addPos)
    {
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, addPos);
        edgeCollider.points = fingerPositions.ToArray();
    }
}
