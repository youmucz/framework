using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace framework.editor
{
    // [AttributeUsage(AttributeTargets.Method)]
    // public class EditorButtonAttribute : PropertyAttribute
    // {
    // }
    //
    // [CustomEditor(typeof(MonoBehaviour), true)]
    // public class EditorButton : Editor
    // {
    //
    //
    //     public override void OnInspectorGUI()
    //     {
    //         base.OnInspectorGUI();
    //
    //         MonoBehaviour monoBehaviour = target as MonoBehaviour;
    //
    //         IEnumerable<MemberInfo> methods = monoBehaviour.GetType()
    //             .GetMembers(BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
    //             .Where(mi => Attribute.IsDefined(mi, typeof(EditorButtonAttribute)));
    //
    //         foreach (MemberInfo memberInfo in methods)
    //         {
    //             if (GUILayout.Button(memberInfo.Name))
    //             {
    //                 MethodInfo method = memberInfo as MethodInfo;
    //                 if (method != null)
    //                 {
    //                     method.Invoke(monoBehaviour, null);
    //                 }
    //             }
    //         }
    //     }
    // }
}
