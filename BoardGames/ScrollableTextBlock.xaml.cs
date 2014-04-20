using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace BoardGames
{
    public partial class ScrollableTextBlock : UserControl
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                "Text",
                typeof(String),
                typeof(ScrollableTextBlock),
                new PropertyMetadata(default(String), OnTextPropertyChanged));

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ScrollableTextBlock source = (ScrollableTextBlock)d;
            string value = (string)e.NewValue;
            source.ParseText(value);
        }

        private static readonly int NEWLINE_CHARACTER_SIZE = 50;
        private static readonly int NEWLINE_CUTOFF_DIFF = 500;

        private int maxTextSize;
        public int MaxTextSize
        {
            get
            {
                return maxTextSize;
            }
            set
            {
                maxTextSize = value;
            }
        }

        public ScrollableTextBlock()
        {
            InitializeComponent();
            MaxTextSize = 1000;
        }

        private int GetTheoreticalTextSize(string value)
        {
            int newlineCount = value.Count(c => c == '\n');
            if (newlineCount == 0)
            {
                return value.Length;
            }
            int newlineSize = (newlineCount - 1) * NEWLINE_CHARACTER_SIZE;
            return value.Length + newlineSize;
        }

        private void ParseText(string value)
        {
            StackPanel.Children.Clear();

            int theoreticalSize = GetTheoreticalTextSize(value);
            if (theoreticalSize < MaxTextSize)
            {
                TextBlock textBlock = new TextBlock()
                {
                    Text = value,
                    TextWrapping = TextWrapping.Wrap
                };
                StackPanel.Children.Add(textBlock);
            }
            else
            {
                string cutString = value;
                int splitIndex = GetSplitIndex(cutString);
                while (splitIndex != -1)
                {
                    string splitString = cutString.Substring(0, splitIndex + 1);

                    TextBlock textBlock = new TextBlock()
                    {
                        Text = splitString,
                        TextWrapping = TextWrapping.Wrap
                    };
                    StackPanel.Children.Add(textBlock);

                    cutString = cutString.Substring(splitIndex + 1, cutString.Length - splitIndex - 1);
                    splitIndex = GetSplitIndex(cutString);
                }

                TextBlock finalTextBlock = new TextBlock()
                {
                    Text = cutString,
                    TextWrapping = TextWrapping.Wrap,
                    TextTrimming = TextTrimming.WordEllipsis
                };
                StackPanel.Children.Add(finalTextBlock);
            }
        }

        private int GetSplitIndex(string value)
        {
            if (value.Length < MaxTextSize)
            {
                return -1;
            }

            int lastNewLine = -1;
            int lastPeriod = -1;
            int charCount = 0;
            int i;
            char[] charArray = value.ToCharArray();
            for (i = 0; i < charArray.Length && charCount < MaxTextSize - 1; i++)
            {
                if (charArray[i] == '.')
                {
                    lastPeriod = i;
                }
                else if (charArray[i]== '\n')
                {
                    lastNewLine = i;
                    charCount += (NEWLINE_CHARACTER_SIZE - 1);
                }
                charCount++;
            }

            int cutoff = i - NEWLINE_CUTOFF_DIFF;
            if (cutoff < 0)
            {
                return lastPeriod;
            }
            if (lastNewLine < cutoff)
            {
                return lastPeriod;
            }

            return lastNewLine;
        }
    }
}
