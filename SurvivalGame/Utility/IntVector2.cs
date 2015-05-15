using Microsoft.Xna.Framework;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Mentula.General
{
    /// <summary>
    /// Defines a vector with two components.
    /// </summary>
    public struct IntVector2 : IEquatable<IntVector2>, IEquatable<Vector2>
    {
        /// <summary> Returns a IntVector2 with both of its components set to one. </summary>
        public static IntVector2 One { get { return pOne; } }
        /// <summary> Returns the unit vector for the x-axis. </summary>
        public static IntVector2 UnitX { get { return pUnitX; } }
        /// <summary> Returns the unit vector for the y-axis. </summary>
        public static IntVector2 UnitY { get { return pUnitY; } }
        /// <summary> Returns a IntVector2 with both of its components set to zero. </summary>
        public static IntVector2 Zero { get { return pZero; } }

        /// <summary> Gets the length of the vector. </summary>
        public float Length { get { return (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2)); } }
        /// <summary> Gets the length of the vector squared. </summary>
        public float LengthSquared { get { return (float)Math.Pow(Length, 2); } }
        /// <summary>
        /// Gets or sets the x-component of the vector.
        /// </summary>
        public int X;
        /// <summary> Gets or sets the y-component of the vector. </summary>
        public int Y;

        private static IntVector2 pOne;
        private static IntVector2 pUnitX;
        private static IntVector2 pUnitY;
        private static IntVector2 pZero;

        /// <summary> Creates a new instance of IntVector2 </summary>
        /// <param name="value"> Value to initialize both componets to. </param>
        public IntVector2(int value)
        {
            X = value;
            Y = value;
        }

        /// <summary> Initializes a new instance of IntVector2. </summary>
        /// <param name="x"> Initial value for the x-component of the vector. </param>
        /// <param name="y"> Initial value for the y-component of the vector. </param>
        public IntVector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary> Initializes a new instance of IntVector2. </summary>
        /// <param name="x"> Initial value for the x-component of the vector. </param>
        /// <param name="y"> Initial value for the y-component of the vector. </param>
        public IntVector2(float x, float y)
        {
            X = (int)x;
            Y = (int)y;
        }

        /// <summary> Creates a new instance of IntVector2 </summary>
        /// <param name="vector"> The Vector2 to base this vector to. </param>
        public IntVector2(Vector2 vector)
        {
            X = (int)vector.X;
            Y = (int)vector.Y;
        }

        static IntVector2()
        {
            pOne = new IntVector2(1, 1);
            pUnitX = new IntVector2(1, 0);
            pUnitY = new IntVector2(0, 1);
            pZero = new IntVector2(0, 0);
        }

        public static IntVector2 operator +(IntVector2 sender, IntVector2 caller) { return new IntVector2(sender.X + caller.X, sender.Y + caller.Y); }
        public static IntVector2 operator -(IntVector2 value) { return new IntVector2(-value.X, -value.Y); }
        public static IntVector2 operator -(IntVector2 sender, IntVector2 caller) { return new IntVector2(sender.X - caller.X, sender.Y - caller.Y); }
        public static IntVector2 operator *(float scaleFactor, IntVector2 caller) { return new IntVector2((int)(caller.X * scaleFactor), (int)(caller.Y * scaleFactor)); }
        public static IntVector2 operator *(IntVector2 sender, float scaleFactor) { return new IntVector2((int)(sender.X * scaleFactor), (int)(sender.Y * scaleFactor)); }
        public static IntVector2 operator *(IntVector2 sender, IntVector2 caller) { return new IntVector2(sender.X * caller.X, sender.Y * caller.Y); }
        public static IntVector2 operator /(IntVector2 sender, float devider) { return new IntVector2((int)(sender.X / devider), (int)(sender.Y / devider)); }
        public static IntVector2 operator /(IntVector2 sender, IntVector2 caller) { return new IntVector2((int)(sender.X / caller.X), (int)(sender.Y / caller.Y)); }
        public static bool operator !=(IntVector2 sender, IntVector2 caller) { return (sender.X != caller.X || sender.Y != caller.Y); }
        public static bool operator ==(IntVector2 sender, IntVector2 caller) { return (sender.X == caller.X && sender.Y == caller.Y); }

        /// <summary> Adds two vectors. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        public static IntVector2 Add(IntVector2 value1, IntVector2 value2)
        {
            return new IntVector2(value1.X + value2.X, value1.Y + value2.Y);
        }

        /// <summary> Adds two vectors. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="result"> [OutAttribute] Sum of the source vectors. </param>
        public static void Add(ref IntVector2 value1, ref IntVector2 value2, out IntVector2 result)
        {
            result = new IntVector2(value1.X + value2.X, value1.Y + value2.Y);
        }

        /// <summary> Get the angle between two vectors. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        public static float AngleBetween(IntVector2 value1, IntVector2 value2)
        {
            return (float)Math.Atan2(value2.Y - value1.Y, value2.X - value1.X);
        }

        /// <summary> Get the angle between two vectors. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="result"> [OutAttribute] Angle between the source vectors. </param>
        public static void AngleBetween(ref IntVector2 value1, ref IntVector2 value2, out float result)
        {
            result = (float)Math.Atan2(value2.Y - value1.Y, value2.X - value1.X);
        }

        /// <summary>
        /// Returns a IntVector2 containing the 2D Cartesian of a point specfied
        /// in a Barycentric (areal) coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1"> A IntVector2 containing the 2D Cartesian coordinates of vertex 1 of the triangle. </param>
        /// <param name="value2"> A IntVector2 containing the 2D Cartesian coordinates of vertex 2 of the triangle. </param>
        /// <param name="value3"> A IntVector2 containing the 2D Cartesian coordinates of vertex 3 of the triangle. </param>
        /// <param name="amount1"> Barycentric coordinate b2, wich expresses the weighting factor toward vertex 2. </param>
        /// <param name="amount2"> Barycentric coordinate b3, wich expresses the weighting factor toward vertex 3. </param>
        public static IntVector2 Barycentric(IntVector2 value1, IntVector2 value2, IntVector2 value3, int amount1, int amount2)
        {
            int Px = ((1 - amount1 - amount2) * value1.X) + (amount1 * value2.X) + (amount2 * value3.X);
            int Py = ((1 - amount1 - amount2) * value1.Y) + (amount1 * value2.Y) + (amount2 * value3.Y);

            return new IntVector2(Px, Py);
        }

        /// <summary>
        /// Returns a IntVector2 containing the 2D Cartesian of a point specfied
        /// in a Barycentric (areal) coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1"> A IntVector2 containing the 2D Cartesian coordinates of vertex 1 of the triangle. </param>
        /// <param name="value2"> A IntVector2 containing the 2D Cartesian coordinates of vertex 2 of the triangle. </param>
        /// <param name="value3"> A IntVector2 containing the 2D Cartesian coordinates of vertex 3 of the triangle. </param>
        /// <param name="amount1"> Barycentric coordinate b2, wich expresses the weighting factor toward vertex 2. </param>
        /// <param name="amount2"> Barycentric coordinate b3, wich expresses the weighting factor toward vertex 3. </param>
        /// <param name="result"> [OutAttribute] The 2D Cartesian coordinates of the specified point are placed in this IntVector2 on exit. </param>
        public static void Barycentric(ref IntVector2 value1, ref IntVector2 value2, ref IntVector2 value3, out IntVector2 result, int amount1, int amount2)
        {
            int Px = ((1 - amount1 - amount2) * value1.X) + (amount1 * value2.X) + (amount2 * value3.X);
            int Py = ((1 - amount1 - amount2) * value1.Y) + (amount1 * value2.Y) + (amount2 * value3.Y);

            result = new IntVector2(Px, Py);
        }

        /// <summary> Performs a Catmull-Rom interpolation using the specified positions. </summary>
        /// <param name="value1"> The first position in the interpolation. </param>
        /// <param name="value2"> The second position in the interpolation. </param>
        /// <param name="value3"> The third position in the interpolation. </param>
        /// <param name="value4"> The fourth position in the interpolation. </param>
        /// <param name="amount"> Weighting factor. </param>
        public static IntVector2 CatmullRom(IntVector2 value1, IntVector2 value2, IntVector2 value3, IntVector2 value4, float amount)
        {
            float amount2 = amount * amount;
            float amount3 = amount2 * amount;

            int Px = (int)(.5f * ((2f * value2.X) + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4 * value3.X - value4.X) * amount2 + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * amount3));
            int Py = (int)(.5f * ((2f * value2.Y) + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4 * value3.Y - value4.Y) * amount2 + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * amount3));

            return new IntVector2(Px, Py);
        }

        /// <summary> Performs a Catmull-Rom interpolation using the specified positions. </summary>
        /// <param name="value1"> The first position in the interpolation. </param>
        /// <param name="value2"> The second position in the interpolation. </param>
        /// <param name="value3"> The third position in the interpolation. </param>
        /// <param name="value4"> The fourth position in the interpolation. </param>
        /// <param name="amount"> Weighting factor. </param>
        /// <param name="result"> [OutAttribute] A vector that is the result of the Catmull-Rom interplation. </param>
        public static void CatmullRom(ref IntVector2 value1, ref IntVector2 value2, ref IntVector2 value3, ref IntVector2 value4, float amount, out IntVector2 result)
        {
            float amount2 = amount * amount;
            float amount3 = amount2 * amount;

            float Px = .5f * ((2f * value2.X) + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4 * value3.X - value4.X) * amount2 + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * amount3);
            float Py = .5f * ((2f * value2.Y) + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4 * value3.Y - value4.Y) * amount2 + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * amount3);

            result = new IntVector2(Px, Py);
        }

        /// <summary> Restricts a value to be within a specified range. </summary>
        /// <param name="value"> The value to clamp. </param>
        /// <param name="min"> The minimum value. </param>
        /// <param name="max"> The maximum value. </param>
        public static IntVector2 Clamp(IntVector2 value, IntVector2 min, IntVector2 max)
        {
            int Px = value.X >= max.X ? max.X : (value.X <= min.X ? min.X : value.X);
            int Py = value.Y >= max.Y ? max.Y : (value.Y <= min.Y ? min.Y : value.Y);

            return new IntVector2(Px, Py);
        }

        /// <summary> Restricts a value to be within a specified range. </summary>
        /// <param name="value"> The value to clamp. </param>
        /// <param name="min"> The minimum value. </param>
        /// <param name="max"> The maximum value. </param>
        /// <param name="result"> [OutAttribute] The clamped value. </param>
        public static void Clamp(ref IntVector2 value, ref IntVector2 min, ref IntVector2 max, out IntVector2 result)
        {
            result.X = value.X >= max.X ? max.X : (value.X <= min.X ? min.X : value.X);
            result.Y = value.Y >= max.Y ? max.Y : (value.Y <= min.Y ? min.Y : value.Y);
        }

        /// <summary> Calculates the distance between two vectors. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        public static int Distance(IntVector2 value1, IntVector2 value2)
        {
            return (value1.X - value2.X) + (value1.Y - value2.Y);
        }

        /// <summary> Calculates the distance between two vectors. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="result"> [OutAttribute] The distance between the vectors. </param>
        public static void Distance(ref IntVector2 value1, ref IntVector2 value2, out int result)
        {
            result = (value1.X - value2.X) + (value1.Y - value2.Y);
        }

        /// <summary> Calculates the distance between tho vectors squared. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        public static int DistanceSquared(IntVector2 value1, IntVector2 value2)
        {
            return (int)Math.Sqrt(Math.Pow(value1.X - value2.X, 2) + Math.Pow(value1.Y - value2.Y, 2));
        }

        /// <summary> Calculates the distance between tho vectors squared. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="result"> [OutAttribute] The distance between the vectors squared. </param>
        public static void DistanceSquared(ref IntVector2 value1, ref IntVector2 value2, out int result)
        {
            result = (int)Math.Sqrt(Math.Pow(value1.X - value2.X, 2) + Math.Pow(value1.Y - value2.Y, 2));
        }

        /// <summary> Divides a vector by a scalar value. </summary>
        /// <param name="value"> Source vector. </param>
        /// <param name="devider"> The devisor. </param>
        public static IntVector2 Devide(IntVector2 value, float devider)
        {
            return new IntVector2(value.X / devider, value.Y / devider);
        }

        /// <summary> Divides the components of a vector by the components of another vector. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Divisor vector. </param>
        public static IntVector2 Devide(IntVector2 value1, IntVector2 value2)
        {
            return new IntVector2(value1.X / value2.X, value1.Y / value2.Y);
        }

        /// <summary> Divides a vector by a scalar value. </summary>
        /// <param name="value"> Source vector. </param>
        /// <param name="devider"> The devisor. </param>
        /// <param name="result"> [OutAttribue] The result of the division. </param>
        public static void Devide(ref IntVector2 value, float devider, out IntVector2 result)
        {
            result = new IntVector2(value.X / devider, value.Y / devider);
        }

        /// <summary> Divides the components of a vector by the components of another vector. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Divisor vector. </param>
        /// <param name="result"> [OutAttribue] The result of the division. </param>
        public static void Devide(ref IntVector2 value1, ref IntVector2 value2, out IntVector2 result)
        {
            result = new IntVector2(value1.X / value2.X, value1.Y / value2.Y);
        }

        /// <summary> 
        /// Calculates the dot product of two vectors. If the two vectors are unit vectors,
        /// the dot product returns a floating point value between -1 and 1 that can be used
        /// to determine some properties of the angle between two vectors. For example, it
        /// show whether the vectors are orthogonal, parallel, or have an acute or obtuse
        /// angle between them.
        /// </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        public static float Dot(IntVector2 value1, IntVector2 value2)
        {
            float thera = AngleBetween(value1, value2);

            return value1.Length * value2.Length * (float)Math.Cos(thera);
        }

        /// <summary>
        /// Calculates the dot product of two vectors and writes the result to a user-specified
        /// variable. If the two vectors are unit vectors, the dot product returns a floating
        /// point value between -1 and 1 that can be used to determine some properties of the
        /// angle between two vector. For example, it can show whether the vectors are orthogonal,
        /// parallel, or have an acute or obtuse angle between them.
        /// </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="result"> [OutAttribute] The dot product of the two vectors. </param>
        public static void Dot(ref IntVector2 value1, ref IntVector2 value2, out float result)
        {
            float thera = AngleBetween(value1, value2);

            result = value1.Length * value2.Length * (float)Math.Cos(thera);
        }

        /// <summary> Returns a value that indicates whether the current instance is equal to a specified object. </summary>
        /// <param name="obj"> Object to make the comparison with. </param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            try
            {
                Vector2 vect2 = (Vector2)TypeDescriptor.GetConverter(typeof(Vector2)).ConvertTo(obj, typeof(Vector2));
                return Equals(vect2);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary> Determines whether the specified Object is equal to the IntVector2. </summary>
        /// <param name="other"> The Object to compare with the current IntVector2. </param>
        public bool Equals(IntVector2 other)
        {
            return (other.X == X) && (other.Y == Y);
        }

        /// <summary> Determines whether the specified Object is equal to the IntVector2. </summary>
        /// <param name="other"> The Object to compare with the current IntVector2. </param>
        public bool Equals(Vector2 other)
        {
            return (other.X == X) && (other.Y == Y);
        }

        /// <summary> Gets the hash code of the vector object. </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = hash * 16777619 ^ X.GetHashCode();
                hash = hash * 16777619 ^ Y.GetHashCode();
                return hash;
            }
        }

        /// <summary> Perfoms a Hermite spline interpolation. </summary>
        /// <param name="value1"> Source position vector. </param>
        /// <param name="tangent1"> Source tangent vector. </param>
        /// <param name="value2"> Source position vector. </param>
        /// <param name="tangent2"> Source tangent vector. </param>
        /// <param name="amount"> Weighting factor. </param>
        public static IntVector2 Hermite(IntVector2 value1, IntVector2 tangent1, IntVector2 value2, IntVector2 tangent2, float amount)
        {
            float num = amount * amount;
            float num2 = amount * num;
            float num3 = num2 - num;
            float num4 = (num2 - (2f * num) + amount);
            float num5 = (-2f * num2) + (3f * num);
            float num6 = ((2f * num2) - (3f * num)) + 1f;

            float Px = (((value1.X * num6) + (value2.X * num5)) + (tangent1.X * num4)) + (tangent2.X * num3);
            float Py = (((value1.Y * num6) + (value2.Y * num5)) + (tangent1.Y * num4)) + (tangent2.Y * num3);

            return new IntVector2(Px, Py);
        }

        /// <summary> Perfoms a Hermite spline interpolation. </summary>
        /// <param name="value1"> Source position vector. </param>
        /// <param name="tangent1"> Source tangent vector. </param>
        /// <param name="value2"> Source position vector. </param>
        /// <param name="tangent2"> Source tangent vector. </param>
        /// <param name="amount"> Weighting factor. </param>
        /// <param name="result"> [OutAttribute] The result of the Hermite spline interpolation. </param>
        public static void Hermite(ref IntVector2 value1, ref IntVector2 tangent1, ref IntVector2 value2, ref IntVector2 tangent2, float amount, out IntVector2 result)
        {
            float num = amount * amount;
            float num2 = amount * num;
            float num3 = num2 - num;
            float num4 = (num2 - (2f * num) + amount);
            float num5 = (-2f * num2) + (3f * num);
            float num6 = ((2f * num2) - (3f * num)) + 1f;

            float Px = (((value1.X * num6) + (value2.X * num5)) + (tangent1.X * num4)) + (tangent2.X * num3);
            float Py = (((value1.Y * num6) + (value2.Y * num5)) + (tangent1.Y * num4)) + (tangent2.Y * num3);

            result = new IntVector2(Px, Py);
        }

        /// <summary> Performs a linear interpolation between two vectors. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="amount"> Value between 0 and 1 indicating the weight of value2. </param>
        public static IntVector2 Lerp(IntVector2 value1, IntVector2 value2, float amount)
        {
            if (amount == 0 || amount == 1) return amount == 0 ? value1 : value2;
            else return value1 + (value2 - value1) * amount;
        }

        /// <summary> Performs a linear interpolation between two vectors. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="amount"> Value between 0 and 1 indicating the weight of value2. </param>
        /// <param name="result"> [OutAttribute] The result of the interpolation. </param>
        public static void Lerp(ref IntVector2 value1, ref IntVector2 value2, float amount, out IntVector2 result)
        {
            if (amount == 0 || amount == 1) result = amount == 0 ? value1 : value2;
            else result = value1 + (value2 - value1) * amount;
        }

        /// <summary> Returns a vector that contains the highest value from each matching pair of components. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        public static IntVector2 Max(IntVector2 value1, IntVector2 value2)
        {
            int Px = value1.X >= value2.X ? value1.X : value2.X;
            int Py = value1.Y >= value2.Y ? value1.Y : value2.Y;

            return new IntVector2(Px, Py);
        }

        /// <summary> Returns a vector that contains the highest value from each matching pair of components. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="result"> [OutAttribute] The maximized vector. </param>
        public static void Max(ref IntVector2 value1, ref IntVector2 value2, out IntVector2 result)
        {
            result.X = value1.X >= value2.X ? value1.X : value2.X;
            result.Y = value1.Y >= value2.Y ? value1.Y : value2.Y;
        }

        /// <summary> Returns a vector that contains the lowest value from each matching pair of components. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        public static IntVector2 Min(IntVector2 value1, IntVector2 value2)
        {
            int Px = value1.X <= value2.X ? value1.X : value2.X;
            int py = value1.Y <= value2.Y ? value1.Y : value2.Y;

            return new IntVector2(Px, py);
        }

        /// <summary> Returns a vector that contains the lowest value from each matching pair of components. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="result"> [OutAttribute] The minimized vector. </param>
        public static void Min(ref IntVector2 value1, ref IntVector2 value2, out IntVector2 result)
        {
            result.X = value1.X <= value2.X ? value1.X : value2.X;
            result.Y = value1.Y <= value2.Y ? value1.Y : value2.Y;
        }

        /// <summary> Multiplies a vector by a scalar value. </summary>
        /// <param name="value"> Source vector. </param>
        /// <param name="scaleFactor"> Scalar value. </param>
        public static IntVector2 Multiply(IntVector2 value, float scaleFactor)
        {
            return new IntVector2(value.X * scaleFactor, value.Y * scaleFactor);
        }

        /// <summary> Multiplies the components of two vectors by each other. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        public static IntVector2 Multiply(IntVector2 value1, IntVector2 value2)
        {
            return new IntVector2(value1.X * value2.X, value1.X * value2.X);
        }

        /// <summary> Multiplies a vector by a scalar value. </summary>
        /// <param name="value"> Source vector. </param>
        /// <param name="scaleFactor"> Scalar value. </param>
        /// <param name="result"> [OutAttribute] The result of the multiplication. </param>
        public static void Multiply(ref IntVector2 value, float scaleFactor, out IntVector2 result)
        {
            result = new IntVector2(value.X * scaleFactor, value.Y * scaleFactor);
        }

        /// <summary> Multiplies the components of two vectors by each other. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="result"> [OutAttribute] The result of the multiplication. </param>
        public static void Multiply(ref IntVector2 value1, IntVector2 value2, out IntVector2 result)
        {
            result = new IntVector2(value1.X * value2.X, value1.X * value2.X);
        }

        /// <summary> Returns a vector pointing in the opposite direction. </summary>
        /// <param name="value"> Source vector. </param>
        public static IntVector2 Negate(IntVector2 value)
        {
            return new IntVector2(-value.X, -value.Y);
        }

        /// <summary> Returns a vector pointing in the opposite direction. </summary>
        /// <param name="value"> Source vector. </param>
        /// <param name="result"> [OutAttribute] Vector pointing in the opposite direction. </param>
        public static void Negate(ref IntVector2 value, out IntVector2 result)
        {
            result = new IntVector2(-value.X, -value.Y);
        }

        /// <summary> 
        /// Turns the current vector into a unit vector. The result is a vector one
        /// unit in length pointing in the same direction as the original vector.
        /// </summary>
        public void Normalize()
        {
            Y = X == 0 ? 0 : (X > 0 ? 1 : -1);
            X = Y == 0 ? 0 : (Y > 0 ? 1 : -1);
        }

        /// <summary>
        /// Creates a unit vector from the specified vector. The result is a vector
        /// one unit in length pointing in the same direction as the original vecto.
        /// </summary>
        /// <param name="value"> Source vector. </param>
        public static IntVector2 Normalize(IntVector2 value)
        {
            int Px = value.X == 0 ? 0 : (value.X > 0 ? 1 : -1);
            int Py = value.Y == 0 ? 0 : (value.Y > 0 ? 1 : -1);

            return new IntVector2(Px, Py);
        }

        /// <summary>
        /// Creates a unit vector from the specfied vector, writing the result to a
        /// user-specified varieble. The result is a vector one unit in length pointing
        /// in the same direction as the orginal vector.
        /// </summary>
        /// <param name="value"> Source vector. </param>
        /// <param name="result"> [OutAttribute] Normalized vector. </param>
        public static void Normalize(ref IntVector2 value, out IntVector2 result)
        {
            result.X = value.X == 0 ? 0 : (value.X > 0 ? 1 : -1);
            result.Y = value.Y == 0 ? 0 : (value.Y > 0 ? 1 : -1);
        }

        /// <summary> Determines the reflect vector of the given vector and normal. </summary>
        /// <param name="vector"> Source vector. </param>
        /// <param name="normal"> Normal of the vector. </param>
        public static IntVector2 Reflect(IntVector2 vector, IntVector2 normal)
        {
            return vector - 2 * (vector * normal) * normal;
        }

        /// <summary> Determines the reflect vector of the given vector and normal. </summary>
        /// <param name="vector"> Source vector. </param>
        /// <param name="normal"> Normal of the vector. </param>
        /// <param name="result"> [OutAttribute] The created reflect vector. </param>
        public static void Reflect(ref IntVector2 vector, ref IntVector2 normal, out IntVector2 result)
        {
            result = vector - 2 * (vector * normal) * normal;
        }

        /// <summary> Interpolates between two values using cubic equation. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="amount"> Weighting value. </param>
        public static IntVector2 SmoothStep(IntVector2 value1, IntVector2 value2, float amount)
        {
            amount = amount > 1f ? 1f : (amount < 0f ? 0f : amount);
            amount = (amount * amount) * (3f - (2f * amount));

            float Px = value1.X + ((value2.X - value1.X) * amount);
            float Py = value1.Y + ((value2.Y - value1.Y) * amount);

            return new IntVector2(Px, Py);
        }

        /// <summary> Interpolates between two values using cubic equation. </summary>
        /// <param name="value1"> Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="amount"> Weighting value. </param>
        /// <param name="result"> [OutAttribute] The interpolated value. </param>
        public static void SmoothStep(ref IntVector2 value1, ref IntVector2 value2, float amount, out IntVector2 result)
        {
            amount = amount > 1f ? 1f : (amount < 0f ? 0f : amount);
            amount = (amount * amount) * (3f - (2f * amount));

            float Px = value1.X + ((value2.X - value1.X) * amount);
            float Py = value1.Y + ((value2.Y - value1.Y) * amount);

            result = new IntVector2(Px, Py);
        }

        /// <summary> Subtract a vector from a vector. </summary>
        /// <param name="value1">Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        public static IntVector2 Subtract(IntVector2 value1, IntVector2 value2)
        {
            return new IntVector2(value1.X - value2.X, value1.Y - value2.Y);
        }

        /// <summary> Subtract a vector from a vector. </summary>
        /// <param name="value1">Source vector. </param>
        /// <param name="value2"> Source vector. </param>
        /// <param name="result"> [OutAttribute] The result of the subtraction. </param>
        public static void Subtract(ref IntVector2 value1, ref IntVector2 value2, out IntVector2 result)
        {
            result = new IntVector2(value1.X - value2.X, value1.Y - value2.Y);
        }

        /// <summary> Retrieves a string representation of the current object. </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{{X:{0} Y:{1}}}", new object[2] { X, Y });
        }

        /// <summary> Retrieves a Vector2 representation of the current object. </summary>
        public Vector2 ToVector2()
        {
            return new Vector2(X, Y);
        }
    }
}