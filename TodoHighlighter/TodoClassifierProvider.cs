using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace De.Keller.VisualStudio.TodoHighlighter
{
    /// <summary>
    /// Adds the <see cref="TodoClassifier"/> to the set of classifiers.
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("text")] // This classifier applies to all text files.
    internal class TodoClassifierProvider : IClassifierProvider
    {
        #region Fields

        [Import]
        private readonly IClassificationTypeRegistryService _classificationRegistry;

        [Import]
        private readonly IClassifierAggregatorService _classifierAggregatorService;

        private static bool _ignoreRequest;

        #endregion

        #region IClassifierProvider

        /// <summary>
        /// Gets a classifier for the given text buffer.
        /// </summary>
        /// <param name="buffer">The <see cref="ITextBuffer"/> to classify.</param>
        /// <returns>A classifier for the text buffer, or null if the provider cannot do so in its current state.</returns>
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (_ignoreRequest)
                return null;

            try
            {
                _ignoreRequest = true;

                return buffer.Properties.GetOrCreateSingletonProperty(() => new TodoClassifier(_classificationRegistry, _classifierAggregatorService.GetClassifier(buffer)));
            }
            finally
            {
                _ignoreRequest = false;
            }
        }

        #endregion
    }
}
