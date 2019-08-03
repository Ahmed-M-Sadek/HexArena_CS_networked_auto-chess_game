using ASU2019_NetworkedGameWorkshop.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows.Forms;
using System.Windows.Media;

namespace ASU2019_NetworkedGameWorkshop.controller
{
    class SoundManager
    {
        private const string PATH_SOUNDS_FOLDER = "Sounds/";
        private const string FILE_EXT = ".wav";
        private static Dictionary<string, List<MediaPlayer>> soundFiles;

        static SoundManager()
        {
            soundFiles = new Dictionary<string, List<MediaPlayer>>();
            Directory.CreateDirectory(PATH_SOUNDS_FOLDER);
            List<string> filenames = new List<string>(Array.ConvertAll(new DirectoryInfo(PATH_SOUNDS_FOLDER).GetFiles("*.wav"), file => file.Name));
            ResourceManager resourceManager = new ResourceManager(typeof(Resources));

            ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            foreach (DictionaryEntry entry in resourceSet)
            {
                if (entry.Value is UnmanagedMemoryStream unmanagedMemoryStream)
                {
                    string fileName = entry.Key.ToString();
                    if (!filenames.Contains(fileName+ FILE_EXT))
                    {
                        using (FileStream fileStream = File.Create(PATH_SOUNDS_FOLDER + fileName + FILE_EXT))
                        {
                            unmanagedMemoryStream.CopyTo(fileStream);
                        }
                    }
                }
            }
        }

        private static void LoadSound(string str)
        {
            if (!soundFiles.ContainsKey(str))
            {
                soundFiles.Add(str, new List<MediaPlayer>());
                MediaPlayer tmp = new MediaPlayer();
                tmp.Open(new Uri(str, UriKind.Relative));
                soundFiles[str].Add(tmp);
            }
        }
        private static MediaPlayer GetAvailabilPlayer(string str)
        {
            foreach (MediaPlayer mp in soundFiles[str])
            {
                if (mp.Position.Equals(TimeSpan.Zero))
                {
                    return mp;
                }
            }
            MediaPlayer tmp = new MediaPlayer();
            tmp.Open(new Uri(str, UriKind.Relative));
            soundFiles[str].Add(tmp);
            return tmp;
        }
        public static void PlaySound(string fileName)
        {
            string path = PATH_SOUNDS_FOLDER + fileName;
            LoadSound(path);
            MediaPlayer mediaPlayer = GetAvailabilPlayer(path);
            mediaPlayer.Play();
        }
        public static void StopAll()
        {
            foreach (var pair in soundFiles)
            {
                StopPath(pair.Key);
            }
        }
        public static void Stop(string fileName)
        {
            string str = PATH_SOUNDS_FOLDER + fileName;
            StopPath(str);
        }
        private static void StopPath(string path)
        {
            foreach (MediaPlayer mp in soundFiles[path])
            {
                if (mp.IsBuffering)
                {
                    mp.Stop();
                }
            }
        }
    }
}
