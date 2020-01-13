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
                new Type[] {typeof(AudioClip)},
                null
            );

            method.Invoke(null, new object[] {clip});
        }
    }
}