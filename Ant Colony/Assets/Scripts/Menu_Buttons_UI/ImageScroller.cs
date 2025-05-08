using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ImageScroller : MonoBehaviour
{
    private RawImage _image;

    [SerializeField, Range(0, 10)] private float _scrollSpeed = 0.1f;

    [SerializeField, Range(0, 360)] private float _scrollAngle = 194.5f;

    private Vector2 _direction;

    private void Awake()
    {
        _image = GetComponent<RawImage>();
        UpdateDirection();
    }

    private void Update()
    {
        _image.uvRect = new Rect(
            _image.uvRect.position + _direction * _scrollSpeed * Time.deltaTime,
            _image.uvRect.size
        );
    }

    private void OnValidate()
    {
        UpdateDirection();
    }

    private void UpdateDirection()
    {
        float angleRad = _scrollAngle * Mathf.Deg2Rad;
        _direction = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
}
