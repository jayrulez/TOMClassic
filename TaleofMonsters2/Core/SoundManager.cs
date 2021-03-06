﻿using System;
using System.Collections.Generic;
using System.Threading;
using FMOD;
using NarlonLib.Log;

namespace TaleofMonsters.Core
{
    internal static class SoundManager
    {
        private static Stack<string> bgmHistory;

        private static FMOD.System _fmod = null;
        
        private static Thread soundThread;
        private static Channel _channelBGM = null;//在子线程使用
        private static List<SoundItem> taskList = new List<SoundItem>();//在子线程使用

        struct SoundItem
        {
            public bool IsBGM;
            public byte[] Data;
        }

        public static void Init()
        {
            NLVFS.NLVFS.LoadVfsFile("./SoundResource.vfs");

            Factory.System_Create(out _fmod);
            _fmod.setDSPBufferSize(4096, 2);
            var result = _fmod.init(16, FMOD.INITFLAGS.NORMAL, (IntPtr)null);//16个频道
            if (result != RESULT.OK)
            {
                NLog.Error("fmod SoundManager " + result);
            }
            bgmHistory = new Stack<string>();

            soundThread = new Thread(SoundWork);
            soundThread.Start();
            soundThread.IsBackground = true;
        }

        public static void Play(string dir, string path)
        {
            string filePath = string.Format("Sound.{0}.{1}", dir, path);
            if (!Config.Config.PlayerSound)
            {
                return;
            }

            Play(filePath, false);
        }

        public static void PlayBGM(string path)
        {
            string filePath = string.Format("Bgm.{0}", path);
            if (!Config.Config.PlayerSound)
            {
                return;
            }

            Play(filePath, true);
            bgmHistory.Push(filePath);
        }

        public static void PlayLastBGM()
        {
            if (bgmHistory == null || bgmHistory.Count == 0)
            {
                return;
            }

            bgmHistory.Pop();

            string path = bgmHistory.Peek();
            Play(path, true);
        }

        private static void Play(string path, bool isBGM)
        {
            var file = NLVFS.NLVFS.LoadFile(path);
            if (file == null)
            {
                NLog.Warn("fmod Play file not Found " + path);
                return;
            }

            SoundItem item = new SoundItem {IsBGM = isBGM, Data = file};
            lock (taskList)
            {
                taskList.Add(item);
            }
        }

        private static void SoundWork()
        {
            while (true)
            {
                SoundItem[] itemInfo;
                lock (taskList)
                {
                    itemInfo = new SoundItem[taskList.Count];
                    taskList.CopyTo(itemInfo);
                    taskList.Clear();
                }
                foreach (var task in itemInfo)
                {
                    PlayInThread(task.Data, task.IsBGM);
                }

                Thread.Sleep(50);
            }
        }

        private static void PlayInThread(byte[] file,bool isBGM)
        {//子线程中的处理
            if (isBGM)
            {
                if (_channelBGM != null)
                {
                    bool isPlaying;
                    _channelBGM.isPlaying(out isPlaying);
                    if (isPlaying)
                    {
                        _channelBGM.stop();
                    }
                }
            }

            var info = new CREATESOUNDEXINFO();
            info.length = (uint)file.Length;
            Sound s;
            var result = _fmod.createSound(file, MODE.OPENMEMORY, ref info, out s);
            if (result != RESULT.OK)
            {
                NLog.Error("fmod createSound " + result);
            }

            Channel channel;
            result = _fmod.playSound(s, null, false, out channel);
            _fmod.update();
            int index;
            channel.getIndex(out index);
            if (result != RESULT.OK)
            {
                NLog.Error("fmod playSound " + result);
            }

            if (isBGM)
            {
                _channelBGM = channel;
            }
        }
    }
}
