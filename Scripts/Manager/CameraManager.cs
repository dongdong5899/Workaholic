using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    static public CameraManager Instance;

    private GameObject _camera;
    private CinemachineVirtualCamera _vCam;
    private CinemachineBasicMultiChannelPerlin _cbmp;

    private Sequence _shakeSeq;
    private Sequence _rotateSeq;
    private Sequence _zoomSeq;


    private void Awake()
    {
        Instance = this;

        _camera = GameManager.Instance.Player.transform.Find("Camera").gameObject;
        _vCam = _camera.transform.Find("PlayerVCam").GetComponent<CinemachineVirtualCamera>();
        _cbmp = _vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        _camera.transform.localScale = GameManager.Instance.Player.transform.localScale;
    }

    public void CameraShake(float amplitude, float frequency, float time)
    {
        if (_cbmp.m_AmplitudeGain < amplitude || _cbmp.m_FrequencyGain < frequency)
        {
            if (_shakeSeq != null && _shakeSeq.IsActive()) _shakeSeq.Kill();
            _shakeSeq = DOTween.Sequence();
            _shakeSeq.Append(DOTween.To(() => amplitude, value => _cbmp.m_AmplitudeGain = value, 0f, time));
            _shakeSeq.Join(DOTween.To(() => frequency, value => _cbmp.m_FrequencyGain = value, 0f, time));
        }
    }

    public void CameraRotate(float rotate, float time, Ease ease)
    {
        if (time == 0)
        {
            _vCam.transform.eulerAngles = new Vector3(0, 0, rotate);
        }
        else
        {
            if (_rotateSeq != null && _shakeSeq.IsActive()) _shakeSeq.Kill();
            _rotateSeq = DOTween.Sequence();
            _rotateSeq.Append(_vCam.transform.DORotate(new Vector3(0, 0, rotate), time).SetEase(ease));
        }
    }
    public void CameraRotate(float rotate, float time)
    {
        if (time == 0)
        {
            _vCam.transform.eulerAngles = new Vector3(0, 0, rotate);
        }
        else
        {
            if (_rotateSeq != null && _shakeSeq.IsActive()) _shakeSeq.Kill();
            _rotateSeq = DOTween.Sequence();
            _rotateSeq.Append(_vCam.transform.DORotate(new Vector3(0, 0, rotate), time, RotateMode.Fast));
        }
    }

    public void CameraZoom(float size, float time, Ease ease)
    {
        if (time == 0)
        {
            _vCam.m_Lens.OrthographicSize = size;
        }
        else
        {
            if (_zoomSeq != null && _shakeSeq.IsActive()) _shakeSeq.Kill();
            _zoomSeq = DOTween.Sequence();
            _zoomSeq.Append(DOTween.To(() => _vCam.m_Lens.OrthographicSize, value => _vCam.m_Lens.OrthographicSize = value, size, time).SetEase(ease));
        }
    }
}
