using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BubbasGameJoltAPI
{
    public class DebugConsole
    {
        public delegate void TextDelegate(string line, int index);

        private static string[] _lines = new string[50];
        private static int _currentLine = 0;

        public static TextDelegate OnNewLine;
        public static TextDelegate OnAppend;

        public static string ReadToEnd()
        {
            int index = 0;
            string lines = "";

            // Add all lines that are not null
            while (_lines[index] != null)
            {
                lines += _lines[index];
                index++;
            }

            //
            if (lines != "")
                lines = lines.Remove(lines.Length - 1);

            // Return lines
            return lines;
        }
        public static string ReadLine(int index)
        {
            return _lines[index];
        }
        public static string[] ReadLines(int start, int count)
        {
            int index = start;
            int end = start + count;
            string[] lines;

            // Add all lines that are not null
            while (_lines[index] != null && index < end)
                index++;

            //
            lines = new string[index];
            for (int i = start; i < index; i++)
                lines[i] = _lines[i];

            // Return lines
            return lines;
        }
        public static string[] ReadAllLines()
        {
            int index = 0;
            string[] lines;

            // Add all lines that are not null
            while (_lines[index] != null)
                index++;

            //
            lines = new string[index];
            for (int i = 0; i < index; i++)
                lines[i] = _lines[i];

            // Return lines
            return lines;
        }

        internal static void Write(string text)
        {
            string[] lines = text.Split('\n');
            int length = lines.Length - 1;

            // Writes all lines but the last
            for (int i = 0; i < length; i++)
                PrivateWriteLine(lines[i]);

            // Writes the last line as Append
            if (length > 0)
                Append(lines[length]);
        }
        internal static void WriteLine(string text)
        {
            // Writes the lines then adds a new one
            Write(text);
            NewLine();
        }

        private static void PrivateWriteLine(string text)
        {
            // Writes on the current line then adds a new one
            Append(text);
            NewLine();
        }
        private static void Append(string text)
        {
            // Adds the text to the current line
            _lines[_currentLine] += text;

            //
            OnAppend(text, _currentLine);
        }
        private static void NewLine()
        {
            // If current line is null, make it and empty string
            if (_lines[_currentLine] == null)
                _lines[_currentLine] = "";

            // Makes the next line the current one
            _currentLine++;

            //
            if (_currentLine >= _lines.Length)
            {
                int jump = _currentLine - (_lines.Length - 1);
                _currentLine = _lines.Length - 1 - jump;

                for (int i = jump; i < _lines.Length; i++)
                    _lines[i - jump] = _lines[i];

                for (int i = _lines.Length - 1 - jump; i < _lines.Length; i++)
                    _lines[i] = null;
            }

            //
            OnNewLine(_lines[_currentLine - 1], _currentLine - 1);
        }
    }
}
