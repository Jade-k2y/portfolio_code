using UnityEngine;
using System;


namespace Studio.Game
{
    [Serializable]
    public class Keyword
    {
        [SerializeField]
        private string[] _tags;


        public float GetSimilarity(string keyword)
        {
            var count = _tags?.Length ?? 0;
            var average = 0f;

            if (0 < count)
            {
                for (var i = 0; i < count; ++i)
                {
                    average += GetSimilarity(_tags[i], keyword);
                }

                average /= count;
            }

            return average;
        }


        private float GetSimilarity(string source, string b)
        {
            var distance = GetLevenshteinDistance(source, b);
            var maxLength = Mathf.Max(source.Length, b.Length);

            return 1f - (float)distance / maxLength;
        }


        private int GetLevenshteinDistance(string a, string b)
        {
            var matrix = new int[a.Length + 1, b.Length + 1];

            for (var i = 0; i <= a.Length; ++i)
            {
                matrix[i, 0] = i;
            }

            for (var k = 0; k <= b.Length; ++k)
            {
                matrix[0, k] = k;
            }

            for (var i = 1; i <= a.Length; ++i)
            {
                for (var k = 1; k <= b.Length; ++k)
                {
                    var cost = (a[i - 1] == b[k - 1]) ? 0 : 1;

                    matrix[i, k] = Mathf.Min(Mathf.Min(matrix[i - 1, k] + 1, matrix[i, k - 1] + 1), matrix[i - 1, k - 1] + cost);
                }
            }

            return matrix[a.Length, b.Length];
        }
    }

    /*
    public class Program
    {
        public static void Main()
        {
            string inputString = "Hello";
            string[] candidateStrings = { "Hallo", "Hell", "Halo", "Hi" };

            foreach (string str in candidateStrings)
            {
                float similarity = StringSimilarity.CalculateStringSimilarity(inputString, str);
                Console.WriteLine("Similarity between '{0}' and '{1}': {2}", inputString, str, similarity);
            }
        }
    }
    */
}