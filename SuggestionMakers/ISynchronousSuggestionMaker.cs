using System.Collections.Generic;

namespace SuggestionMakers
{
    /// <summary>
    /// Synchonous interface to a suggestion maker.
    /// </summary>
    public interface ISynchronousSuggestionMaker
    {
        /// <summary>
        /// Request a sequence of suggestions based on <paramref name="start"/>.
        /// </summary>
        /// <param name="start">A common prefix for the suggestions to be returned. The 
        /// IsTokenized property of the suggestion text will be true.</param>
        /// <param name="desiredCount">The desired number of suggestions to be returned 
        /// from the function. The client is likely to try and retrieve at least this 
        /// number of suggestions and may attempt to retrieve additional ones.</param>
        /// <returns>A enumeration of <see cref="SuggestionText"/> suggestions. The 
        /// returned suggestions should be suffixes to the <paramref name="start"/> text, 
        /// so if the input parameter is "Hello" an enumerated return may be " world" 
        /// (with a leading space). The client will cope with enumerated 
        /// <see cref="SuggestionText"/> values that have false IsTokenized 
        /// properties.</returns>
        /// <remarks>A <paramref name="start"/> value of 
        /// <see cref="SuggestionText.Empty"/> is used when starting a new 
        /// sentence.</remarks>
        IEnumerable<SuggestionText> CreateCompletionSuggestions(in SuggestionText start, 
            in int desiredCount);

        /// <summary>
        /// A hint given to the <see cref="ISynchronousSuggestionMaker"/> implementation
        /// that the given text was used by the user.
        /// </summary>
        /// <param name="text">The text the user accepted.</param>
        /// <remarks>The implementator is free to ignore this text or may record it as an
        /// example of the kind of text the user accepts.
        /// <para>The implementation may ignore this call.</para></remarks>
        void RecordAcceptedSuggestionHint(in SuggestionText text);

        /// <summary>
        /// A hint given to the <see cref="ISynchronousSuggestionMaker"/> implementation
        /// of a piece of text to which the user is likely to repsond.
        /// </summary>
        /// <param name="text">The text the user is likely to respond to.</param>
        /// <remarks>The suggestions may be being used as part of a two- or multi-way 
        /// conversation. If the other side or sides of the conversation are known to the
        /// client, it will used this call to provide the 
        /// <see cref="ISynchronousSuggestionMaker"/> of that context.
        /// <para>The implementation may ignore this call.</para></remarks>
        void RecordExternalTextHint(in SuggestionText text);
    }
}
