/*
		  __ _/| _/. _  ._/__ /
		_\/_// /_///_// / /_|/
					   _/
		copyright (c) sof digital 2019
		written by michael rinderle <michael@sofdigital.net>

        mit license

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
*/

using headtrip.Data;
using headtrip.Interfaces;
using headtrip.Models;
using headtrip.Utils;
using Microcharts;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Forms = Xamarin.Forms;

namespace headtrip.ViewModels
{
    // todo : add switches for mag / acc speed
    public class MonitorViewModel : BaseViewModel
    {
        public ICommand DumpLogCommand
        {
            get
            {
                return new Forms.Command((async) => { DumpCommand(); });
            }
        }

        public static SensorSpeed _magnetSpeed = SensorSpeed.Default;
        public static SensorSpeed _acceleratorSpeed = SensorSpeed.Default;

        private Chart _EMFChart { get; set; }
        public Chart EMFChart
        {
            get => _EMFChart;
            set
            {
                if (_EMFChart == value) return;
                _EMFChart = value;
                OnPropertyChanged();
            }
        }

        private string _SensorX { get; set; }
        public string SensorX
        {
            get => _SensorX;
            set
            {
                _SensorX = value;
                OnPropertyChanged();
            }
        }

        private string _SensorY { get; set; }
        public string SensorY
        {
            get => _SensorY;
            set
            {
                _SensorY = value;
                OnPropertyChanged();
            }
        }

        private string _SensorZ { get; set; }
        public string SensorZ
        {
            get => _SensorZ;
            set
            {
                _SensorZ = value;
                OnPropertyChanged();
            }
        }

        private string _EMF { get; set; }
        public string EMF
        {
            get => _EMF;
            set
            {
                _EMF = value;
                OnPropertyChanged();
            }
        }

        private string _Magnitude { get; set; }
        public string Magnitude
        {
            get => _Magnitude;
            set
            {
                if (value == _Magnitude) return;
                _Magnitude = value;
                OnPropertyChanged();
            }
        }

        private string _Threshold { get; set; } = Preferences.Get("_Threshold", "75");
        public string Threshold
        {
            get => _Threshold;
            set
            {
                Preferences.Set("_Threshold", value);
                _Threshold = value;
                OnPropertyChanged();
            }
        }

        private int _ThresholdRange { get; set; } = Preferences.Get("_ThresholdRange", 75);
        public int ThresholdRange
        {
            get => _ThresholdRange;
            set
            {
                if (value == _ThresholdRange) return;
                Preferences.Set("_ThresholdRange", value);
                _ThresholdRange = value;
                Threshold = value.ToString();
                OnPropertyChanged(nameof(Threshold));
                OnPropertyChanged();
            }
        }

        private float Alpha = (float)0.8;
        private float[] Gravity = new float[3];
        private float[] Magnetic = new float[3];

        private ObservableCollection<LogEntry> _LogItems { get; set; }
        public ObservableCollection<LogEntry> LogItems
        {
            get
            {
                if (_LogItems == null)
                    _LogItems = new ObservableCollection<LogEntry>();
                return _LogItems;
            }
            set
            {
                _LogItems = value;
                OnPropertyChanged();
            }
        }

        public List<Entry> Entries = new List<Entry>
        {
            new Entry(10){},
            new Entry(10){},
            new Entry(10){},
            new Entry(10){},
            new Entry(10){},
            new Entry(10){},
            new Entry(10){},
            new Entry(10){},
            new Entry(10){},
            new Entry(10){},
         };
        public List<LogEntry> Logs = new List<LogEntry>();

        public MonitorViewModel()
        {
            Title = "EMF MONITOR";

            if (!Magnetometer.IsMonitoring)
                Magnetometer.Start(_magnetSpeed);

            if (!Accelerometer.IsMonitoring)
                Accelerometer.Start(_acceleratorSpeed);

            Magnetometer.ReadingChanged += Magnetometer_ReadingChanged;
            Accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
        }

