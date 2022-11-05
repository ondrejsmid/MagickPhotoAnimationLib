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

        public class Vector
        {
            public string TypeKey;
            public Point[] Points;
        }

        public static Vector Load(string imagePath)
        {
            var vector = new Vector();

            var dirPath = Path.GetDirectoryName(imagePath);
            var imageFileName = Path.GetFileNameWithoutExtension(imagePath);
            var vectorFilePath = Path.Combine(dirPath, $"{imageFileName}{VectorFileExtension}");
            if (!File.Exists(vectorFilePath))
            {
                return null;
            }
            var vectorFileContent = File.ReadAllText(vectorFilePath);
            var lines = vectorFileContent.Split(Environment.NewLine);
            vector.TypeKey = lines[0];
            vector.Points = new Point[lines.Length - 1];
            for (int i = 1; i < lines.Length; i ++)
            {
                vector.Points[i - 1] = Point.Parse(lines[i]);
            }
            return vector;
        }
    }
}
