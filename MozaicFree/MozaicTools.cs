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
    [BepInPlugin(GUID: "MozaicTools", Name: "Kanro.MozaicTools", Version: "0.1")]
    class MozaicTools : BaseUnityPlugin
    {
        private string ObjectsPath { get; } = @"BepInEx/objects/objects.txt";
        private string DumpPath { get; } = @"BepInEx/dump";
        private List<GameObject> disabledObjects = new List<GameObject>();

        public void OnGUI()
        {
            if (Event.current.type == EventType.KeyUp)
            {
                if (Event.current.alt)
                {
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.F11:
                            LogLine(LogLevel.Debug, "Dumping Images..." + Environment.NewLine);
                            try
                            {
                                DumpImages();
                            }
                            catch (Exception ex)
                            {
                                Log(LogLevel.Error, "[Error] " + ex.ToString());
                            }
                            break;
                        case KeyCode.F10:
                            LogLine(LogLevel.Debug, "Dumping Objects..." + Environment.NewLine);
                            try
                            {
                                DumpObjects();
                            }
                            catch (Exception ex)
                            {
                                Log(LogLevel.Error, "[Error] " + ex.ToString());
                            }
                            break;
                        case KeyCode.F9:
                            LogLine(LogLevel.Debug, "Disable Objects..." + Environment.NewLine);
                            try
                            {
                                DisableObjects();
                            }
                            catch (Exception ex)
                            {
                                Log(LogLevel.Error, "[Error] " + ex.ToString());
                            }
                            break;
                        case KeyCode.F8:
                            LogLine(LogLevel.Debug, "Enable Objects..." + Environment.NewLine);
                            try
                            {
                                EnableObjects();
                            }
                            catch (Exception ex)
                            {
                                Log(LogLevel.Error, "[Error] " + ex.ToString());
                            }
                            break;
                    }
                }
            }
        }
        
        private void DumpImages()
        {
            string dumppath = Path.Combine(DumpPath, SceneManager.GetActiveScene().name);

            if (!Directory.Exists(dumppath))
            {
                Directory.CreateDirectory(dumppath);
            }

            UnityEngine.Object[] textures = Resources.FindObjectsOfTypeAll(typeof(Texture2D));

            foreach (UnityEngine.Object t in textures)
            {
                try
                {
                    Texture2D tex = (Texture2D)t;
                    Texture2D tex_readable = GetReadableTexture2D(tex);
                    byte[] bytes = tex_readable.EncodeToPNG();
                    File.WriteAllBytes(Path.Combine(dumppath, t.name + ".png"), bytes);
                    LogLine(LogLevel.Debug, "Dumping " + t.name);
                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, "[Error]" + ex.ToString());
                }
            }

            LogLine(LogLevel.Debug, "Dumppath: " + dumppath);
        }

        private void DumpObjects()
        {
            string dumppath = Path.Combine(DumpPath, SceneManager.GetActiveScene().name);

            if (!Directory.Exists(dumppath))
            {
                Directory.CreateDirectory(dumppath);
            }

            UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(typeof(Material));
            
            try
            {
                File.WriteAllText(Path.Combine(dumppath, "objects.txt"), String.Join("\n", objects.Select(it =>
                {
                    return GetGameObjectPath(it as GameObject);
                }).ToArray()));
                LogLine(LogLevel.Debug, "Dumping Objects");
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, "[Error]" + ex.ToString());
            }

            LogLine(LogLevel.Debug, "Dumppath: " + dumppath);
        }

        private void DisableObjects()
        {
            if (!File.Exists(ObjectsPath))
            {
                return;
            }

            try
            {
                var lines = File.ReadAllLines(ObjectsPath).ToList();

                foreach (var line in lines)
                {
                    GameObject gameObject = GameObject.Find(line);

                    if (gameObject.activeSelf)
                    {
                        LogLine(LogLevel.Debug, $"Disable Object: {gameObject.name}({gameObject.GetInstanceID()})");
                        gameObject.SetActive(false);
                        disabledObjects.Add(gameObject);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, "[Error]" + ex.ToString());
            }
        }

        private void EnableObjects()
        {
            try
            {
                foreach (GameObject gameObject in disabledObjects)
                {
                    LogLine(LogLevel.Debug, $"Enable Object: {gameObject.name}({gameObject.GetInstanceID()})");
                    gameObject.SetActive(true);
                }
                disabledObjects = new List<GameObject>();
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, "[Error]" + ex.ToString());
            }
        }

        private Texture2D GetReadableTexture2D(Texture2D texture)
        {
            RenderTexture tmp = RenderTexture.GetTemporary(
                                texture.width,
                                texture.height,
                                0,
                                RenderTextureFormat.Default,
                                RenderTextureReadWrite.Linear);

            Graphics.Blit(texture, tmp);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = tmp;
            Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
            myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
            myTexture2D.Apply();
            RenderTexture.active = previous;

            RenderTexture.ReleaseTemporary(tmp);

            return myTexture2D;
        }

        private string GetGameObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }

        private void Log(LogLevel level, string text)
        {
            Logger.Log(level, $"[MozaicTools] {text}");
        }

        private void LogLine(LogLevel level, string text)
        {
            Logger.Log(level, $"[MozaicTools] {text}{Environment.NewLine}");
        }
    }
}
