using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voiceer
{
    public enum Trigger
    {
        OnCompileEnd = 0,
        OnEnteredPlayMode,
        OnExitingPlayMode,
        OnPlayButHasError,
        OnPreProcessBuild,
        OnPostProcessBuild_Success,
        OnPostProcessBuild_Failed,
        OnSave,
        OnBuildTargetChanged,
    }

    [Serializable]
    public class Sound
    {
        public List<AudioClip> voiceClips = new List<AudioClip>();

        public AudioClip GetRandom()
        {
            if (voiceClips == null || voiceClips.Count == 0)
            {
                return null;
            }
            var rand = UnityEngine.Random.Range(0, voiceClips.Count);
            return voiceClips[rand];
        }
    }

    public class VoicePreset : ScriptableObject
    {
        [Serializable]
        public class Set
        {
            public Trigger trigger;
            public Sound sound;

            public Set(Trigger t, Sound s)
            {
                trigger = t;
                sound = s;
            }
        }

        [Serializable]
        public class Meta
        {
            public string memo;
            public string url;
        }

        public List<Set> voiceSetList = new List<Set>();
        public Meta metaData = new Meta();

        public AudioClip GetRandomClip(Trigger trigger)
        {
            var sound = GetVoiceSet(trigger);
            return sound?.GetRandom();
        }

        public int ClipCount(Trigger trigger)
        {
            var sound = GetInstanceSafety((int)trigger);
            if (sound == null || sound.voiceClips.Count == 0)
            {
                return 0;
            }
            return sound.voiceClips.Count;
        }

        public Sound GetVoiceSet(Trigger trigger)
        {
            foreach (var set in voiceSetList)
            {
                if (trigger == set.trigger)
                {
                    return set.sound;
                }
            }
            return null;
        }

        public Sound GetInstanceSafety(int index)
        {
            return voiceSetList[index].sound;
        }

        public void CopyFrom(VoicePreset p)
        {

            this.voiceSetList = new List<Set>();

            foreach (Trigger trigger in Enum.GetValues(typeof(Trigger)))
            {
                if (GetVoiceSet(trigger) == null)
                {
                    voiceSetList.Add(new Set(trigger, new Sound()));
                }

                if (p.GetVoiceSet(trigger) != null)
                {

                    var sound = new Sound();
                    foreach (var clip in p.GetVoiceSet(trigger).voiceClips)
                    {
                        sound.voiceClips.Add(clip);
                    }
                    voiceSetList.Add(new Set(trigger, sound));
                }
            }


            this.metaData = new Meta();
            this.metaData.memo = p.metaData.memo;
            this.metaData.url = p.metaData.url;
        }

        public void ClearAll()
        {
            voiceSetList.Clear();
            metaData.memo = "";
            metaData.url = "";
        }
    }
}