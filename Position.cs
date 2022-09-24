namespace sokocraft_core
{
     public class Position
    {
        private int _x;
        private int _y;

        public Position(int pX, int pY)
        {
            X = pX;
            Y = pY;
        }

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }
    }
}