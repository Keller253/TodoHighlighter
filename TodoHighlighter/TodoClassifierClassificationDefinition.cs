using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;

namespace De.Keller.VisualStudio.TodoHighlighter
{
    /// <summary>
    /// Classification type definition export for TodoClassifier
    /// </summary>
    internal static class TodoClassifierClassificationDefinition
    {
        // This disables "The field is never used" compiler's warning. Justification: the field is used by MEF.
#pragma warning disable 169

        /// <summary>
        /// Defines the "TodoClassifier" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.TodoComment)]
        private static ClassificationTypeDefinition typeDefinition;

#pragma warning restore 169
    }
}
