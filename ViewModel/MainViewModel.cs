using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using minesweeper.Commands;
using minesweeper.Model;

namespace minesweeper.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private GameBoard _gameBoard;

        private int _flagCount;
        private int _remainingCells;
        private bool _isGameOver;
        private bool _isGameEnd;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand LeftClickCommand { get; }
        public ICommand RightClickCommand { get; }
        public ICommand ResetClickCommand { get; }

        public int RemainingMines
        {
            get
            {
                return GameBoard.MineCount - _flagCount;
            }
        }

        public ObservableCollection<CellViewModel> Cells { get; set; }
        public MainViewModel()
        {
            
            _gameBoard = new GameBoard();
            Cells = new ObservableCollection<CellViewModel>();
            LeftClickCommand = new RelayCommand(LeftClick);
            RightClickCommand = new RelayCommand(RightClick);
            ResetClickCommand = new RelayCommand(execute => ResetGame());
            _isGameEnd = false;
            InitializeGame();
        }
        private void InitializeGame()
        {
            _flagCount = 0;
            _remainingCells = GameBoard.GridSize * GameBoard.GridSize - GameBoard.MineCount;
            _isGameOver = false;
            _isGameEnd = false;
            _gameBoard.Initialize();
            Cells = new ObservableCollection<CellViewModel>();
            for (int row = 0; row < GameBoard.GridSize; row++)
            {
                for (int col = 0; col < GameBoard.GridSize; col++)
                {
                    Cells.Add(new CellViewModel(_gameBoard.Cells[row, col]));
                }
            }

            OnPropertyChanged(nameof(Cells));
            OnPropertyChanged(nameof(RemainingMines));
        }
        public void ResetGame()
        {
            InitializeGame();
        }
        private void LeftClick(object parameter)
        {
            if (_isGameEnd)
                return;
            CellViewModel cellVM = parameter as CellViewModel;
            if (cellVM == null || cellVM.IsFlagged || cellVM.IsRevealed)
                return;
            if (!_gameBoard.IsMinePlaced)
            {
                _gameBoard.PlaceMine(cellVM.Row, cellVM.Col);
            }
           RevealCell(cellVM.Row, cellVM.Col);
        }

        private void RightClick(object parameter)
        {
            if (_isGameEnd)
                return;
            CellViewModel cellVM = parameter as CellViewModel;
            if (cellVM == null || cellVM.IsRevealed)
                return;
            cellVM.IsFlagged = !cellVM.IsFlagged;
            if(cellVM.IsFlagged)
                _flagCount++;
            else
                _flagCount--;
            OnPropertyChanged(nameof(RemainingMines));
        }

        private void RevealCell(int row, int col)
        {
            if (_isGameEnd) 
                return;
            CellViewModel cellVM = GetCorrectCellVM(row, col);
            if (cellVM.IsRevealed || cellVM.IsFlagged)
                return;
            cellVM.IsRevealed = true;
            if (cellVM.IsMine)
            {
                GameResult(false);
                return;
            }

            _remainingCells--;
            if (cellVM.AdjacentMines == 0)
            {
                for (int nearRow = -1; nearRow <= 1; nearRow++)
                {
                    for (int nearCol = -1; nearCol <= 1; nearCol++)
                    {
                        if (nearRow == 0 && nearCol == 0)
                            continue;
                        RevealCell(row + nearRow, col + nearCol);
                    }
                }
            }
            if (_remainingCells == 0)
                GameResult(true);

        }

        private void GameResult(bool result)
        {
            _isGameOver = true;
            foreach (var cellVM in Cells)
            {
                if(cellVM.IsMine)
                {
                    cellVM.IsRevealed = true;
                    cellVM.UpdateDisplay();
                }
            }

            if(result)
            {
                MessageBox.Show("우승");
            }
            else
            {
                MessageBox.Show("패배");
            }
            _isGameEnd = true;
        }
        private CellViewModel GetCorrectCellVM(int row, int col)
        {
            int index = row * GameBoard.GridSize + col;
            return Cells[index];
        }

    }
}
