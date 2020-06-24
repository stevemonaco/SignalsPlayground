using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SignalsPlayground.Domain;
using Stylet;

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
        public PlotModel WaveletPlot
        {
            get => _waveletPlot;
            set => SetAndNotify(ref _waveletPlot, value);
        }

        public DaubechiesWaveletPageViewModel()
        {
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
                WaveletPlot.Series.Add(new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 3, MarkerFill = OxyColor.FromArgb(255, 255, 0, 0), Title = "Wavelet Function" });
                WaveletPlot.Series.Add(new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 3, MarkerFill = OxyColor.FromArgb(255, 0, 255, 0), Title = "Scaling Function" });
            }
            else
            {
                WaveletPlot.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
                WaveletPlot.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom });
                WaveletPlot.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, Color = OxyColor.FromArgb(255, 255, 0, 0), Title = "Wavelet Function" });
                WaveletPlot.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, LineJoin = LineJoin.Miter, Color = OxyColor.FromArgb(255, 0, 255, 0), Title = "Scaling Function" });
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
            WaveletPlot.Axes[1].Maximum = xMax * 1.05;
            WaveletPlot.Axes[0].Maximum = yMax + Math.Abs(yMax * 0.05);
            WaveletPlot.Axes[0].Minimum = yMin - Math.Abs(yMin * 0.05);

            WaveletPlot.InvalidatePlot(false);
        }
    }
}
