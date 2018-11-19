using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] GameObject _partTemplate;
    [SerializeField] [Range(3, 10)] int _partsAmount = 4;

    [SerializeField] float _smoothing = 10.0f;

    [SerializeField] [Range(1.0f, 30.0f)] float _width = 1.0f, _height = 15.0f;

    [SerializeField] [Range(1.0f, 100.0f)] float spread = 20;

    List<GameObject> _parts;
    List<RectTransform> _partsChildren;

    [SerializeField] LayerMask mask;
    [SerializeField] Color _normalColor, _enemyColor;
    Color _targetColor;

	void Awake ()
    {
        _parts = new List<GameObject>
        {
            _partTemplate
        };

        _partsChildren = new List<RectTransform>();

        if (_partTemplate != null)
        {
            CloneParts();
        }
    }

    void CloneParts()
    {
        float offset = 360 / _partsAmount;
        float targetRotation = 0;

        for (int i = 0; i < _partsAmount; i++)
        {
            var temp = Instantiate(_partTemplate, _partTemplate.transform.position, _partTemplate.transform.rotation);
            temp.transform.position = _partTemplate.transform.position;
            temp.transform.rotation = Quaternion.Euler(0, 0, targetRotation);
            temp.transform.SetParent(_partTemplate.transform.parent);

            _parts.Add(temp);
            _partsChildren.Add(temp.transform.GetChild(0).GetComponent<RectTransform>());
            targetRotation += offset;
        }
        _partTemplate.SetActive(false);
    }
	
	void Update ()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, mask))
        {
            if(hit.transform.tag == "zombie")
            {
                _targetColor = _enemyColor;
            } else if (hit.transform.tag == "human")
            {
                _targetColor = _enemyColor;
            }
            else
            {
                _targetColor = _normalColor;
            }
        }
        else
        {
            _targetColor = _normalColor;
        }

        foreach (var _part in _partsChildren)
        {
            UpdateColor(_part);
            UpdateDimensions(_part);
            UpdateSpread(_part);
        }
	}

    void UpdateColor(RectTransform part)
    {
        part.GetComponentInChildren<Image>().color = _targetColor;
    }

    void UpdateDimensions(RectTransform part)
    {
        part.sizeDelta = new Vector2(_width, _height);
    }

    void UpdateSpread(RectTransform part)
    {
        float targetSpread = spread + _height / 2;
        float currentSpread = part.localPosition.y;
        currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * _smoothing);

        part.localPosition = new Vector2(0, currentSpread);
    }

    public void SetSpread(float targetSpread)
    {
        spread = targetSpread;
    }
}
