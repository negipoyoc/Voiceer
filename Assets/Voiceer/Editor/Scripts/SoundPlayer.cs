using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Voiceer
{
    public static class SoundPlayer
    {
        private static VoicePreset CurrentVoicePreset => VoiceerEditorUtility.GetStorageSelector()?.CurrentVoicePreset;

        public static void PlaySound(Hook hook)
        {
            //VoicePresetがあるか
            if (CurrentVoicePreset == null)
            {
                return;
            }

            //Clipが存在するか
            var clip = CurrentVoicePreset.GetRandomClip(hook);
            if (clip == null)
            {
                return;
            }
　
            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            
            var method = audioUtilClass.GetMethod
            (
                "PlayClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
#if UNITY_2019_2_OR_NEWER
                new System.Type[] { typeof(AudioClip), typeof(int), typeof(bool) },
#else
                new Type[] { typeof(AudioClip) },
#endif
                null
            );

#if UNITY_2019_2_OR_NEWER
            method.Invoke(null, new object[] { clip, 0, false });
#else
            method.Invoke(null, new object[] {clip});
#endif
            
            
            
        }
    }
}