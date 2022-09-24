using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Media;

namespace sokocraft_core
{
    public class Game
    {
        public enum Status
        {
            Empty,
            Wall,
            Target
        }

        private Status[,] _grid;
        private List<Position> _crates;
        private Position _player;
        static string _field;
        private int _moveCount;
        private int _level;

        #region Properties
        public List<Position> Crates
        {
            get { return _crates; }
            private set { _crates = value; }
        }

        public Position Player
        {
            get { return _player; }
            private set { _player = value; }
        }

        public int MoveCount
        {
            get { return _moveCount; }
            private set { _moveCount = value; }
        }

        public int Level { get => _level; set => _level = value; }
        #endregion

        public Game(string pLevel)
        {
            int intLevel = Convert.ToInt32(pLevel);
            _grid = new Status[10, 10];
            _moveCount = 0;
            Level = intLevel;
            InitMap(Level);
        }

        private void InitMap(int level)
        {
            Field(level);
            Crates = new List<Position>();
            for (int row = 0; row < 10; row++)
            {
                for (int column = 0; column < 10; column++)
                {
                    switch (_field[row * 10 + column])
                    {
                        case '.':
                            _grid[row, column] = Status.Empty;
                            break;
                        case 'X':
                            _grid[row, column] = Status.Wall;
                            break;
                        case 'o':
                            _grid[row, column] = Status.Target;
                            break;
                        case 'C':
                            _grid[row, column] = Status.Empty;
                            Crates.Add(new Position(row, column));
                            break;
                        case 'P':
                            _grid[row, column] = Status.Empty;
                            Player = new Position(row, column);
                            break;
                        default:
                            column += 2;
                            break;
                    }
                }
            }
        }


        public Status Box(int row, int column)
        {
            return _grid[row, column];
        }

        private void Field(int map)
        {
            using StreamReader sr = new StreamReader("fields/level" + map.ToString() + ".txt");
            _field = sr.ReadToEnd();
            sr.Close();

            _field = _field.Replace("\r\n", "");
        }

        public void KeyDown(Key key)
        {
            Position newPos = new Position(Player.X, Player.Y);
            FindNewPos(newPos, key);

            if (!BoxOk(newPos, key)) return;
            
            Player = newPos;
            MoveCount++;
        }


        private static void FindNewPos(Position newPos, Key key)
        {
            switch (key)
            {
                case Key.Right:

                    newPos.Y++;
                    break;
                case Key.Left:
                    newPos.Y--;
                    break;
                case Key.Down:
                    newPos.X++;
                    break;
                case Key.Up:
                    newPos.X--;
                    break;
            }
        }

        public void Restart()
        {
            InitMap(Level);
            MoveCount = 0;
        }

        private bool BoxOk(Position newPos, Key key)
        {
            // outside zone
            if ((newPos.X < 0 || newPos.X > 9) || (newPos.Y < 0 || newPos.Y > 9))
                return false;

            //wall ?
            if (_grid[newPos.X, newPos.Y] == Status.Wall)
            {
                return false;
            }

            Position crate = CrateOnBox(newPos);
            // crate ?
            if (crate != null)
            {
                // moveCrate
                Position newPosCrate = new Position(crate.X, crate.Y);
                FindNewPos(newPosCrate, key);

                // crate out of zone
                if ((newPosCrate.X < 0 || newPosCrate.X > 9) || (newPosCrate.Y < 0 || newPosCrate.Y > 9))
                    return false;
                // Wall behind crate ?
                if (_grid[newPosCrate.X, newPosCrate.Y] == Status.Wall)
                {
                    return false;
                }
                // crate behind crate
                else if (CrateOnBox(newPosCrate) != null)
                {
                    return false;
                }
                // nothing behind
                else
                {
                    crate.X = newPosCrate.X;
                    crate.Y = newPosCrate.Y;
                    return true;
                }
            }
            return true;
        }

        private Position CrateOnBox(Position newPos)
        {
            foreach (Position crate in Crates)
            {
                if (crate.X == newPos.X && crate.Y == newPos.Y)
                {
                    return crate;
                }
            }
            return null;
        }

        internal bool GameOver()
        {
            foreach (Position crate in Crates)
            {
                if (_grid[crate.X, crate.Y] != Status.Target)
                    return false;
            }
            return true;
        }
    }
}