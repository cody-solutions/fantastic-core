using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using FantasticCore.Runtime.Debug;
using FantasticCore.Runtime.Modules.Orientation.Data;

namespace FantasticCore.Runtime.Modules.Orientation
{
    public static class OrientationUtils
    {
        #region Fields

        private static OrientationModuleConfig _cacheConfig;
        private static OrientationSettings _defaultSettings;

        #endregion
        
        public static T GetCopyOf<T>(T comp, T other) where T : Component
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
                                              BindingFlags.Default | BindingFlags.DeclaredOnly;
            Type type = comp.GetType();
            if (type != other.GetType())
            {
                return null;
            }

            var derivedTypes = new List<Type>();
            Type derived = type.BaseType;
            while (derived != null)
            {
                if (derived == typeof(MonoBehaviour))
                {
                    break;
                }

                derivedTypes.Add(derived);
                derived = derived.BaseType;
            }

            if (type == typeof(GridLayoutGroup))
            {
                GridCopy(comp as GridLayoutGroup, other as GridLayoutGroup);
            }
            else
            {
                NormalCopy(type, derivedTypes, bindingFlags, comp, other);
            }

            return comp;
        }

        //The Grid Layout stops working when copying in the normal.
        private static void GridCopy(GridLayoutGroup comp, GridLayoutGroup other)
        {
            comp.spacing = other.spacing;
            comp.constraint = other.constraint;
            comp.cellSize = other.cellSize;
            comp.startAxis = other.startAxis;
            comp.startCorner = other.startCorner;
            comp.childAlignment = other.childAlignment;
            comp.constraintCount = other.constraintCount;
            comp.padding = other.padding;
        }

        private static void NormalCopy<T>(Type type, List<Type> derivedTypes, BindingFlags bindingFlags, T comp, T other) where T : Component
        {
            IEnumerable<PropertyInfo> pinfos = type.GetProperties(bindingFlags);
            foreach (Type derivedType in derivedTypes)
            {
                pinfos = pinfos.Concat(derivedType.GetProperties(bindingFlags));
            }

            pinfos = from property in pinfos
                where !property.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute))
                select property;
            foreach (PropertyInfo pinfo in pinfos)
            {
                if (!pinfo.CanWrite) continue;

                if (pinfos.Any(e => e.Name == $"shared{char.ToUpper(pinfo.Name[0])}{pinfo.Name.Substring(1)}"))
                {
                    continue;
                }

                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch (Exception ex)
                {
                    FantasticDebug.Logger.LogMessage(ex.Message, FantasticLogType.ERROR);
                }
            }

            IEnumerable<FieldInfo> finfos = type.GetFields(bindingFlags);
            foreach (FieldInfo finfo in finfos)
            {
                foreach (Type derivedType in derivedTypes)
                {
                    if (finfos.Any(e => e.Name == $"shared{char.ToUpper(finfo.Name[0])}{finfo.Name.Substring(1)}"))
                    {
                        continue;
                    }

                    finfos = finfos.Concat(derivedType.GetFields(bindingFlags));
                }
            }

            foreach (FieldInfo finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }

            finfos = from field in finfos
                where field.CustomAttributes.Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute))
                select field;
            foreach (FieldInfo finfo in finfos)
            {
                finfo.SetValue(comp, finfo.GetValue(other));
            }
        }

        public static OrientationSettings GetDefaultSettings()
        {
            if (_defaultSettings == null) GenerateDefaultSettings();

            return _defaultSettings;
        }

        private static void GenerateDefaultSettings()
        {
            _defaultSettings = new OrientationSettings()
            {
                DefaultOrientation = true,
                EnabledAutomaticOrientationChange = true,
                RefreshOnSceneLoad = true,
                EnabledAutomaticOnEditor = false
            };
        }

        public static OrientationSettings GetSettings()
        {
            if (_cacheConfig) return _cacheConfig.OrientationSettings;
            if (_defaultSettings != null && Application.isPlaying) return GetDefaultSettings();

            OrientationModuleConfig[] configs = Resources.LoadAll<OrientationModuleConfig>("");
            
            if (configs.Length == 0)
            {
                return GetDefaultSettings();
            }

            _cacheConfig = configs[0];

            return _cacheConfig.OrientationSettings;
        }
    }
}