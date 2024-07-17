using System;
using UnityEngine;
using UnityEngine.UI;

namespace FantasticCore.Runtime.Modules.Orientation.Elements
{
    [ExecuteAlways, RequireComponent(typeof(GridLayoutGroup))]
    public sealed class OrientationGridElement : OrientationDataElement<SavedGrid, GridLayoutGroup>
    { }

    [Serializable]
    public class SavedGrid : SavedData<GridLayoutGroup>
    {
        #region Fields

        [SerializeField] private RectOffset _padding;
        [SerializeField] private Vector2 _cellSize;
        [SerializeField] private Vector2 _spacing;
        [SerializeField] private GridLayoutGroup.Corner _startCorner;
        [SerializeField] private GridLayoutGroup.Corner _child;
        [SerializeField] private GridLayoutGroup.Axis _startAxis;
        [SerializeField] private GridLayoutGroup.Constraint _constraint;
        [SerializeField] private int _count;

        #endregion
        
        protected override void SaveData(GridLayoutGroup component)
        {
            _padding = component.padding;
            _cellSize = component.cellSize;
            _spacing = component.spacing;
            _startCorner = component.startCorner;
            _startAxis = component.startAxis;
            _constraint = component.constraint;
            _count = component.constraintCount;
        }

        protected override void PutData(GridLayoutGroup component)
        {
            component.padding = _padding;
            component.cellSize = _cellSize;
            component.spacing = _spacing;
            component.startCorner = _startCorner;
            component.startAxis = _startAxis;
            component.constraint = _constraint;
            component.constraintCount = _count;
        }
    }
}