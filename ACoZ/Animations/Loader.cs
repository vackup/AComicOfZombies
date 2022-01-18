using System;
using System.Collections.Generic;
using System.IO;
using ACoZ.Helpers;
using Microsoft.Xna.Framework;

namespace ACoZ.Animations
{
    public static class Loader
    {
        public static T ParseEnum<T>(string value) where T : struct, IConvertible
        {            
            //if (!typeof(T).GetTypeInfo().IsEnum)
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }
            
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value debe ser un valor");
            }

            if (Enum.IsDefined(typeof(T), value))
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            throw new Exception("No se encontro el valor del enum");
        }

        public static Dictionary<int, Rectangle> LoadData<T>(string fileName, int capacity) where T : struct, IConvertible
        {
            var spriteSourceRectangles = new Dictionary<int, Rectangle>(capacity);

            // open a StreamReader to read the index
#if IPHONE
            using (var reader = new StreamReader(File.OpenRead(fileName)))
#else
            using (var reader = new StreamReader(TitleContainer.OpenStream(fileName)))
#endif
            {
                // while we're not done reading...
                while (!reader.EndOfStream)
                {
                    // get a line
                    var line = reader.ReadLine();

                    // split at the equals sign
                    var sides = line.Split('=');

                    // trim the right side and split based on spaces
                    var rectParts = sides[1].Trim().Split(' ');

                    // create a rectangle from those parts
                    var r = new Rectangle(
                       int.Parse(rectParts[0]),
                       int.Parse(rectParts[1]),
                       int.Parse(rectParts[2]),
                       int.Parse(rectParts[3]));

                    var enumType = ParseEnum<T>(sides[0].Trim());

                    // add the name and rectangle to the dictionary
                    //spriteSourceRectangles.Add(enumType.ToInt32(null), r);
                    spriteSourceRectangles.Add(Convert.ToInt32(enumType), r);
                }
            }

            return spriteSourceRectangles;
        }

        public static Dictionary<string, Rectangle> LoadData(string fileName, int capacity)
        {
            var spriteSourceRectangles = new Dictionary<string, Rectangle>(capacity);

            // open a StreamReader to read the index
#if IPHONE
            using (var reader = new StreamReader(File.OpenRead(fileName)))
#else
            using (var reader = new StreamReader(TitleContainer.OpenStream(fileName)))
#endif
            {
                // while we're not done reading...
                while (!reader.EndOfStream)
                {
                    // get a line
                    var line = reader.ReadLine();

                    // split at the equals sign
                    var sides = line.Split('=');

                    // trim the right side and split based on spaces
                    var rectParts = sides[1].Trim().Split(' ');

                    // create a rectangle from those parts
                    var r = new Rectangle(
                       int.Parse(rectParts[0]),
                       int.Parse(rectParts[1]),
                       int.Parse(rectParts[2]),
                       int.Parse(rectParts[3]));

                    // add the name and rectangle to the dictionary
                    spriteSourceRectangles.Add(sides[0].Trim(), r);
                }
            }

            return spriteSourceRectangles;
        }

        public static Dictionary<string, Vector2> LoadDataToVector(string fileName, int capacity)
        {
            var dataVector = new Dictionary<string, Vector2>(capacity);

            // open a StreamReader to read the index
#if IPHONE
            using (var reader = new StreamReader(File.OpenRead(fileName)))
#else
            using (var reader = new StreamReader(TitleContainer.OpenStream(fileName)))
#endif
            {
                // while we're not done reading...
                while (!reader.EndOfStream)
                {
                    // get a line
                    var line = reader.ReadLine();

                    // split at the equals sign
                    var sides = line.Split('=');

                    // trim the right side and split based on spaces
                    var VectorParts = sides[1].Trim().Split(' ');

                    // create a rectangle from those parts
                    var r = new Vector2(int.Parse(VectorParts[0]), int.Parse(VectorParts[1]));

                    // add the name and rectangle to the dictionary
                    dataVector.Add(sides[0].Trim(), r);
                }
            }

            return dataVector;
        }

        public static Rectangle[] LoadData(string animationName, string fileName)
        {
            var spriteSourceRectangles = new List<Rectangle>();

            // open a StreamReader to read the index
#if IPHONE
            using (var reader = new StreamReader(File.OpenRead(fileName)))
#else
            using (var reader = new StreamReader(TitleContainer.OpenStream(fileName)))
#endif
            {
                // while we're not done reading...
                while (!reader.EndOfStream)
                {
                    // get a line
                    var line = reader.ReadLine();

                    // split at the equals sign
                    var sides = line.Split('=');

                    var txtAnimationName = sides[0].Trim();
                    txtAnimationName = txtAnimationName.Substring(0, txtAnimationName.Length - GlobalParameters.ANIMATION_NUMERICAL_CHARS);

                    if (txtAnimationName != animationName) continue;

                    // trim the right side and split based on spaces
                    var rectParts = sides[1].Trim().Split(' ');

                    // add the name and rectangle to the dictionary
                    spriteSourceRectangles.Add(new Rectangle(
                        int.Parse(rectParts[0]),
                        int.Parse(rectParts[1]),
                        int.Parse(rectParts[2]),
                        int.Parse(rectParts[3])));
                }
            }

            return spriteSourceRectangles.ToArray();
        }
    }
}
