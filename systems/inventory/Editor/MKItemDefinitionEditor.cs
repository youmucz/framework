// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Minikit.Inventory.Internal;
//
// namespace Minikit.Inventory.Editor
// {
//     [CustomEditor(typeof(ItemDefinitionScriptableObject))]
//     public class MKItemDefinitionEditor : UnityEditor.Editor
//     {
//         static int shardTypeOption = 0;
//         static int shardContainerOption = 0;
//
//
//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();
//
//             ItemDefinitionScriptableObject itemDefinition = target as ItemDefinitionScriptableObject;
//
//             EditorGUILayout.Space(20f);
//
//             List<string> nativeShardTypes = ShardReflector.GetNativelyDefinedShardTypes();
//             if (nativeShardTypes.Count == 0)
//             {
//                 nativeShardTypes.Add("Native Shards Not Found!"); // You need to add the [MKNativeShard] attribute to your shards that inherit from Shard
//             }
//
//             List<string> shardContainers = new List<string>() { "Static", "Dynamic" };
//
//             shardTypeOption = EditorGUILayout.Popup("Shard Type:", shardTypeOption, nativeShardTypes.ToArray());
//             shardContainerOption = EditorGUILayout.Popup("Shard Container:", shardContainerOption, shardContainers.ToArray());
//             if (GUILayout.Button($"Add {shardContainers[shardContainerOption]} {nativeShardTypes[shardTypeOption]}"))
//             {
//                 Type shardType = ShardReflector.GetRegisteredShardType(nativeShardTypes[shardTypeOption]);
//                 if (shardType == null)
//                 {
//                     Debug.LogError($"Failed to get valid Type from selected Native Shard Type ({nativeShardTypes[shardTypeOption]})");
//                     return;
//                 }
//
//                 Shard shard = Activator.CreateInstance(shardType) as Shard;
//                 if (shard == null)
//                 {
//                     Debug.LogError($"Failed to create valid Shard from selected Type ({shardType.Name})");
//                     return;
//                 }
//
//                 if (shardContainerOption == 0)
//                 {
//                     itemDefinition.AddStaticShard(shard);
//                 }
//                 else if (shardContainerOption == 1)
//                 {
//                     itemDefinition.AddDynamicShard(shard);
//                 }
//
//                 EditorUtility.SetDirty(itemDefinition);
//             }
//         }
//     }
// } // Minikit.Inventory.Editor namespace
