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

using SkiaSharp;
using System;

namespace headtrip.Utils
{
    public static class Emf
    {
        public static double ConvertToEmfReading(float x, float y, float z)
        {
            return Math.Sqrt(Math.Pow(x, 2.0d) + Math.Pow(y, 2.0d) + Math.Pow(z, 2.0d));
        }

        public static int MagnitudeLevelCheck(double emf)
        {
            if (emf <= 70)
            {
                return 0;
            }
            else if (emf > 75 && emf <= 150)
            {
                return 1;
            }
            else if (emf > 150 && emf <= 300)
            {
                return 2;
            }
            else if (emf > 300 && emf <= 450)
            {
                return 3;
            }
            else if (emf > 450 && emf <= 600)
            {
                return 4;
            }
            else if (emf > 600 && emf <= 900)
            {
                return 5;
            }
            else
            {
                return 5;
            }
        }

        public static SKColor MagnitudeColorGet(int magnitudeLevel)
        {
            switch (magnitudeLevel)
            {
                case 0:
                    {
                        return SKColor.Parse("#23c41d");
                    }
                case 1:
                    {
                        return SKColor.Parse("#3be3ab");
                    }
                case 2:
                    {
                        return SKColor.Parse("#def016");
                    }
                case 3:
                    {
                        return SKColor.Parse("#f0b216");
                    }
                case 4:
                    {
                        return SKColor.Parse("#f03716");
                    }
                default:
                    {
                        return SKColor.Parse("#171616");
                    }
            }
        }

        public static bool GetRotationMatrix(float[] R, float[] I, float[] gravity, float[] geomagnetic)
        {
            //! taken from android source code
            // TODO: move this to native code for efficiency
            float Ax = gravity[0];
            float Ay = gravity[1];
            float Az = gravity[2];
            float normsqA = (Ax * Ax + Ay * Ay + Az * Az);
            float g = 9.81f;
            float freeFallGravitySquared = 0.01f * g * g;
            if (normsqA < freeFallGravitySquared)
            {
                // gravity less than 10% of normal value
                return false;
            }
            float Ex = geomagnetic[0];
            float Ey = geomagnetic[1];
            float Ez = geomagnetic[2];
            float Hx = Ey * Az - Ez * Ay;
            float Hy = Ez * Ax - Ex * Az;
            float Hz = Ex * Ay - Ey * Ax;
            float normH = (float)Math.Sqrt(Hx * Hx + Hy * Hy + Hz * Hz);
            if (normH < 0.1f)
            {
                // device is close to free fall (or in space?), or close to
                // magnetic north pole. Typical values are  > 100.
                return false;
            }
            float invH = 1.0f / normH;
            Hx *= invH;
            Hy *= invH;
            Hz *= invH;
            float invA = 1.0f / (float)Math.Sqrt(Ax * Ax + Ay * Ay + Az * Az);
            Ax *= invA;
            Ay *= invA;
            Az *= invA;
            float Mx = Ay * Hz - Az * Hy;
            float My = Az * Hx - Ax * Hz;
            float Mz = Ax * Hy - Ay * Hx;
            if (R != null)
            {
                if (R.Length == 9)
                {
                    R[0] = Hx; R[1] = Hy; R[2] = Hz;
                    R[3] = Mx; R[4] = My; R[5] = Mz;
                    R[6] = Ax; R[7] = Ay; R[8] = Az;
                }
                else if (R.Length == 16)
                {
                    R[0] = Hx; R[1] = Hy; R[2] = Hz; R[3] = 0;
                    R[4] = Mx; R[5] = My; R[6] = Mz; R[7] = 0;
                    R[8] = Ax; R[9] = Ay; R[10] = Az; R[11] = 0;
                    R[12] = 0; R[13] = 0; R[14] = 0; R[15] = 1;
                }
            }
            if (I != null)
            {
                // compute the inclination matrix by projecting the geomagnetic
                // vector onto the Z (gravity) and X (horizontal component
                // of geomagnetic vector) axes.
                float invE = 1.0f / (float)Math.Sqrt(Ex * Ex + Ey * Ey + Ez * Ez);
                float c = (Ex * Mx + Ey * My + Ez * Mz) * invE;
                float s = (Ex * Ax + Ey * Ay + Ez * Az) * invE;
                if (I.Length == 9)
                {
                    I[0] = 1; I[1] = 0; I[2] = 0;
                    I[3] = 0; I[4] = c; I[5] = s;
                    I[6] = 0; I[7] = -s; I[8] = c;
                }
                else if (I.Length == 16)
                {
                    I[0] = 1; I[1] = 0; I[2] = 0;
                    I[4] = 0; I[5] = c; I[6] = s;
                    I[8] = 0; I[9] = -s; I[10] = c;
                    I[3] = I[7] = I[11] = I[12] = I[13] = I[14] = 0;
                    I[15] = 1;
                }
            }
            return true;
        }
    }
}
