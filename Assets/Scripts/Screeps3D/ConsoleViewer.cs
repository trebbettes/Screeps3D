using System;
using System.IO;
using UnityEngine;
using Screeps3D;
using Screeps_API;
using Unity_Console;
using System.Text.RegularExpressions;

namespace Screeps3D
{
    public class ConsoleViewer : MonoBehaviour
    {
        [SerializeField] private UnityConsole _console;

        private void Start()
        {
            _console.OnInput += OnInput;
            ScreepsAPI.Console.OnConsoleMessage += OnMessage;
            ScreepsAPI.Console.OnConsoleError += OnError;
            ScreepsAPI.Console.OnConsoleResult += OnResult;
            
            GameManager.OnModeChange += OnModeChange;
        }

        private void OnModeChange(GameMode mode)
        {
            _console._panel.SetVisibility(mode != GameMode.Login);
        }

        private void OnInput(string obj)
        {
            ScreepsAPI.Console.Input(obj);
        }

        private void OnMessage(string obj)
        {
            
            bool regexMatch = false;
            // TODO: obj should be a data structure with "parsed data" and the sanitisized message
            // find font tag
            // <font color=\"#ff5555\" type=\"highlight\">console test string</font>
            string fontPattern = @"<font (?:color=\""(?<color>.+?)\"")?\s*?(?:type=\""(?<type>.+?)\"")?\s*?>(?<text>.+)<\/font>";
            foreach (Match m in Regex.Matches(obj, fontPattern))
            {
                regexMatch = true;
                var color = m.Groups["color"].Value; 
                var type = m.Groups["type"].Value; 
                var text = m.Groups["text"].Value;

                Color messageColor;
                ColorUtility.TryParseHtmlString(color, out messageColor);
                
                PrintMessage(string.Format("[{0}] {1}", type, text), messageColor);
            }

            // find room link
            // <a href=\"#!/room//W2N2\">[W2N2 17,24]</a>
            string roomLinkPattern = @"<a href=\""#!\/room\/(?<shard>.*?)\/(?<room>.+)\"">(?<text>.+)<\/a>";
            foreach (Match m in Regex.Matches(obj, roomLinkPattern))
            {
                regexMatch = true;

                // #!/room/botarena/W2N2
                var shard = m.Groups["shard"].Value; // botarena
                var room = m.Groups["room"].Value; // W2N2
                var text = m.Groups["text"].Value; // [W2N2 17,24]
                // TODO: extract coordinates
                // What if they put a font tag inside a link tag? ....
                // TODO: make links clickable

                PrintMessage(string.Format("[{0}/{1}] {2} ", shard, room, text), Color.white);
            }

            if (!regexMatch)
            {
                PrintMessage(obj, Color.white);
            }
        }

        private void OnError(string obj)
        {
            PrintMessage(obj, Color.red);
        }

        private void OnResult(string obj)
        {
            PrintMessage(obj, Color.green);
        }

        private void PrintMessage(string message, Color color)
        {
            var reader = new StringReader(message);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                _console.AddMessage(line, color);
            }
        }
    }
}