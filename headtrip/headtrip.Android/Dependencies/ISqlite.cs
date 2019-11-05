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

using Android.Runtime;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(headtrip.Droid.ISqlite))]

namespace headtrip.Droid
{
    [Preserve(AllMembers = true)]
    public class ISqlite : headtrip.Interfaces.ISqlite
    {
        private string dbName = "headtrip.db3";

        public void ExportDB()
        {
            var internalPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
            var bytes = File.ReadAllBytes(internalPath);
            string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var fileCopyName = string.Format("headtrip_{0:dd-MM-yyyy_HH-mm-ss-tt}.db3", System.DateTime.Now);
            string dbPath = Path.Combine(path, fileCopyName);
            File.WriteAllBytes(dbPath, bytes);
        }

        public string GetEntityDbPath()
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), dbName);
            return path;
        }
    }
}