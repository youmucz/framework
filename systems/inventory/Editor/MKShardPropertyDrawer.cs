// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
//
// namespace Minikit.Inventory.Editor
// {
//     [CustomPropertyDrawer(typeof(Shard))]
//     public class MKShardPropertyDrawer : PropertyDrawer
//     {
//         public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//         {
//             if (property?.boxedValue?.GetType() != null)
//             {
//                 label.text += $" ({property.boxedValue.GetType().Name})";
//             }
//
//             EditorGUI.PropertyField(position, property, label, true);
//         }
//
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             if (property.isExpanded)
//             {
//                 return EditorGUI.GetPropertyHeight(property) + 20f;
//             }
//
//             return EditorGUI.GetPropertyHeight(property);
//         }
//     }
// } // Minikit.Inventory.Editor namespace
