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
    [BepInPlugin(GUID: "MozaicFree", Name: "Kanro.MozaicFree", Version: "0.1")]
    public class MozaicFree : BaseUnityPlugin
    {
        private string DisabledObjectsPath { get; } = @"BepInEx/objects/disabled.txt";
        private string EnabledObjectsPath { get; } = @"BepInEx/objects/enabled.txt";
        private List<string> DisabledObjectList { get; set; } = new List<string>();
        private List<string> EnabledObjectList { get; set; } = new List<string>();
        public void Start()
        {
            UpdateDisabledObjectList();
        }

        public void Update()
        {
            UpdateDisabledObjectList();
            DisableObjects();
        }

        private void UpdateDisabledObjectList()
        {
            if (!File.Exists(DisabledObjectsPath))
            {
                return;
            }
            try
            {
                DisabledObjectList = File.ReadAllLines(DisabledObjectsPath).ToList();
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, ex.ToString());
            }
        }

        private void UpdateEnabledObjectList()
        {
            if (!File.Exists(EnabledObjectsPath))
            {
                return;
            }
            try
            {
                EnabledObjectList = File.ReadAllLines(EnabledObjectsPath).ToList();
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, ex.ToString());
            }
        }

        private void DisableObjects()
        {
            try
            {
                foreach (string objectPath in DisabledObjectList)
                {
                    var gameObject = GameObject.Find(objectPath);
                    if (gameObject != null && gameObject.activeSelf)
                    {
                        LogLine(LogLevel.Debug, "Disabling " + objectPath);
                        gameObject.SetActive(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, ex.ToString());
            }
        }

        private void EnableObjects()
        {
            try
            {
                foreach (string objectPath in EnabledObjectList)
                {
                    String requiredObjectPath = null;
                    String enabledObjectPath = null;
                    var state = true;

                    if (objectPath.Contains("?:"))
                    {
                        var paths = objectPath.Split(new[] { "?:" }, StringSplitOptions.RemoveEmptyEntries);
                        if(paths.Length != 2)
                        {
                            continue;
                        }
                        requiredObjectPath = paths[0];
                        enabledObjectPath = paths[1];
                    }
                    else
                    {
                        enabledObjectPath = objectPath;
                    }

                    if(requiredObjectPath != null)
                    {
                        var requiredObject = GameObject.Find(requiredObjectPath);
                        state = requiredObject?.activeSelf ?? false;
                    }

                    var gameObject = GameObject.Find(enabledObjectPath);
                    if (gameObject != null)
                    {
                        LogLine(LogLevel.Debug, "Enabling " + enabledObjectPath);
                        gameObject.SetActive(state);
                    }
                }
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, ex.ToString());
            }
        }

        private void Log(LogLevel level, string text)
        {
            Logger.Log(level, $"[MozaicFree] {text}");
        }

        private void LogLine(LogLevel level, string text)
        {
            Logger.Log(level, $"[MozaicFree] {text}{Environment.NewLine}");
        }
    }
}