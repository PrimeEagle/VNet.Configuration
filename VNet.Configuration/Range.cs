using System.ComponentModel;
using System.Numerics;
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace VNet.Configuration
{
    public class Range<T> : IRange<T>, INotifyPropertyChanged where T : INumber<T>,
                            IComparable<T>,
                            IEquatable<T>
    {
        private T _end;
        private T _start;

        public T End
        {
            get => _end;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_end, value)) return;
                _end = value;
                OnPropertyChanged(nameof(End));
            }
        }

        public T Start
        {
            get => _start;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_start, value)) return;
                _start = value;
                OnPropertyChanged(nameof(Start));
            }
        }

        T IRange<T>.Start => this.Start;
        T IRange<T>.End => this.End;

        public Range(T start, T end)
        {
            if (start.CompareTo(end) > 0)
            {
                throw new ArgumentException("Start must be less than or equal to end.", nameof(start));
            }
            _start = start;
            _end = end;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;



        public bool IsInRange(double value)
        {
            return value >= Convert.ToDouble(Start) && value <= Convert.ToDouble(End);
        }
    }
}