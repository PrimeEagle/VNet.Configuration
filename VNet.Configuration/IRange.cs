using System.Numerics;

// ReSharper disable ClassNeverInstantiated.Global


namespace VNet.Configuration
{
    public interface IRange<out T> where T : INumber<T>
    {
        public T Start { get; }
        public T End { get; }


        public bool IsInRange(double value);
    }
}