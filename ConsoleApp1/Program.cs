using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ConsoleApp1
{
    public class Program
    {
        private static IEnumerable<IEnumerable<Cell>> _cells;

        private static void Main()
        {
            _cells = SetRandomCells();
            while (true)
            {
                Console.Clear();
                Console.WriteLine(LiveCells(_cells));
                Thread.Sleep(60);
            }
        }

        private static IEnumerable<IEnumerable<Cell>> SetRandomCells()
        {
            var rand = new Random();
            var count = 30;
            var cells = new List<List<Cell>>();

            for (int i = 0; i < count; i++)
            {
                var cellsRow = new List<Cell>();
                for (int j = 0; j < count; j++)
                {
                    var chance = rand.Next(0, 101);
                    cellsRow.Add(new Cell(chance >= 50, rand.Next(0, 10).ToString()[0]));
                }
                cells.Add(cellsRow);
            }
            return cells;
        }

        private static string LiveCells(IEnumerable<IEnumerable<Cell>> allCells)
        {
            var str = string.Empty;
            var cells = allCells.ToArray();

            var viewCells = CopyCells(allCells).ToArray();

            for (int i = 0; i < cells.Count(); i++)
            {
                var cellRow = cells[i].ToArray();
                var viewCellRow = viewCells[i].ToArray();
                for (int j = 0; j < cellRow.Count(); j++)
                    str += cellRow[j].Live(new List<Cell>() {

                    //up cells
                    i != 0 ? j != 0 ? viewCells[i-1].ToArray()[j-1] : null : null,
                    i != 0 ? viewCells[i-1].ToArray()[j] : null,
                    i != 0 ? j != viewCellRow.Count()-1 ? viewCells[i-1].ToArray()[j+1] : null : null,

                    //middle cells
                    j != 0 ? viewCellRow[j-1] : null,
                    j != viewCellRow.Count()-1 ? viewCellRow[j+1] : null,

                    //down cells
                    i != viewCellRow.Count()-1 ? j != 0 ? viewCells[i+1].ToArray()[j-1] : null : null,
                    i != viewCellRow.Count()-1 ?  viewCells[i+1].ToArray()[j] : null,
                    i != viewCellRow.Count()-1 ? j != viewCellRow.Count()-1 ? viewCells[i+1].ToArray()[j+1] : null : null
                    });

                str += "\n";
            }
            return str;
        }

        private static IEnumerable<IEnumerable<Cell>> CopyCells(IEnumerable<IEnumerable<Cell>> source)
        {
            var cells = new List<List<Cell>>();

            foreach (var cellRow in source)
            {
                var needCellRow = new List<Cell>();
                foreach (var cell in cellRow)
                    needCellRow.Add(cell.Copy());

                cells.Add(needCellRow);
            }
            return cells;
        }
    }

    public class Cell
    {
        private bool _isAlive;

        private readonly char _aliveChar;
        private const char _deadChar = '-';

        public Cell(bool isAlive, char aliveChar)
        {
            _aliveChar = aliveChar;
            _isAlive = isAlive;
        }

        public char Live(IEnumerable<Cell> cellsNerby)
        {
            if (CheckComfortCircs(cellsNerby, !_isAlive))
            {
                _isAlive = true;
                return _aliveChar;
            }
            else
            {
                _isAlive = false;
                return _deadChar;
            }
        }

        public Cell Copy() =>
            new Cell(_isAlive, _aliveChar);

        private bool CheckComfortCircs(IEnumerable<Cell> cellNearby, bool toBurn)
        {
            var aliveCount = 0;
            foreach (var cell in cellNearby)
                if (cell != null && cell._isAlive) aliveCount++;

            if (!toBurn)
                return aliveCount == 2 || aliveCount == 3;
            else
                return aliveCount == 3;
        }
    }
}
