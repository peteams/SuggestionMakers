using System.Collections.Generic;
using System.Diagnostics;

namespace SuggestionMakers
{
    /// <summary>
    /// Container for text that forms part of a conversation.
    /// </summary>
    public struct SuggestionText
    {
        private readonly string _text;
        private readonly int[] _tokenEnds;

        private SuggestionText(in string text, in int[] tokenEnds)
        {
            _text = text;
            _tokenEnds = tokenEnds;
        }

        /// <summary>
        /// Create an untokenized <see cref="SuggestionText"/>.
        /// </summary>
        /// <param name="text">The text to be used.</param>
        /// <returns>A <see cref="SuggestionText"/> that has <see cref="IsTokenized"/> 
        /// false.</returns>
        public static SuggestionText CreateUntokenized(in string text)
        {
            Debug.Assert(text is object);

            var suggestionText = new SuggestionText(text, null);
            return suggestionText;
        }

        /// <summary>
        /// Create a tokenized <see cref="SuggestionText"/>.
        /// </summary>
        /// <param name="text">The text to be used.</param>
        /// <param name="tokenEnds">An non-zero integers that mark the end of each token 
        /// within the string. The last item in the array should equal the length of the 
        /// string.</param>
        /// <returns>A <see cref="SuggestionText"/> that has <see cref="IsTokenized"/> 
        /// false.</returns>
        public static SuggestionText CreateTokenized(in string text, in int[] tokenEnds)
        {
            AssertValid(text, tokenEnds);

            var suggestionText = new SuggestionText(text, tokenEnds);
            return suggestionText;
        }

        /// <summary>
        /// An empty tokenize instance of <see cref="SuggestionText"/>.
        /// </summary>
        public static SuggestionText Empty { get; } = 
            new SuggestionText(string.Empty, new[] { 0 });

        /// <summary>
        /// The text contained in this object.
        /// </summary>
        public string Text => _text;

        /// <summary>
        /// True when the contained text has stored token end markers.
        /// </summary>
        public bool IsTokenized => _tokenEnds is object;

        /// <summary>
        /// If <see cref="IsTokenized"/> is true, the token end values passed to 
        /// <see cref="CreateTokenized(in string, in int[])"/>.
        /// </summary>
        public IReadOnlyList<int> TokenEnds => _tokenEnds;

        [Conditional("DEBUG")]
        private static void AssertValid(in string text, in int[] tokenEnds)
        {
            Debug.Assert(text is object);
            Debug.Assert(tokenEnds is object);

            var prevEnd = 0;
            foreach (var thisEnd in tokenEnds)
            {
                Debug.Assert(prevEnd < thisEnd);
                prevEnd = thisEnd;
            }

            Debug.Assert(prevEnd == text.Length);
        }
    }
}
