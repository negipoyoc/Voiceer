using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Voiceer
{
    public class SoundPlayer
    {
        public static void PlaySound(AudioClip clip)
        {
            if (clip == null) { return; }

            var unityEditorAssembly = typeof(AudioImporter).Assembly;
            var audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            var method = audioUtilClass.GetMethod
            (
                "PlayClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new Type[] { typeof(AudioClip) },
                null
            );

            method.Invoke(null, new object[] { clip });
        }

    }
}