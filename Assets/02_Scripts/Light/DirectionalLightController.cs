using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����Ʈ ���ӿ�����Ʈ�� ȸ������ ������ ���� ����Ŭ�� �߰��ϴ� Ŭ����
/// </summary>
public class DirectionalLightController : MonoBehaviour
{
    // �ð� ���
    [SerializeField] float _timeMultiplier;
    
    // ���� �� �Ϸ� �� ���ΰ�(�Ϸ縦 ����ȭ�ؼ� ���. 24�ð��� 1�� ���ڴ�.)
    [SerializeField] float _initialTime;

    float _normalizedTime;
    Vector3 _euler;

    private void Start()
    {
        _normalizedTime = _initialTime;
        _euler = transform.eulerAngles;
    }

    private void Update()
    {
        // �Ϸ�� 24�ð��̰� 1�ð��� 3600���̴ϱ� �Ϸ��...
        _normalizedTime += (Time.deltaTime * _timeMultiplier) / 3600 / 24;
        // 24�� ������ �ٽ� 0�ú���
        _normalizedTime %= 1;

        // ����ȭ�� �ð��� 360���� ���ؼ� ������ ��ȯ
        float angle = _normalizedTime * 360.0f;
        // ���� �غ��� Light�� -90���϶� 0��ó�� ��Ӵ���
        _euler.x = angle - 90.0f;
        // ����Ʈ�� ȸ������ ����
        transform.eulerAngles = _euler;
    }

    //[SerializeField] Light _directionalLight;

    //[SerializeField] float _dayDuration = 60f;

    //private void Update()
    //{
    //    Vector3 rotateSpeed = new Vector3(360f / _dayDuration, 0f, 0f) * Time.deltaTime;
    //    _directionalLight.transform.Rotate(rotateSpeed);
    //}
}
