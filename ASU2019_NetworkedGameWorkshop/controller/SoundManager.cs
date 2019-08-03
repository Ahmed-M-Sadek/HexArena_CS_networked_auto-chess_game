using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.IO;
using ASU2019_NetworkedGameWorkshop.Properties;
using System.Reflection;
using Microsoft.DirectX.DirectSound;
using DS = Microsoft.DirectX.DirectSound;
using Microsoft.DirectX;
using System.Windows.Media;
using WMPLib;
using System.Windows.Forms;

namespace ASU2019_NetworkedGameWorkshop.controller {
    class SoundManager {

        private static string PATH = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "../../assets/sounds/");
        private static Dictionary<string, List<MediaPlayer>> soundFiles;

        static SoundManager() {
            soundFiles = new Dictionary<string, List<MediaPlayer>>();
        }

        private static void LoadSound(string str) {
            if (!soundFiles.ContainsKey(str)) {
                soundFiles.Add(str, new List<MediaPlayer>());
                MediaPlayer tmp = new MediaPlayer();
                tmp.Open(new Uri(str));
                soundFiles[str].Add(tmp);
            }
        }
        private static MediaPlayer GetAvailabilPlayer(string str) {
            foreach (MediaPlayer mp in soundFiles[str]) {
                if (mp.Position.Equals(TimeSpan.Zero)) {
                    return mp;
                }
            }
            MediaPlayer tmp = new MediaPlayer();
            tmp.Open(new Uri(str));
            soundFiles[str].Add(tmp);
            return tmp;
        }
        public static void PlaySound(string fileName) {
            string path = PATH + fileName;
            LoadSound(path);
            GetAvailabilPlayer(path).Play();
        }
        public static void StopAll() {
            foreach (var pair in soundFiles) {
                StopPath(pair.Key);
            }
        }
        public static void Stop(string fileName) {
            string str = PATH + fileName;
            StopPath(str);
        }
        private static void StopPath(string path) {
            foreach (MediaPlayer mp in soundFiles[path]) {
                if (mp.IsBuffering) {
                    mp.Stop();
                }
            }
        }
    }
}
