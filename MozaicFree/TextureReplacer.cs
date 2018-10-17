using System;
using BepInEx;
using BepInEx.Logging;
using Logger = BepInEx.Logger;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MozaicFree
{
    [BepInPlugin(GUID: "TextureReplacer", Name: "Kanro.TextureReplacer", Version: "0.1")]
    class TextureReplacer : BaseUnityPlugin
    {
        public string ImagesPath { get; } = @"BepInEx/images";
        private readonly Dictionary<string, string> images = new Dictionary<string, string>();
        public void Start()
        {
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            GetImages();
            ReplaceImages();
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            GetImages();
            ReplaceImages();
        }

        private void GetImages()
        {
            if (!Directory.Exists(ImagesPath))
            {
                return;
            }

            try
            {
                Logger.Log(LogLevel.Debug, "Fetching Images...");

                foreach (string file in Directory.GetFiles(ImagesPath))
                {
                    if (Path.GetExtension(file).Equals(".png", StringComparison.OrdinalIgnoreCase))
                    {
                        images.Add(Path.GetFileNameWithoutExtension(file), file);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.ToString());
            }
        }

        private void ReplaceImages()
        {
            UnityEngine.Object[] textures = Resources.FindObjectsOfTypeAll(typeof(Texture2D));

            foreach (UnityEngine.Object t in textures)
            {
                try
                {
                    Texture2D tex = (Texture2D)t;
                    if (images.ContainsKey(t.name))
                    {
                        Logger.Log(LogLevel.Debug, "Replacing Image: " + tex.name);
                        byte[] fileData = File.ReadAllBytes(images[t.name]);
                        tex.LoadImage(fileData);

                    }
                }
                catch (Exception ex)
                {

                    Logger.Log(LogLevel.Error, ex.ToString());
                }
            }

            Resources.UnloadUnusedAssets();
        }
    }
}
