using Xamarin.Forms;

namespace Drinctet.Mobile.Controls
{
    public class MarkdownLabel : Label
    {
        public static readonly BindableProperty MarkdownProperty = BindableProperty.Create(nameof(Markdown),
            typeof(string), typeof(MarkdownLabel), null, BindingMode.OneWay, null, MarkdownPropertyChanged);

        public string Markdown
        {
            get => (string) GetValue(MarkdownProperty);
            set => SetValue(MarkdownProperty, value);
        }

        private static void MarkdownPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var label = (MarkdownLabel) bindable;
            label.FormattedText = MarkdownToFormattedString((string) newvalue);
        }

        private static FormattedString MarkdownToFormattedString(string markdown)
        {
            var result = new FormattedString();
            var i = 0;
            var lastTokenPos = 0;
            var isBold = false;
            var isItalic = false;

            void AddSpan()
            {
                if (lastTokenPos != i)
                {
                    var span = new Span {Text = markdown.Substring(lastTokenPos, i - lastTokenPos)};
                    if (isBold)
                        span.FontAttributes |= FontAttributes.Bold;
                    if (isItalic)
                        span.FontAttributes |= FontAttributes.Italic;

                    result.Spans.Add(span);
                }
            }

            while (markdown.Length > i)
            {
                var c = markdown[i];

                char? nextChar = null;
                if (markdown.Length > i + 1)
                    nextChar = markdown[i + 1];

                switch (c)
                {
                    case '*':
                        AddSpan();

                        if (nextChar == '*')
                        {
                            lastTokenPos = i + 2;
                            isBold = !isBold;
                        }
                        else
                        {
                            lastTokenPos = i + 1;
                            isItalic = !isItalic;
                        }

                        break;
                    case '_':
                        AddSpan();

                        if (nextChar == '_')
                        {
                            lastTokenPos = i + 2;
                            isBold = !isBold;
                        }
                        else
                        {
                            lastTokenPos = i + 1;
                            isItalic = !isItalic;
                        }

                        break;
                    default:
                        i++;
                        continue;
                }
            }

            AddSpan();
            return result;
        }
    }
}