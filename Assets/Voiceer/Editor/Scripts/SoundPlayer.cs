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

            //ボリューム調整が有効か
            if(VoicePresetSelectorEditor.isVolumeControlEnabled)
            {
                PlaySoundClipExperimental(clip, VoicePresetSelectorEditor.volume);
            }
            else
            {
                PlaySoundClip(clip);
            }
        }

        private static void PlaySoundClip(AudioClip clip)
        {
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

        private static float Decibel2Liner(float db)
        {
            return Mathf.Pow(10f, db/20f);
        }

        private static void PlaySoundClipExperimental(AudioClip clip, float db)
        {
            var voiceerAudio = new GameObject("VoiceerAudio");
            voiceerAudio.hideFlags = HideFlags.HideAndDontSave;
            var audioSource = voiceerAudio.AddComponent<AudioSource>();
            float[] samples = new float[clip.samples * clip.channels];
            clip.GetData(samples, 0);

            var volume = Decibel2Liner(db);
            for (int i = 0; i < samples.Length; ++i)
            {
                samples[i] = samples[i] * volume;
            }

            var adjustedClip = AudioClip.Create("VolumeAdjusted_" + clip.name, clip.samples, clip.channels, clip.frequency, false);
            adjustedClip.SetData(samples, 0);
            audioSource.clip = adjustedClip;
            audioSource.Play();

            EditorApplication.CallbackFunction removeOnPlayed = null;
            removeOnPlayed = () =>
            {
                if(!audioSource.isPlaying) {
                    EditorApplication.update -= removeOnPlayed;
                    UnityEngine.Object.DestroyImmediate(voiceerAudio);
                }
            };
            EditorApplication.update += removeOnPlayed;
        }
    }
}