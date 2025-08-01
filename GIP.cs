using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WilliamPersonalMultiTool
{
    public class IntelligenceIntuitionPrompt
    {
        private readonly int _dimensions;
        private readonly int _seedLength;
        private readonly Dictionary<int, string> _state;
        private readonly DataTable _dataset;
        private readonly List<string> _hashHistory = new();

        private static readonly List<string> EmojiList =
        [
            "😀", "😂", "😅", "😊", "😍", "😎", "😜", "🤔", "🤗", "🤩",
            "👍", "🔥", "💯", "🎉", "🚀", "🌟", "⚡", "💡", "🎯", "🏆"
        ];

        public IntelligenceIntuitionPrompt(int dimensions = 1024, int seedLength = 16)
        {
            this._dimensions = dimensions;
            this._seedLength = seedLength;
            this._state = GenerateRandomState();
            this._dataset = CreateRandomDataset();
        }

        private Dictionary<int, string> GenerateRandomState()
        {
            var dict = new Dictionary<int, string>();
            using var rng = RandomNumberGenerator.Create();

            for (int i = 0; i < _dimensions; i++)
            {
                byte[] buffer = new byte[_seedLength];
                rng.GetBytes(buffer);
                dict[i] = BitConverter.ToString(buffer).Replace("-", "").ToLower();
            }

            return dict;
        }

        private DataTable CreateRandomDataset(int numRows = 100, int numFeatures = 10)
        {
            var table = new DataTable();
            var rng = new Random();

            for (int i = 0; i < numFeatures; i++)
                table.Columns.Add($"Feature_{i}", typeof(double));

            for (int row = 0; row < numRows; row++)
            {
                var values = new object[numFeatures];
                for (int col = 0; col < numFeatures; col++)
                    values[col] = rng.NextDouble() * 10.0;
                table.Rows.Add(values);
            }

            return table;
        }

        public (string, Dictionary<int, Dictionary<int, string>>) PreprocessInput(string text)
        {
            var tokens = text.Split(' ');
            var selectedDimensions = SelectActiveDimensions(tokens.Length);

            var transformedTokens = new List<string>();
            for (int i = 0; i < tokens.Length; i++)
            {
                transformedTokens.Add(ApplyTransformation(tokens[i], selectedDimensions[i]));
            }

            return (string.Join(" ", transformedTokens), selectedDimensions);
        }

        private Dictionary<int, Dictionary<int, string>> SelectActiveDimensions(int numTokens)
        {
            var rng = new Random();
            var selected = new Dictionary<int, Dictionary<int, string>>();

            for (int i = 0; i < numTokens; i++)
            {
                int count = rng.Next(1, 11);
                var dims = new Dictionary<int, string>();

                foreach (int dim in _state.Keys.OrderBy(_ => rng.Next()).Take(count))
                {
                    dims[dim] = _state[dim];
                }

                selected[i] = dims;
            }

            return selected;
        }

        private string GetHashDigest(string text, Dictionary<int, string> dimensions)
        {
            var modifiedText = text;
            var rng = new Random();
            int count = 1;
            if (rng.Next(100) < 10) count = 2;
            if (rng.Next(100) < 5) count = 3;

            for (var loop = 0; loop < count; loop++)
            {
                var input = modifiedText + string.Concat(dimensions.Values);

                // 50% chance to link with a prior hash
                if (_hashHistory.Count > 0 && rng.NextDouble() < 0.5)
                {
                    var linkedHash = _hashHistory[rng.Next(_hashHistory.Count)];
                    input += linkedHash;
                }

                using var sha256 = SHA256.Create();
                var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var hashDigest = BitConverter.ToString(hash).Replace("-", "").ToLower();

                var digestLength = rng.Next(100) < 53 ? rng.Next(1, 11) : 7;
                var digest = hashDigest.Substring(0, digestLength);

                // Store digest for potential future linking
                _hashHistory.Add(digest);

                modifiedText += $" [{digest}]";
            }

            return modifiedText;
        }

        private string ApplyTransformation(string text, Dictionary<int, string> dimensions)
        {
            return GetHashDigest(text, dimensions);
        }

        public string GenerateIntuitiveResponse(string text)
        {
            _hashHistory.Clear(); // optional: clear history between calls
            var (preprocessed, _) = PreprocessInput(text);
            return preprocessed;
        }
    }
}
