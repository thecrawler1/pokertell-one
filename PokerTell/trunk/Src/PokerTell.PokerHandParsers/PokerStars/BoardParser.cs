namespace PokerTell.PokerHandParsers.PokerStars
{
    using System.Text.RegularExpressions;

    public class BoardParser : PokerHandParsers.BoardParser
    {
        #region Constants and Fields

        const string BoardPattern = @"Board.*\[(?<Board>(" + SharedPatterns.CardPattern + @" *){0,5}).*\]";

        #endregion

        #region Public Methods

        public override void Parse(string handHistory)
        {
            Match board = MatchBoard(handHistory);
            IsValid = board.Success;
           
            if (IsValid)
            {
                ExtractBoard(board);
            }
        }

        #endregion

        #region Methods

        static Match MatchBoard(string handHistory)
        {
            return Regex.Match(handHistory, BoardPattern, RegexOptions.IgnoreCase);
        }

        void ExtractBoard(Match board)
        {
            Board = board.Groups["Board"].Value;
        }

        #endregion
    }
}