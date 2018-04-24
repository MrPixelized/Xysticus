namespace Chess
{
    public class Game
    {
        public Position currentPosition;

        public Game(Position currentPosition = null)
        {
            if (currentPosition == null) {
                currentPosition = new Position();
            }
            this.currentPosition = currentPosition;
        }        
    }
}
