using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using Point = System.Windows.Point;

namespace VectorDesigner
{
    public static class VectorLoader
    {
        public const string VectorFileExtension = ".vector";

        public static readonly Dictionary<string, string[]> VectorTypes = new Dictionary<string, string[]>
        {
            { "Limb", new string[] { "Pivot", "Tail" } },
            { "LimbTest", new string[] { "Center", "TopLeft", "BottomRight", "Pivot", "Tail", "Pivot2", "Tail2" } }
        };

        public static Vector Load(string imgPath)
        {
            var vector = new Vector();

            var dirPath = Path.GetDirectoryName(imgPath);
            var imageFileName = Path.GetFileNameWithoutExtension(imgPath);
            var vectorFilePath = Path.Combine(dirPath, $"{imageFileName}{VectorFileExtension}");
            if (!File.Exists(vectorFilePath))
            {
                return null;
            }
            var vectorFileContent = File.ReadAllText(vectorFilePath);
            var lines = vectorFileContent.Split(Environment.NewLine);
            vector.TypeKey = lines[0];
            vector.Points = new Point[lines.Length - 1];
            for (int i = 1; i < lines.Length; i++)
            {
                vector.Points[i - 1] = Point.Parse(lines[i]);
            }
            return vector;
        }

        public static Vector LoadAsPixelShiftFromCenter(string imgPath, int targetImgWidth, int targetImgHeight)
        {
            var vector = Load(imgPath);
            vector.Points = vector.Points.Select(p => new Point((p.X - 0.5) * targetImgWidth, (p.Y - 0.5) * targetImgHeight)).ToArray();
            return vector;
        }
    }
}
