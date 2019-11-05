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

using Xamarin.Essentials;

namespace headtrip.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        // for android only
        private bool _StartOnBoot { get; set; } = Preferences.Get("start_on_boot", false);
        public bool StartOnBoot
        {
            get => _StartOnBoot;
            set
            {
                if (_StartOnBoot == value) return;
                _StartOnBoot = value;
                Preferences.Set("start_on_boot", value);
                OnPropertyChanged();
            }
        }

        private SensorSpeed _MagnometerSpeed { get; set; } = (SensorSpeed)Preferences.Get("_MagnometerSpeed", (int)SensorSpeed.UI);
        public SensorSpeed MagnometerSpeed
        {
            get => _MagnometerSpeed;
            set
            {
                Preferences.Set("_MagnometerSpeed", (int)value);
                MonitorViewModel._magnetSpeed = _MagnometerSpeed = (SensorSpeed)value;
                OnPropertyChanged();
            }
        }

        private SensorSpeed _AcceleratorSpeed { get; set; } = (SensorSpeed)Preferences.Get("_AcceleratorSpeed", (int)SensorSpeed.UI);
        public SensorSpeed AcceleratorSpeed
        {
            get => _AcceleratorSpeed;
            set
            {
                Preferences.Set("_AcceleratorSpeed", (int)value);
                MonitorViewModel._acceleratorSpeed = _AcceleratorSpeed = (SensorSpeed)value;
                OnPropertyChanged();
            }
        }

        private bool _ThresholdAlarm { get; set; } = Preferences.Get("_ThresholdAlarm", false);
        public bool ThresholdAlarm
        {
            get => _ThresholdAlarm;
            set
            {
                if (value == _ThresholdAlarm) return;
                Preferences.Set("_ThresholdRange", value);
                _ThresholdAlarm = value;
                OnPropertyChanged();
            }
        }

        public SettingsViewModel()
        {
            Title = "CONFIG";
        }
    }
}