using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using Point = System.Drawing.Point;

namespace MafiaAutoName
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        protected IList<String> Names;
        private ListViewDragDropManager<String> _dragManager;
        private Font _displayFont;

        private string _outputFilePath = "";
        private string _inputImagePath;
        private int _rows;
        private int _columns;
        private int _imageLeftMargin;
        private int _imageRightMargin;
        private int _imageTopMargin;
        private int _imageBottomMargin;
        private int _portraitHorizontalMargins;
        private int _portraitVerticalMargins;
        private int _newImageHorizontalResolution;
        private int _newImageVerticalResolution;
        private Color _fontColor;

        public string OutputImagePath
        {
            get { return _outputFilePath;  }
            set
            {
                _outputFilePath = value;
                OnPropertyChanged("OutputImagePath");
            }
        }

        public string InputImagePath
        {
            get { return _inputImagePath; }
            set
            {
                _inputImagePath = value;
                OnPropertyChanged("InputImagePath");
            }
        }

        public int Rows
        {
            get { return _rows; }
            set
            {
                _rows = value;
                OnPropertyChanged("Rows");
            }
        }

        public int Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                OnPropertyChanged("Columns");
            }
        }

        public int ImageLeftMargin
        {
            get { return _imageLeftMargin; }
            set
            {
                _imageLeftMargin = value;
                OnPropertyChanged("ImageLeftMargin");
            }
        }

        public int ImageRightMargin
        {
            get { return _imageRightMargin; }
            set
            {
                _imageRightMargin = value;
                OnPropertyChanged("ImageRightMargin");
            }
        }

        public int ImageTopMargin
        {
            get { return _imageTopMargin; }
            set
            {
                _imageTopMargin = value;
                OnPropertyChanged("ImageTopMargin");
            }
        }

        public int ImageBottomMargin
        {
            get { return _imageBottomMargin; }
            set
            {
                _imageBottomMargin = value;
                OnPropertyChanged("ImageBottomMargin");
            }
        }

        public int PortraitHorizontalMargins
        {
            get { return _portraitHorizontalMargins; }
            set
            {
                _portraitHorizontalMargins = value;
                OnPropertyChanged("PortraitHorizontalMargins");
            }
        }

        public int PortraitVerticalMargins
        {
            get { return _portraitVerticalMargins; }
            set
            {
                _portraitVerticalMargins = value;
                OnPropertyChanged("PortraitVerticalMargins");
            }
        }

        public int NewImageHorizontalResolution
        {
            get { return _newImageHorizontalResolution; }
            set
            {
                _newImageHorizontalResolution = value;
                OnPropertyChanged("NewImageHorizontalResolution");
            }
        }

        public int NewImageVerticalResolution
        {
            get { return _newImageVerticalResolution; }
            set
            {
                _newImageVerticalResolution = value;
                OnPropertyChanged("NewImageVerticalResolution");
            }
        }

        public System.Windows.Media.Color FontColor
        {
            get
            {
                return System.Windows.Media.Color.FromArgb(_fontColor.A, _fontColor.R, _fontColor.G, _fontColor.B);
            }
            set
            {
                _fontColor = Color.FromArgb(value.A, value.R, value.G, value.B);
                OnPropertyChanged("FontColor");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Names = new ObservableCollection<string>();
            NamesListView.ItemsSource = Names;
            NewNameTextBox.Focus();

            _dragManager = new ListViewDragDropManager<string>(NamesListView);

            _displayFont = new Font("Verdana", 20, System.Drawing.FontStyle.Bold);
            _fontColor = Color.AntiqueWhite;

            ImageLeftMargin = 128;
            ImageRightMargin = 128;
            ImageTopMargin = 52;
            ImageBottomMargin = 75;
            PortraitHorizontalMargins = 20;
            PortraitVerticalMargins = 20;
            NewImageHorizontalResolution = 1920;
            NewImageVerticalResolution = 1080;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
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
            try
            {
                Bitmap image;
                Graphics graphicImage;

                if (InputImageRadioButton.IsChecked.GetValueOrDefault())
                {
                    image = new Bitmap(_inputImagePath);
                    graphicImage = Graphics.FromImage(image);
                }
                else
                {
                    image = new Bitmap(NewImageHorizontalResolution, NewImageVerticalResolution);
                    graphicImage = Graphics.FromImage(image);
                }
                
                int portraitWidth = ((image.Width - _imageLeftMargin - ImageRightMargin) -
                                     (Columns - 1) * PortraitHorizontalMargins) / Columns;
                int portraitHeight = ((image.Height - ImageTopMargin - ImageBottomMargin) -
                                      (Rows - 1) * PortraitVerticalMargins) / Rows;

                
                int columnIndex = 0;
                int rowIndex = 0;

                for (int i = 0; i < Names.Count; i++)
                {
                    if (columnIndex + 1 > Columns)
                    {
                        rowIndex++;
                        columnIndex = 0;
                    }

                    SizeF size = graphicImage.MeasureString(Names[i], new Font("Verdana", 20, System.Drawing.FontStyle.Bold));

                    int extraHorizontalMargin = 0;
                    if (rowIndex + 1 >= Rows)
                    {
                        if (Rows * Columns - Names.Count != 1)
                            extraHorizontalMargin = (Rows * Columns - Names.Count) * (portraitWidth + PortraitHorizontalMargins) / 2;
                    }

                    graphicImage.DrawString(Names[i], _displayFont,
                            new SolidBrush(_fontColor),
                            new Point(
                                columnIndex * portraitWidth + _imageLeftMargin + (columnIndex * PortraitHorizontalMargins) +
                                portraitWidth - (int)size.Width + extraHorizontalMargin,
                                rowIndex * portraitHeight + ImageTopMargin + (rowIndex * PortraitVerticalMargins) + portraitHeight -
                                (int)size.Height));

                    columnIndex++;
                }

                image.Save(_outputFilePath, ImageFormat.Png);

                image.Dispose();
                graphicImage.Dispose();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error encountered", "Error encountere", MessageBoxButton.OK);
            }
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
            var fd = new FontDialog {Font = _displayFont};

            DialogResult dr = fd.ShowDialog();
            if (dr != System.Windows.Forms.DialogResult.Cancel)
            {
                _displayFont = fd.Font;
            }
        }

        private void InputImageLocationBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog {CheckFileExists = true, Filter = @"PNG (*.png)|*.png"};

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                InputImagePath = dialog.FileName;

            dialog.Dispose();
        }

        private void OutputImageLocationBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog {DefaultExt = ".png", CheckPathExists = true, Filter = @"PNG (*.png)|*.png"};

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                OutputImagePath = dialog.FileName;

            dialog.Dispose();
        }
    }
}