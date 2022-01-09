using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System;
using System.Collections.Generic;

namespace De.Keller.VisualStudio.TodoHighlighter
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the "TodoClassifier" classification type.
    /// </summary>
    internal class TodoClassifier : IClassifier
    {
        private readonly IClassificationType _classificationType;
        private readonly IClassifier _aggregatedClassifier;

        private bool _isClassificationRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="TodoClassifier"/> class.
        /// </summary>
        /// <param name="registry">Classification registry.</param>
        internal TodoClassifier(IClassificationTypeRegistryService registry, IClassifier aggregatedClassifier)
        {
            _classificationType = registry.GetClassificationType(ClassificationTypeNames.TodoComment);
            _aggregatedClassifier = aggregatedClassifier;
        }

        #region IClassifier

#pragma warning disable 67

        /// <summary>
        /// An event that occurs when the classification of a span of text has changed.
        /// </summary>
        /// <remarks>
        /// This event gets raised if a non-text change would affect the classification in some way,
        /// for example typing /* would cause the classification to change in C# without directly
        /// affecting the span.
        /// </remarks>
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;

#pragma warning restore 67

        /// <summary>
        /// Gets all the <see cref="ClassificationSpan"/> objects that intersect with the given range of text.
        /// </summary>
        /// <remarks>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </remarks>
        /// <param name="span">The span currently being classified.</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification.</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            if (_isClassificationRunning) return new List<ClassificationSpan>();

            try
            {
                _isClassificationRunning = true;
                return Classify(span);
            }
            finally
            {
                _isClassificationRunning = false;
            }
        }

        private IList<ClassificationSpan> Classify(SnapshotSpan span)
        {
            var classificationSpans = new List<ClassificationSpan>();

            if (span.IsEmpty)
                return classificationSpans;

            var aggregatedClassificationSpans = _aggregatedClassifier.GetClassificationSpans(span);
            foreach (var classificationSpan in aggregatedClassificationSpans)
            {
                var text = span.GetText();
                var token = "TODO";
                var tokenIndex = text.IndexOf(token);

                var isComment = classificationSpan.ClassificationType.IsOfType(PredefinedClassificationTypeNames.Comment);
                var containsToken = tokenIndex > 0;

                if (isComment && containsToken)
                {
                    var classifiedSpan = new SnapshotSpan(span.Snapshot, span.Start + tokenIndex, token.Length);
                    classificationSpans.Add(new ClassificationSpan(classifiedSpan, _classificationType));
                }
            }

            return classificationSpans;
        }

        #endregion
    }
}
