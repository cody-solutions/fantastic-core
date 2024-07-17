using System;
using TMPro;
using UnityEngine;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{ 
    [ExecuteAlways] 
    [RequireComponent(typeof(TMP_Text))]
    public sealed class OrientationTextElement : OrientationDataElement<SavedText,  TMP_Text>
    { }

    [Serializable]
    public class SavedText : SavedData<TMP_Text>
    {
        #region Fields
        
        [Header("Text Properties")]
        [SerializeField] private float _fontSize;
        [SerializeField] private float _minFontSize;
        [SerializeField] private float _maxFontSize;
        
        [SerializeField] private bool _autoSize;
        
        [SerializeField] private Vector4 _margin;
        [SerializeField] private Color _color;
        [SerializeField] private TextAlignmentOptions _textAlignmentOptions;

        #endregion

        protected override void SaveData(TMP_Text tmpText)
        {
            _fontSize = tmpText.fontSize;
            _minFontSize = tmpText.fontSizeMin;
            _maxFontSize = tmpText.fontSizeMax;

            _autoSize = tmpText.enableAutoSizing;

            _margin = tmpText.margin;
            _color = tmpText.color;

            _textAlignmentOptions = tmpText.alignment;
        }

        protected override void PutData(TMP_Text tmpText)
        {
            tmpText.fontSize = _fontSize;
            tmpText.fontSizeMin = _minFontSize;
            tmpText.fontSizeMax = _maxFontSize;

            tmpText.enableAutoSizing = _autoSize;

            tmpText.margin = _margin;
            tmpText.color = _color;

            tmpText.alignment = _textAlignmentOptions;
        }
    }
}