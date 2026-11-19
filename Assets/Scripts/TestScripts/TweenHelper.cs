using System.Data.SqlTypes;
using System.Net.NetworkInformation;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public static Sequence CameraBob(Transform _transform)
    {
        Sequence sq = DOTween.Sequence();

        sq.Append(_transform.DOLocalMoveY(1f, 0.1f));
        sq.Append(_transform.DOLocalMoveY(-1f, 0.1f));
        return sq;
    }

    public static Sequence FailTextTween(TMP_Text _text)
    {
        Sequence sq = DOTween.Sequence();
        sq.Append(_text.DOColor(Color.red, 1f));
        sq.Append(_text.DOColor(Color.red, 0.5f));
        sq.Append(_text.DOColor(new Color(1f, 0f, 0f, 0f), 1f));
        return sq;
    }

    public static Sequence CompleteTextTween(TMP_Text _text)
    {
        Sequence sq = DOTween.Sequence();
        sq.Append(_text.DOColor(new Color(0.42f, 0.81f, 0.23f, 1f), 1f));
        sq.Append(_text.DOColor(new Color(0.42f, 0.81f, 0.23f, 1f), 0.5f));
        sq.Append(_text.DOColor(new Color(0.42f, 0.81f, 0.23f, 0f), 1f));
        return sq;
    }

    public static Sequence EscortBobTween(Transform _transform)
    {
        float startY = _transform.localPosition.y;

        Sequence sq = DOTween.Sequence();
        sq.Append(_transform.DOLocalMoveY(startY + 0.5f, 1f).SetEase(Ease.InOutSine));
        sq.Append(_transform.DOLocalMoveY(startY, 1f).SetEase(Ease.InOutSine));
        sq.SetLoops(-1, LoopType.Restart);
        sq.SetLink(_transform.gameObject);
        return sq;
    }

    public static Sequence PuddleCleanTween(Transform _transform)
    {
        Sequence sq = DOTween.Sequence();
        sq.Append(_transform.DOScale(1.1f, 0.3f));
        sq.Append(_transform.DOScale(0f, 0.5f));
        
        return sq;
    }
}
