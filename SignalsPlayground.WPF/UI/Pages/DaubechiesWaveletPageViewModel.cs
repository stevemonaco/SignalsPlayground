using System;
using System.Linq;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Wpf;
using SignalsPlayground.Domain;
using SignalsPlayground.WPF.Services;
using Stylet;

using LineSeries = OxyPlot.Series.LineSeries;
using ScatterSeries = OxyPlot.Series.ScatterSeries;
using LinearAxis = OxyPlot.Axes.LinearAxis;

namespace SignalsPlayground.WPF.UI.Pages
{
    public class DaubechiesWaveletPageViewModel : NavigationPageViewModel
    {
        private DaubechiesWavelet _wavelet = new DaubechiesWavelet();

        private BindableCollection<WaveletKind> _waveletKinds = new BindableCollection<WaveletKind>();
        public BindableCollection<WaveletKind> WaveletKinds
        {
            get => _waveletKinds;
            set => SetAndNotify(ref _waveletKinds, value);
        }

        private WaveletKind _selectedWavelet;
        public WaveletKind SelectedWavelet
        {
            get => _selectedWavelet;
            set
            {
                if (SetAndNotify(ref _selectedWavelet, value))
                    UpdatePlot();
            }
        }

        private int _levels;
        public int Levels
        {
            get => _levels;
            set
            {
                if (SetAndNotify(ref _levels, value))
                    UpdatePlot();
            }
        }

        private int _pointCount;
        public int PointCount
        {
            get => _pointCount;
            set => SetAndNotify(ref _pointCount, value);
        }

        private bool _useScatterPlot = false;
        public bool UseScatterPlot
        {
            get => _useScatterPlot;
            set
            {
                if (SetAndNotify(ref _useScatterPlot, value))
                {
                    ChangePlot();
                    Levels = 0;
                }
            }
        }

        private PlotModel _waveletPlot;
        private readonly IFileSelectService _fileService;

        public PlotModel WaveletPlot
        {
            get => _waveletPlot;
            set => SetAndNotify(ref _waveletPlot, value);
        }

        public DaubechiesWaveletPageViewModel(IFileSelectService fileService)
        {
            _fileService = fileService;
            PageName = "Daubechies Wavelets";

            _waveletKinds = new BindableCollection<WaveletKind>
            { 
                WaveletKind.db2, WaveletKind.db3, WaveletKind.db4, WaveletKind.db5, WaveletKind.db6, 
                WaveletKind.db7, WaveletKind.db8, WaveletKind.db9, WaveletKind.db10
            };
            _selectedWavelet = WaveletKinds.First();
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            ChangePlot();
            UpdatePlot();
        }
        
        private void ChangePlot()
        {
            WaveletPlot = new PlotModel();

            if (UseScatterPlot)
            {
                WaveletPlot.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
                WaveletPlot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
                WaveletPlot.Series.Add(new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 4, MarkerFill = OxyColors.Crimson, Title = "Wavelet Function" });
                WaveletPlot.Series.Add(new ScatterSeries { MarkerType = MarkerType.Diamond, MarkerSize = 4, MarkerFill = OxyColors.ForestGreen, Title = "Scaling Function" });
            }
            else
            {
                WaveletPlot.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
                WaveletPlot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
                WaveletPlot.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, Color = OxyColors.Crimson, Title = "Wavelet Function" });
                WaveletPlot.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, Color = OxyColors.Black, Title = "Scaling Function" });
            }
        }

        private void UpdatePlot()
        {
            double yMax;
            double yMin;

            if (UseScatterPlot)
            {
                var waveletPoints = _wavelet.GetWaveletLevel(Levels, SelectedWavelet)
                    .Select(x => new ScatterPoint(x.X, x.Y));

                var scalingPoints = _wavelet.GetScalingLevels(Levels, SelectedWavelet)
                    .Last()
                    .Select(x => new ScatterPoint(x.X, x.Y));

                lock (WaveletPlot.SyncRoot)
                {
                    var waveletSeries = WaveletPlot.Series[0] as ScatterSeries;
                    waveletSeries.Points.Clear();
                    waveletSeries.Points.AddRange(waveletPoints);

                    var scalingSeries = WaveletPlot.Series[1] as ScatterSeries;
                    scalingSeries.Points.Clear();
                    scalingSeries.Points.AddRange(scalingPoints);

                    var yMaxA = waveletSeries.Points.Max(x => x.Y);
                    var yMaxB = scalingSeries.Points.Max(x => x.Y);
                    var yMinA = waveletSeries.Points.Min(x => x.Y);
                    var yMinB = scalingSeries.Points.Min(x => x.Y);

                    yMax = Math.Max(yMaxA, yMaxB);
                    yMin = Math.Min(yMinA, yMinB);
                    PointCount = waveletSeries.Points.Count;
                }
            }
            else
            {
                var waveletPoints = _wavelet.GetWaveletLevel(Levels, SelectedWavelet)
                    .Select(x => new DataPoint(x.X, x.Y));

                var scalingPoints = _wavelet.GetScalingLevels(Levels, SelectedWavelet)
                    .Last()
                    .Select(x => new DataPoint(x.X, x.Y));

                lock (WaveletPlot.SyncRoot)
                {
                    var waveletSeries = WaveletPlot.Series[0] as LineSeries;
                    waveletSeries.Points.Clear();
                    waveletSeries.Points.AddRange(waveletPoints);

                    var scalingSeries = WaveletPlot.Series[1] as LineSeries;
                    scalingSeries.Points.Clear();
                    scalingSeries.Points.AddRange(scalingPoints);

                    var yMaxA = waveletSeries.Points.Max(x => x.Y);
                    var yMaxB = scalingSeries.Points.Max(x => x.Y);
                    var yMinA = waveletSeries.Points.Min(x => x.Y);
                    var yMinB = scalingSeries.Points.Min(x => x.Y);

                    yMax = Math.Max(yMaxA, yMaxB);
                    yMin = Math.Min(yMinA, yMinB);
                    PointCount = waveletSeries.Points.Count;
                }
            }

            var xMax = _wavelet.GetMaxRange(_selectedWavelet);

            WaveletPlot.Axes[1].Minimum = -0.05;
            WaveletPlot.Axes[1].Maximum = xMax * 1.05;
            WaveletPlot.Axes[0].Maximum = 2; // yMax + Math.Abs(yMax * 0.05);
            WaveletPlot.Axes[0].Minimum = -1; // yMin - Math.Abs(yMin * 0.05);

            WaveletPlot.InvalidatePlot(false);
        }

        public void SaveImage()
        {
            if (_fileService.GetImageNameToSaveByUser() is string fileName)
            {
                var pngExporter = new PngExporter { Width = 600, Height = 400, Background = OxyColors.White };
                pngExporter.ExportToFile(_waveletPlot, fileName);
            }    
        }
    }
}
