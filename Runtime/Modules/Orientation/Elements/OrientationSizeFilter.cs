using System;
using UnityEngine;
using UnityEngine.UI;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{
    [ExecuteAlways]
    [RequireComponent(typeof(ContentSizeFitter))]
    public class OrientationSizeFilter : OrientationDataElement<SavedSizeFilter, ContentSizeFitter>
    { }

    [Serializable]
    public class SavedSizeFilter : SavedData<ContentSizeFitter>
    {
        #region Fields

        [Header("Size Filter Properties")]
        [SerializeField] private ContentSizeFitter.FitMode _horizontalFit;
        [SerializeField] private ContentSizeFitter.FitMode _verticalFit;

        #endregion

        protected override void SaveData(ContentSizeFitter contentSizeFitter)
        {
            _horizontalFit = contentSizeFitter.horizontalFit;
            _verticalFit = contentSizeFitter.verticalFit;
        }

        protected override  void PutData(ContentSizeFitter contentSizeFitter)
        {
            contentSizeFitter.horizontalFit = _horizontalFit;
            contentSizeFitter.verticalFit = _verticalFit;
        }
    }
}