        private async void DumpCommand()
        {
            try
            {
                string action = await Forms.Application.Current.MainPage
                           .DisplayActionSheet("Database Log Dump", "Cancel", null, "Dump DB", "Dump & Clear DB", "Clear DB");

                switch (action)
                {
                    case "Dump DB":
                        {
                            Forms.DependencyService.Get<ISqlite>().ExportDB();
                            break;
                        }
                    case "Dump & Clear DB":
                        {
                            Forms.DependencyService.Get<ISqlite>().ExportDB();
                            using (var ctx = new SqliteContext())
                                ctx.RemoveRange(ctx.LogEntrys);
                            LogItems.Clear();
                            Logs.Clear();
                            break;
                        }
                    case "Clear DB":
                        {
                            using (var ctx = new SqliteContext())
                                ctx.RemoveRange(ctx.LogEntrys);

                            LogItems.Clear();
                            Logs.Clear();
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                await Forms.Application.Current.MainPage.DisplayAlert("Alert", "Error dumping database.", "Okay");
            }
        }

        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            // isolating force
            Gravity[0] = Alpha * Gravity[0] + (1 - Alpha) * e.Reading.Acceleration.X;
            Gravity[1] = Alpha * Gravity[1] + (1 - Alpha) * e.Reading.Acceleration.Y;
            Gravity[2] = Alpha * Gravity[2] + (1 - Alpha) * e.Reading.Acceleration.Z;
        }

        private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            Magnetic[0] = e.Reading.MagneticField.X;
            Magnetic[1] = e.Reading.MagneticField.Y;
            Magnetic[2] = e.Reading.MagneticField.Z;
            OnSensorUpdate();
        }

        private void OnSensorUpdate()
        {
            float[] originalValues = Magnetic;
            float[] R = new float[9];
            float[] I = new float[9];

            if (!Emf.GetRotationMatrix(R, I, Gravity, Magnetic)) return;

            float[] A_D = originalValues;
            float[] A_W = new float[3];

            A_W[0] = R[0] * A_D[0] + R[1] * A_D[1] + R[2] * A_D[2];
            A_W[1] = R[3] * A_D[0] + R[4] * A_D[1] + R[5] * A_D[2];
            A_W[2] = R[6] * A_D[0] + R[7] * A_D[1] + R[8] * A_D[2];

            double emf = Emf.ConvertToEmfReading(A_W[0], A_W[1], A_W[2]);
            int mag = Emf.MagnitudeLevelCheck(emf);

            UpdateUI(A_W, emf, mag);
            UpdateChart(emf, mag);

            if (emf > ThresholdRange)
                CreateLogEntry(A_W, emf, mag);
        }

        private void UpdateUI(float[] convertedSensorValues, double emf, int mag)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                SensorX = convertedSensorValues[0].ToString();
                SensorY = convertedSensorValues[1].ToString();
                SensorZ = convertedSensorValues[2].ToString();

                EMF = emf.ToString("0.00") + " \u00B5T";
                Magnitude = mag.ToString();
            });
        }

        private void UpdateChart(double emf, double magnitude)
        {
            Entries.RemoveAt(0);
            var new_entry = new Entry((int)emf)
            {
                Color = Emf.MagnitudeColorGet((int)magnitude),
                Label = Magnitude,
                ValueLabel = emf.ToString("0.00"),
            };

            Entries.Add(new_entry);
            float maxValue = Entries.Select(x => x.Value).Max();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                EMFChart = new LineChart() { Entries = Entries, MaxValue = (int)maxValue + 100, BackgroundColor = SKColor.Parse("#778899") };
            });
        }

        private void CreateLogEntry(float[] convertedSensorValues, double emf, int mag)
        {
            var geo = Geo.GetLocationCoordinates().Result;
            if (geo == null) geo = new Location();

            LogEntry log = new LogEntry()
            {
                Logged = DateTime.Now,
                X = convertedSensorValues[0],
                Y = convertedSensorValues[1],
                Z = convertedSensorValues[2],
                EMF = emf,
                MagnitudeLevel = mag,
                Latitude = geo.Latitude,
                Longitude = geo.Longitude,
                Altitude = geo.Altitude,
            };

            using (var sql = new SqliteContext())
            {
                Logs.Add(log);
                sql.LogEntrys.Add(log);
                sql.SaveChanges();
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                LogItems.Add(log);
            });
        }
    }
}