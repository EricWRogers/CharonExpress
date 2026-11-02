using System.Net.NetworkInformation;
using DG.Tweening;
using UnityEngine;

public static class TweenHelper
{
    public static Sequence PunchUp(Transform _transform) 
    {
        Sequence sq = DOTween.Sequence();

        sq.Append(_transform.DOLocalMoveY(1f,1f));

        sq.Join(_transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 0.7f));

        return sq;
    }

    public static Sequence HappyGhost(Transform _transform)
    {
        Sequence sq = DOTween.Sequence();

        sq.Append(_transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));
        sq.Append(_transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f));
        sq.Append(_transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f));
        sq.Append(_transform.DOScale(new Vector3(1.0f, 1.0f, 1.0f), 0.2f));
        
        return sq;
    }
}
