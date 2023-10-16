// ReSharper disable ClassNeverInstantiated.Global

namespace VNet.Configuration
{
    public interface IRange
    {
        public object Start { get; }
        public object End { get; }



        public bool IsInRange(double value);
    }
}