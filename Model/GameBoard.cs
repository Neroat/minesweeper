using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minesweeper.Model
{
    public class GameBoard
    {
        public static int GridSize = 9;
        public static int MineCount = 10;

        public Cell[,] Cells { get; set; }
        public bool IsMinePlaced { get; set; }

        public GameBoard()
        {
            Initialize();
        }
        public void Initialize()
        {
            Cells = new Cell[GridSize, GridSize];
            IsMinePlaced = false;
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Cells[row, col] = new Cell
                    {
                        Row = row,
                        Column = col
                    };
                }
            }
        }

        //지뢰 설치
        public void PlaceMine(int firstClickRow, int firstClickCol)
        {
            if (IsMinePlaced)
                return;
            Random random = new Random();
            int placedMines = 0;
            while(placedMines< MineCount) 
            {
                int row = random.Next(GridSize);
                int col = random.Next(GridSize);
                if (Cells[row, col].IsMine)
                    continue;
                if (row == firstClickRow && col == firstClickCol)
                    continue;

                Cells[row, col].IsMine = true;
                placedMines++;
            }
            IsMinePlaced = true;
            PlaceAdjacentMines();
        }

        //1,2,3,4 등 힌트 설치
        public void PlaceAdjacentMines()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (!Cells[row, col].IsMine)
                        Cells[row,col].AdjacentMines = CountAdjacentMines(row, col);
                }
            }
        }

        //주변 지뢰 갯수 체크
        public int CountAdjacentMines(int row, int col)
        {
            int count = 0;
            for (int nearRow = -1; nearRow <= 1; nearRow++)
            {
                for (int nearCol = -1; nearCol <= 1; nearCol++)
                {
                    if (nearRow == 0 && nearCol == 0)
                        continue;
                    int checkRow = row + nearRow;
                    int checkCol = col + nearCol;
                    if (IsExistCell(checkRow, checkCol) && Cells[checkRow, checkCol].IsMine)
                        count++;
                }
            }
            return count;
        }

        //실존하는 칸인지 체크
        public bool IsExistCell(int row, int col)
        {
            return row>=0 && row < GridSize && col>=0 && col < GridSize;
        }
    }
}
