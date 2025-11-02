using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using minesweeper.Model;

namespace minesweeper.ViewModel
{
    public class CellViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        //모델 속성 가져오기
        private Cell _cell;
        private bool _isRevealed;
        private bool _isFlagged;
        public int Row
        {
            get { return _cell.Row; }
        }
        public int Col
        {
            get { return _cell.Column; }
        }
        public bool IsMine
        {
            get { return _cell.IsMine; }
        }
        public int AdjacentMines
        {
            get { return _cell.AdjacentMines; }
        }

        //UI 속성 시각화
        public bool IsEnabled
        {
            get { return !IsRevealed; }
        }
        public string DisplayText
        {
            get
            {
                if (IsFlagged) return "🚩";
                if (!IsRevealed) return "";
                if (IsMine) return "💣";
                if (AdjacentMines > 0) return AdjacentMines.ToString();
                return "";
            }
        }

        public Brush Background
        {
            get
            {
                if (IsFlagged) return Brushes.Yellow;
                if (!IsRevealed) return Brushes.LightGray;
                if (IsMine) return Brushes.Red;
                return Brushes.White;
            }
        }

        public Brush Foreground
        {
            get
            {
                if (!IsRevealed || IsMine || IsFlagged)
                    return Brushes.Black;

                return AdjacentMines switch
                {
                    1 => Brushes.Blue,
                    2 => Brushes.Green,
                    3 => Brushes.Red,
                    4 => Brushes.DarkBlue,
                    5 => Brushes.DarkRed,
                    6 => Brushes.Cyan,
                    7 => Brushes.Black,
                    8 => Brushes.Gray,
                    _ => Brushes.Black
                };
            }
        }

        //UI 상태 변경 속성
        public bool IsRevealed
        {
            get => _isRevealed;
            set
            {
                if (_isRevealed != value)
                {
                    _isRevealed = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(DisplayText));
                    OnPropertyChanged(nameof(Background));
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        public bool IsFlagged
        {
            get => _isFlagged;
            set
            {
                if (_isFlagged != value)
                {
                    _isFlagged = value;
                    OnPropertyChanged(); 
                    OnPropertyChanged(nameof(DisplayText));
                    OnPropertyChanged(nameof(Background));
                }
            }
        }
        void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public CellViewModel(Cell cell)
        {
            _cell = cell;
        }
        public void UpdateDisplay()
        {
            OnPropertyChanged(nameof(DisplayText));
            OnPropertyChanged(nameof(Background));
            OnPropertyChanged(nameof(Foreground));
        }
    }
}
