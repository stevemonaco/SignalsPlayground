using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SignalsPlayground.Domain;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SignalsPlayground.WPF.UI.Pages
{
    public class DaubechiesWaveletPageViewModel : NavigationPageViewModel
    {
        private PlotModel _waveletPlot = new PlotModel();
        public PlotModel WaveletPlot
        {
            get => _waveletPlot;
            set => SetAndNotify(ref _waveletPlot, value);
        }

        public DaubechiesWaveletPageViewModel()
        {
            PageName = "Daubechies Wavelets";
        }

        protected override void OnInitialActivate()
        {
            base.OnInitialActivate();

            var wavelet = new DaubechiesWavelet();
            var useScatterPlot = false;
            var waveletKind = WaveletKind.db4;
            var levels = 10;

            if (useScatterPlot)
            {
                var waveletPoints = wavelet.GetWaveletLevel(levels, waveletKind)
                    .Select(x => new ScatterPoint(x.X, x.Y));

                var scalingPoints = wavelet.GetScalingLevels(levels, waveletKind)
                    .Last()
                    .Select(x => new ScatterPoint(x.X, x.Y));

                WaveletPlot.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
                WaveletPlot.Series.Add(new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 3, MarkerFill = OxyColor.FromArgb(255, 255, 0, 0), Title = "Wavelet Function" });
                WaveletPlot.Series.Add(new ScatterSeries { MarkerType = MarkerType.Circle, MarkerSize = 3, MarkerFill = OxyColor.FromArgb(255, 0, 255, 0), Title = "Scaling Function" });

                lock (WaveletPlot.SyncRoot)
                {
                    var waveletSeries = WaveletPlot.Series[0] as ScatterSeries;
                    waveletSeries.Points.AddRange(waveletPoints);

                    var scalingSeries = WaveletPlot.Series[1] as ScatterSeries;
                    scalingSeries.Points.AddRange(scalingPoints);
                }
            }
            else
            {
                var waveletPoints = wavelet.GetWaveletLevel(levels, waveletKind)
                    .Select(x => new DataPoint(x.X, x.Y));

                var scalingPoints = wavelet.GetScalingLevels(levels, waveletKind)
                    .Last()
                    .Select(x => new DataPoint(x.X, x.Y));

                WaveletPlot.Axes.Add(new LinearAxis { Position = AxisPosition.Left });
                WaveletPlot.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, Color = OxyColor.FromArgb(255, 255, 0, 0), Title = "Wavelet Function" });
                WaveletPlot.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, Color = OxyColor.FromArgb(255, 0, 255, 0), Title = "Scaling Function" });

                lock (WaveletPlot.SyncRoot)
                {
                    var waveletSeries = WaveletPlot.Series[0] as LineSeries;
                    waveletSeries.Points.AddRange(waveletPoints);

                    var scalingSeries = WaveletPlot.Series[1] as LineSeries;
                    scalingSeries.Points.AddRange(scalingPoints);
                }
            }
        }
    }
}
