using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace De.Keller.VisualStudio.TodoHighlighter
{
    /// <summary>
    /// Margin's canvas and visual definition including both size and content
    /// </summary>
    internal class TodoScrollBarAnnotationsMargin : Canvas, IWpfTextViewMargin
    {
        #region Constants

        /// <summary>
        /// Margin name.
        /// </summary>
        public const string MarginName = "TodoScrollBarAnnotationMargin";

        #endregion

        #region Fields

        private readonly IVerticalScrollBar _scrollBar;
        private readonly IClassifierAggregatorService _classifierAggregatorService;

        private bool _isDisposed;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="TodoScrollBarAnnotationsMargin"/> class for a given <paramref name="textView"/>.
        /// </summary>
        public TodoScrollBarAnnotationsMargin(
            IWpfTextView textView,
            IWpfTextViewMargin marginContainer,
            IClassifierAggregatorService classifierAggregatorService)
        {
            _classifierAggregatorService = classifierAggregatorService;

            ITextViewMargin scrollBarMargin = marginContainer.GetTextViewMargin(PredefinedMarginNames.VerticalScrollBar);
            _scrollBar = (IVerticalScrollBar)scrollBarMargin;

            textView.LayoutChanged += OnLayoutChanged;
        }

        #region IWpfTextViewMargin

        /// <summary>
        /// Gets the <see cref="Sytem.Windows.FrameworkElement"/> that implements the visual representation of the margin.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                this.ThrowIfDisposed();
                return this;
            }
        }

        #endregion

        #region ITextViewMargin
        /// <summary>
        /// Gets the size of the margin.
        /// </summary>
        /// <remarks>
        /// For a horizontal margin this is the height of the margin,
        /// since the width will be determined by the <see cref="ITextView"/>.
        /// For a vertical margin this is the width of the margin,
        /// since the height will be determined by the <see cref="ITextView"/>.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public double MarginSize
        {
            get
            {
                this.ThrowIfDisposed();

                // Since this is a horizontal margin, its width will be bound to the width of the text view.
                // Therefore, its size is its height.
                return this.ActualHeight;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the margin is enabled.
        /// </summary>
        /// <exception cref="ObjectDisposedException">The margin is disposed.</exception>
        public bool Enabled
        {
            get
            {
                this.ThrowIfDisposed();

                // The margin should always be enabled
                return true;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITextViewMargin"/> with the given <paramref name="marginName"/> or null if no match is found
        /// </summary>
        /// <param name="marginName">The name of the <see cref="ITextViewMargin"/></param>
        /// <returns>The <see cref="ITextViewMargin"/> named <paramref name="marginName"/>, or null if no match is found.</returns>
        /// <remarks>
        /// A margin returns itself if it is passed its own name. If the name does not match and it is a container margin, it
        /// forwards the call to its children. Margin name comparisons are case-insensitive.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="marginName"/> is null.</exception>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return string.Equals(marginName, TodoScrollBarAnnotationsMargin.MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        /// <summary>
        /// Disposes an instance of <see cref="TodoScrollBarAnnotationsMargin"/> class.
        /// </summary>
        public void Dispose()
        {
            if (!this._isDisposed)
            {
                GC.SuppressFinalize(this);
                this._isDisposed = true;
            }
        }

        #endregion

        #region Private methods

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            var classifier = _classifierAggregatorService.GetClassifier(e.NewSnapshot.TextBuffer);

            // Clear all annotations
            Children.Clear();

            // Evaluate all lines and create a annotation if needed
            // TODO: Improve performance by only evaluating the changed lines
            foreach (var line in e.NewSnapshot.Lines)
            {
                var textSpan = new SnapshotSpan(line.Snapshot, line.Start, line.Length);
                var classifications = classifier.GetClassificationSpans(textSpan);

                if (!classifications.Any(x => x.ClassificationType.IsOfType(ClassificationTypeNames.TodoComment)))
                    continue;

                // Create annotation (absolute values are based on the default breakpoint marker)
                var rect = new Rectangle();
                MapLineToPixels(line, out var firstLineTop, out var bottom);
                SetTop(rect, firstLineTop + 3);
                SetLeft(rect, 1);
                rect.Height = 6;
                rect.Width = 6;
                rect.IsHitTestVisible = false;
                Color color = new TodoClassifierFormat().BackgroundColor ?? Colors.Transparent;
                rect.Fill = new SolidColorBrush(color);

                Children.Add(rect);
            }
        }

        private void MapLineToPixels(ITextSnapshotLine line, out double top, out double bottom)
        {
            double mapTop = _scrollBar.Map.GetCoordinateAtBufferPosition(line.Start) - 0.5;
            double mapBottom = _scrollBar.Map.GetCoordinateAtBufferPosition(line.End) + 0.5;
            top = Math.Round(_scrollBar.GetYCoordinateOfScrollMapPosition(mapTop)) - 2.0;
            bottom = Math.Round(_scrollBar.GetYCoordinateOfScrollMapPosition(mapBottom)) + 2.0;
        }

        /// <summary>
        /// Checks and throws <see cref="ObjectDisposedException"/> if the object is disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this._isDisposed)
            {
                throw new ObjectDisposedException(MarginName);
            }
        }

        #endregion
    }
}
