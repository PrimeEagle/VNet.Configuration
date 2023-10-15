using System.ComponentModel;
using System.Numerics;

namespace VNet.Configuration
{
    public class Range<T> : INotifyPropertyChanged where T : INumber<T>,
                            IComparable<T>,
                            IEquatable<T>
    {
        private T _start;
        private T _end;

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

        public Range(T start, T end)
        {
            if (start.CompareTo(end) > 0)
            {
                throw new ArgumentException("Start must be less than or equal to end.", nameof(start));
            }
            _start = start;
            _end = end;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}