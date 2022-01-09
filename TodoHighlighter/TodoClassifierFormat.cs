using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace De.Keller.VisualStudio.TodoHighlighter
{
    /// <summary>
    /// Defines an editor format for the TodoClassifier type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = ClassificationTypeNames.TodoComment)]
    [Name(ClassificationTypeNames.TodoComment)]
    [UserVisible(true)] // This should be visible to the end user
    [BaseDefinition(PredefinedClassificationTypeNames.Comment)]
    [Order(After = PredefinedClassificationTypeNames.Comment)]
    internal sealed class TodoClassifierFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TodoClassifierFormat"/> class.
        /// </summary>
        public TodoClassifierFormat()
        {
            DisplayName = "todo comment"; // Human readable version of the name
            BackgroundColor = Color.FromRgb(244, 244, 244);
            BackgroundCustomizable = false;
        }
    }
}
