using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using Point = System.Drawing.Point;

namespace MafiaAutoName
{
    public partial class MainWindow : Window
    {
        protected IList<String> Names;
        private ListViewDragDropManager<String> _dragManager;
        private Font _displayFont;

        public MainWindow()
        {
            InitializeComponent();

            Names = new ObservableCollection<string>();
            NamesListView.ItemsSource = Names;
            NewNameTextBox.Focus();

            _dragManager = new ListViewDragDropManager<string>(NamesListView);

            _displayFont = new Font("Verdana", 20, System.Drawing.FontStyle.Bold);
        }

        private void AddNameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(NewNameTextBox.Text)) return;

            Names.Add(NewNameTextBox.Text);
            NewNameTextBox.Text = "";

            NewNameTextBox.Focus();
        }

        private void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            var image = new Bitmap(@"F:\test.jpg");
            Graphics graphicImage = Graphics.FromImage(image);

            const int rows = 4;
            const int columns = 4;
            const int imageLeftMargin = 128;
            const int imageRightMargin = 128;
            const int imageTopMargin = 52;
            const int imageBottomMargin = 75;
            const int portraitHorizontalMargins = 20;
            const int portraitVerticalMargins = 20;

            int portraitWidth = ((image.Width - imageLeftMargin - imageRightMargin) -
                                 (columns - 1) * portraitHorizontalMargins) / columns;
            int portraitHeight = ((image.Height - imageTopMargin - imageBottomMargin) -
                                  (rows - 1) * portraitVerticalMargins) / rows;

            int columnIndex = 0;
            int rowIndex = 0;

            for (int i = 0; i < Names.Count; i++)
            {
                if (columnIndex + 1 > columns)
                {
                    rowIndex++;
                    columnIndex = 0;
                }

                SizeF size = graphicImage.MeasureString(Names[i], new Font("Verdana", 20, System.Drawing.FontStyle.Bold));

                if (rowIndex + 1 < rows)
                {
                    graphicImage.DrawString(Names[i], _displayFont,
                        new SolidBrush(Color.AntiqueWhite),
                        new Point(
                            columnIndex * portraitWidth + imageLeftMargin + (columnIndex * portraitHorizontalMargins) +
                            portraitWidth -
                            (int)size.Width,
                            rowIndex * portraitHeight + imageTopMargin + (rowIndex * portraitVerticalMargins) + portraitHeight -
                            (int)size.Height));

                    columnIndex++;
                }
                else
                {
                    // zoom.us will center portraits on last row unless there is only one missing then it will continue to follow grid pattern
                    int extraHorizontalMargin = 0;
                    if (rows * columns - Names.Count != 1)
                        extraHorizontalMargin = (rows * columns - Names.Count) * (portraitWidth + portraitHorizontalMargins) / 2;

                    graphicImage.DrawString(Names[i], _displayFont,
                        new SolidBrush(Color.AntiqueWhite),
                        new Point(
                            columnIndex * portraitWidth + imageLeftMargin + (columnIndex * portraitHorizontalMargins) +
                            portraitWidth - (int)size.Width + extraHorizontalMargin,
                            rowIndex * portraitHeight + imageTopMargin + (rowIndex * portraitVerticalMargins) + portraitHeight -
                            (int)size.Height));

                    columnIndex++;
                }
            }

            image.Save(@"F:\output.jpg", ImageFormat.Jpeg);

            image.Dispose();
            graphicImage.Dispose();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (NamesListView.SelectedItem != null)
            {
                Names.Remove((String)NamesListView.SelectedValue);
            }
        }

        private void FontButton_Click(object sender, RoutedEventArgs e)
        {
            var fd = new FontDialog();
            fd.Font = _displayFont;

            System.Windows.Forms.DialogResult dr = fd.ShowDialog();
            if (dr != System.Windows.Forms.DialogResult.Cancel)
            {
                _displayFont = fd.Font;
            }
        }
    }
}