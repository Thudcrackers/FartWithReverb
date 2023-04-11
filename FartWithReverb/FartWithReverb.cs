using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FartWithReverb
{
    public class FartWithReverb : ModBehaviour
    {
        public static FartWithReverb Instance { get; private set; }
        private void Awake()
        {
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        private void Start()
        {
            Instance = this;
            // Starting here, you'll have access to OWML's mod helper.
            Instance.ModHelper.Console.WriteLine($"My mod {nameof(FartWithReverb)} is loaded!", MessageType.Success);

            GetClips(new string[] { "Assets/fartwithreverb.mp3" });


            // Example of accessing game code.
            LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
            {
                if (loadScene != OWScene.SolarSystem) return;
                ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
                StartCoroutine(PatchAudio());
            };
        
        }

        //Shamelessly stolen from the Funny Noises mod
        private IEnumerator PatchAudio()
        {
            yield return new WaitForSecondsRealtime(1);

            Dictionary<int, AudioLibrary.AudioEntry> dict = ((Dictionary<int, AudioLibrary.AudioEntry>)AccessTools.Field(typeof(AudioManager), "_audioLibraryDict").GetValue(Locator.GetAudioManager()));

            Instance.ModHelper.Console.WriteLine($"Patching game sounds", MessageType.Info);
            Instance.PatchAudioType(dict, AudioType.ShipCockpitEject, "Assets/fartwithreverb.mp3");

            Instance.PatchAudioType(dict, AudioType.ShipThrustIgnition, "Assets/fartwithreverb.mp3");

            Instance.ModHelper.Console.WriteLine($"All sounds patched!", MessageType.Success);
        }

        public Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>();
        private AudioClip GetClip(string name)
        {
            if (Instance.Sounds.ContainsKey(name)) { return Instance.Sounds[name]; }
            AudioClip audioClip = ModHelper.Assets.GetAudio(name);
            Instance.Sounds.Add(name, audioClip);
            return audioClip;
        }
        private AudioClip[] GetClips(string[] names)
        {
            AudioClip[] clips = new AudioClip[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                clips[i] = GetClip(names[i]);
            }
            return clips;
        }

        private void PatchAudioType(Dictionary<int, AudioLibrary.AudioEntry> dict, AudioType type, string[] names)
        {
            AudioLibrary.AudioEntry entry = new AudioLibrary.AudioEntry(type, GetClips(names), 0.5f);
            try
            {
                dict[(int)type] = entry;
            }
            catch
            {
                dict.Add((int)type, entry);
            }
        }
        private void PatchAudioType(Dictionary<int, AudioLibrary.AudioEntry> dict, AudioType type, string name)
        {
            Instance.PatchAudioType(dict, type, new string[] { name });
        }
    }
}