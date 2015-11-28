using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage;
using System.IO;

namespace Soduko_App.Game_Logic
{
    enum Difficulty { Easy, Normal, Hard };
    class SodukoPuzzle
    {
        //TODO:
        // Give this random number generator a seed.
        private Random _random = new Random();

        /// <summary>
        /// Does basic initialization of the soduko puzzle.
        /// </summary>
        /// <param name="colsAndRows">Represents the columns as well as the rows.</param>
        /// <param name="canvas">The height and the width must be equal.</param>
        public SodukoPuzzle(int colsAndRows, Canvas canvas, HintMode hintMode)
        {
            _hintMode = hintMode;
            _showSelfHint = true;

            if (canvas.Height != canvas.Width)
                throw new ArgumentException("Canvas height must equal width!");
            if (colsAndRows == 0)
                throw new ArgumentNullException("colsAndRows");

            _canvas = canvas;
            _rows = colsAndRows;

            _sodukoPieces = new SodukoPiece[_rows, _rows];
            _currentValues = new int[_rows, _rows];
            _startingState = new int[_rows, _rows];

            _blankState = new int[_rows, _rows];
            for (int row = 0; row < _rows; ++row)
            {
                for (int col = 0; col < _rows; ++col)
                {
                    // So the values are rendered as undef.
                    _blankState[row, col] = -1;
                }
            }
        }

        /// <summary>
        /// Actually creates the puzzle and adds the pieces to the canvas.
        /// </summary>
        /// <returns></returns>
        public bool InitPuzzle(Difficulty d)
        {
            // The canvas is symmetrical.
            int canvasLength = (int)_canvas.Height;
            _pieceLength = canvasLength / _rows;

            int numberOfDefinedPieces = GetNumberOfDefinedNumbers(d, _rows * _rows);
            int[,] puzzleValues = GeneratePieceValues(numberOfDefinedPieces);

            // Ensure that this is a two rank array.
            if (puzzleValues.Rank != 2)
                return false;

            uint numberOfTries = 0;
            while (!_solver.IsSolvable(_currentValues, _rows))
            {
                puzzleValues = GeneratePieceValues(numberOfDefinedPieces);
                ++numberOfTries;
            }
            InitPuzzlePieces(puzzleValues);
            _startingState = (int[,])puzzleValues.Clone();

            return true;
        }

        // Init the puzzle with clear values. ( Used in the solver mode ).
        public bool InitPuzzle()
        {
            // The canvas is symmetrical.
            int canvasLength = (int)_canvas.Height;
            _pieceLength = canvasLength / _rows;

            int[,] puzzleValues = new int[9, 9];
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    puzzleValues[i, j] = -1;
                }
            }

            InitPuzzlePieces(puzzleValues);
            _startingState = (int[,])puzzleValues.Clone();

            return true;
        }

        public bool SoftInit()
        {
            int canvasLength = (int)_canvas.Height;
            _pieceLength = canvasLength / _rows;
            InitPuzzlePieces(_currentValues);

            return true;
        }

        private void InitPuzzlePieces(int[,] puzzleValues)
        {
            int vTicker = 0;
            int rTicker = 0;
            bool useBlue = true;
            bool startUseBlue = useBlue;
            const int boxSize = 3;
            _currentValues = puzzleValues;

            for (int row = 0; row < _rows; ++row)
            {
                if (rTicker % 3 == 0 && rTicker != 0)
                    startUseBlue = !startUseBlue;
                useBlue = startUseBlue;
                vTicker = 0;
                for (int col = 0; col < _rows; ++col)
                {
                    int pieceVal = puzzleValues[row, col];
                    bool isDefined = false;
                    
                    if (pieceVal != -1)
                    {
                        isDefined = true;
                    }
                    if (_sodukoPieces[row,col] != null)
                    {
                        // This means we are restoring so we can use the preset values.
                        isDefined = _sodukoPieces[row, col].IsPresetPiece;
                    }
                    var piece = new SodukoPiece(isDefined);
                    piece.OnTappedEvent += OnPieceTapped;
                    piece.SetScaling(_pieceLength);
                    piece.SetPosition(row, col);
                    if (vTicker % boxSize == 0 && vTicker != 0)
                        useBlue = !useBlue;
                    piece.SetBorderBrush(useBlue);

                    _sodukoPieces[row, col] = piece;

                    if (pieceVal != -1)
                    {
                        piece.NumberValue = pieceVal;
                    }

                    _canvas.Children.Add(piece);
                    ++vTicker;
                }
                ++rTicker;
            }
        }

        //BUG:
        // For some odd reason this function doesn't work it
        // makes all operations involving the UI just silently fail.
        // This all happens if this function is only called once on during startup.
        private void AddPuzzlePieces(int[,] puzzleValues)
        {
            int vTicker = 0;
            int rTicker = 0;
            bool useBlue = true;
            const int boxSize = 3;
            bool startUseBlue = useBlue;
            _canvas.Children.Clear();
            for (int row = 0; row < _rows; ++row)
            {
                if (rTicker % 3 == 0 && rTicker != 0)
                    startUseBlue = !startUseBlue;
                useBlue = startUseBlue;
                vTicker = 0;
                for (int col = 0; col < _rows; ++col)
                {
                    bool preDefined = false;
                    if (_startingState[row, col] != -1)
                    {
                        preDefined = true;
                    }
                    SodukoPiece p = new SodukoPiece(preDefined);

                    p.SetScaling(_pieceLength);
                    p.SetPosition(row, col);
                    if (vTicker % boxSize == 0 && vTicker != 0)
                        useBlue = !useBlue;
                    p.SetBorderBrush(useBlue);
                    p.OnTappedEvent += OnPieceTapped;
                    p.NumberValue = puzzleValues[row, col];
                    _canvas.Children.Add(p);


                    ++vTicker;
                }
                ++rTicker;
            }
        }

        public void ResetPuzzle()
        {
            _canvas.Children.Clear();
            _sodukoPieces = new SodukoPiece[_rows, _rows];
            InitPuzzlePieces(_startingState);
        }

        private int[,] GeneratePieceValues(int definedPieces)
        {
            int[,] a = (int[,])_blankState.Clone();
            _currentValues = (int[,])_blankState.Clone();

            for (int row = 0; row < _rows; ++row)
            {
                for (int col = 0; col < _rows; ++col)
                {
                    // So the values are rendered as undef.
                    a[row, col] = -1;
                    _currentValues[row, col] = -1;
                }
            }

            int randCol = 0;
            int randRow = 0;
            for (int i = 0; i < definedPieces; ++i)
            {
                randCol = _random.Next(_rows);
                randRow = _random.Next(_rows);

                // Then fill it with one of the valid numbers.
                int valueOfPiece = _random.Next(1, 10);
                while ( a[randRow,randCol] != -1 || !EnsureValidMove(_currentValues, randRow, randCol, valueOfPiece)  )
                {
                    randCol = _random.Next(_rows);
                    randRow = _random.Next(_rows);
                    valueOfPiece = _random.Next(1, 10);
                }
                a[randRow, randCol] = valueOfPiece;
                _currentValues[randRow, randCol] = valueOfPiece;
            }

            return a;
        }

        private int GetNumberOfDefinedNumbers(Difficulty d, int numberOfPieces)
        {
            Random r = new Random();
            int l1 = 5;
            int l2 = 10;
            int l3 = 20;
            int l4 = 28;
            if (d == Difficulty.Hard)
            {
                return r.Next(l3, l4);
            }
            else if (d == Difficulty.Normal)
            {
                return r.Next(l2, l3);
            }

            else
            {
                return r.Next(l1, l2);
            }
        }

        public void OnPieceTapped(object sender, TappedRoutedEventArgs e)
        {
            ShowNumberSelector(sender, EventArgs.Empty);
        }

        public bool CheckForWin()
        {
            // We have already been constantly checking the validity of the moves through EnsureValidMove(...)
            // so we can just check that there are no empty pieces.
            if (_solver.AreSpacesEmpty(_currentValues, _rows))
                return true;

            return false;
        }
        
        public bool EnsureValidMove(SodukoPiece p,int value, bool lightUpInvalidPiece)
        {
            int blockingCol;
            int blockingRow;

            bool result = EnsureValidMove(_currentValues, p.Row, p.Col, value, out blockingRow, out blockingCol);
            if (!result && lightUpInvalidPiece)
            {
                _sodukoPieces[blockingRow, blockingCol].LightPiece();
            }


            return result;
        }

        public bool EnsureValidMove(int[,] states, int pieceRow, int pieceCol, int value)
        {
            int g1, g2;
            return EnsureValidMove(states, pieceRow, pieceCol, value, out g1, out g2);
        }

        public bool EnsureValidMove(int[,] states, int pieceRow, int pieceCol, int value, out int blockingRow, out int blockingCol)
        {
            bool[] row = new bool[9];
            bool[] col = new bool[9];
            bool[] box = new bool[9];
            

            row[value - 1] = true;
            col[value - 1] = true;
            box[value - 1] = true;
            blockingRow = -1;
            blockingCol = -1;

            // Check that the column is valid.
            for (int i = 0; i < _rows; ++i)
            {
                if (states[pieceRow, i] == -1)
                    continue;
                if (col[states[pieceRow, i] - 1] == true)
                {
                    blockingRow = pieceRow;
                    blockingCol = i;
                    return false;
                }
                col[states[pieceRow, i] - 1] = true;
            }

            // Check that the row is valid.
            for (int i = 0; i < _rows; ++i)
            {
                if (states[i, pieceCol] == -1)
                    continue;
                if (row[states[i, pieceCol] - 1] == true)
                {
                    blockingRow = i;
                    blockingCol = pieceCol;
                    return false;
                }
                row[states[i, pieceCol] - 1] = true;
            }

            // Check that the box is valid.
            int rowSector = 0;
            if (pieceRow < 3)
                rowSector = 0;
            else if (pieceRow < 6)
                rowSector = 1;
            else
                rowSector = 2;

            int colSector = 0;
            if (pieceCol < 3)
                colSector = 0;
            else if (pieceCol < 6)
                colSector = 1;
            else
                colSector = 2;

            int lowerRowBounds = rowSector * 3;
            int higherRowBounds = (rowSector + 1) * 3;

            int lowerColBounds = colSector * 3;
            int higherColBounds = (colSector + 1) * 3;
            for (int i = lowerRowBounds; i < higherRowBounds; ++i)
            {
                for (int j = lowerColBounds; j < higherColBounds; ++j)
                {
                    if (states[i, j] == -1)
                        continue;
                    if (box[states[i, j] - 1] == true)
                    {
                        blockingRow = i;
                        blockingCol = j;
                        return false;
                    }
                    box[states[i, j] - 1] = true;
                }
            }
            

            return true;
        }

        public void SaveState()
        {
            Serilizer.SavePuzzleState(_sodukoPieces, _startingState, _currentValues);
            _solver.SaveState();
        }

        public void RestoreState()
        {
            var state = Serilizer.GetPuzzleState();
            string piecesTextRep = (string)state["Pieces"];
            string initPosTextRep = (string)state["OriginalState"];
            string currentPosTextRep = (string)state["CurrentPos"];

            string[] pieceData = piecesTextRep.Split(new char[] { ';' },StringSplitOptions.RemoveEmptyEntries);
            if (_rows * _rows != pieceData.Length)
                throw new IOException();
            int loc = 0;
            for (int i = 0; i < _rows; ++i)
            {
                for (int j = 0; j < _rows; ++j)
                {
                    SodukoPiece p = new SodukoPiece(false);
                    p.InitFromInfoString(pieceData[loc]);
                    _sodukoPieces[i,j] = p;
                    ++loc;
                }
            }

            string[] initPosData = initPosTextRep.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            loc = 0;
            for (int i = 0; i < _rows; ++i)
            {
                for (int j = 0; j < _rows; ++j)
                {
                    _startingState[i, j] = int.Parse(initPosData[loc]);
                    ++loc;
                }
            }

            string[] currentPosData = currentPosTextRep.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            loc = 0;
            for (int i = 0; i < _rows; ++i)
            {
                for (int j = 0; j < _rows; ++j)
                {
                    _currentValues[i, j] = int.Parse(currentPosData[loc]);
                    ++loc;
                }
            }

            _solver.LoadState();
        }

        public void SetHintMode(HintMode newHintMode,SodukoPiece selectedPiece)
        {
            if (_hintMode == HintMode.Everything && newHintMode != HintMode.Everything)
            {
                ClearAllSuggestionText();
            }

            if (selectedPiece != null && _hintMode == HintMode.Adjacent && newHintMode != HintMode.Adjacent)
            {
                ClearNeighborsSuggestionText(selectedPiece);
            }

            if (newHintMode == HintMode.Everything && _hintMode != HintMode.Everything)
            {
                _hintMode = newHintMode;
                SetSuggestions(0, 0);
                return;
            }
            else if (newHintMode == HintMode.Adjacent && _hintMode != HintMode.Adjacent)
            {
                _hintMode = newHintMode;
                if (selectedPiece != null)
                    SetSuggestions(selectedPiece);
                return;
            }
            else if (newHintMode == HintMode.Off && selectedPiece != null)
            {
                selectedPiece.SetSuggestionTextToNothing();
            }
            

            _hintMode = newHintMode;
        }

        public void SetSelfHint(bool selfHint,SodukoPiece selectedPiece)
        {
            if (selfHint != _showSelfHint)
            {
                // _showSelfHint has changed.
                if (!selfHint && _hintMode != HintMode.Everything)
                {
                    selectedPiece.SetSuggestionTextToNothing();
                }
                else if (selfHint && _hintMode != HintMode.Off)
                {
                    SetSuggestions(selectedPiece);
                }

                _showSelfHint = selfHint;
            }
        }

        public void SetPiece(int row, int col, int value)
        {
            _currentValues[row, col] = value;
        }

        public void SetSuggestions(SodukoPiece sp)
        {
            if (_hintMode == HintMode.Off)
                return;
            SetSuggestions(sp.Row, sp.Col);
        }

        public void SetSuggestions(int row, int col)
        {
            if (_hintMode == HintMode.Adjacent)
            {
                // Iterate through all of the neighbors of the piece.
                if (_showSelfHint)
                {
                    SetSuggestionsOfPiece(row, col);
                }
                if (row + 1 < 9)
                    SetSuggestionsOfPiece(row + 1, col);
                if (row - 1 > -1)
                    SetSuggestionsOfPiece(row - 1, col);
                if (col + 1 < 9)
                    SetSuggestionsOfPiece(row, col + 1);
                if (col - 1 > -1)
                    SetSuggestionsOfPiece(row, col - 1);
            }

            else if (_hintMode == HintMode.Everything)
            {
                for (int i = 0; i < _rows; ++i)
                {
                    for (int j = 0; j < _rows; ++j)
                    {
                        SetSuggestionsOfPiece(i, j);
                    }
                }
            }
        }


        /// <summary>
        /// Sets the suggestion text based on if certian combinations work. 
        /// Doesn't set the suggestion text if the piece is already set.
        /// </summary>
        /// <param name="row">Row of piece to set.</param>
        /// <param name="col">Col of piece to set.</param>
        private void SetSuggestionsOfPiece(int row, int col)
        {
            List<int> suggestions = GetSuggestionsOfPiece(row, col);
            if (suggestions != null)
            {
                _sodukoPieces[row, col].SetSuggestionText(suggestions.ToArray());
            }
        }

        private List<int> GetSuggestionsOfPiece(int row, int col)
        {
            // Ensure that the piece isn't already set.
            if (_currentValues[row, col] != -1)
            {
                return null;
            }
            List<int> suggestions = new List<int>();
            // Just test the piece with every value and see what works.
            for (int i = 1; i < 10; ++i)
            {
                if (EnsureValidMove(_currentValues, row, col, i))
                    suggestions.Add(i);
            }

            return suggestions;
        }

        public void ClearNeighborsSuggestionText(SodukoPiece p)
        {
            int row = p.Row;
            int col = p.Col;
            if (row + 1 < 9)
                _sodukoPieces[row + 1, col].SetSuggestionTextToNothing();
            if (row - 1 > -1)
                _sodukoPieces[row - 1, col].SetSuggestionTextToNothing();
            if (col + 1 < 9)
                _sodukoPieces[row, col + 1].SetSuggestionTextToNothing();
            if (col - 1 > -1)
                _sodukoPieces[row, col - 1].SetSuggestionTextToNothing();
        }

        public void ClearAllSuggestionText()
        {
            for (int i = 0; i < _rows; ++i)
            {
                for (int j = 0; j < _rows; ++j)
                {
                    _sodukoPieces[i, j].SetSuggestionTextToNothing();
                }
            }
        }

        public HintMode GetHintMode()
        {
            return _hintMode;
        }

        public void SolvePuzzle(bool fireOnCompletionEvent)
        {
 
            _currentValues = _solver.GetSolvableState();
            InitPuzzlePieces(_currentValues);
            if (fireOnCompletionEvent)
                OnCompletedGame(this, new EventArgs());
        }

        public void SolveUserPuzzle()
        {
            int[,] tempValues = (int[,])_currentValues.Clone();
            _currentValues = _solver.BruteSolve(_currentValues);
            if (_currentValues == null)
            {
                _currentValues = tempValues;
                // We timed out.
                throw new TimeoutException(SodukoSolver.GetTimeout().ToString());
            }
            InitPuzzlePieces(_currentValues);
        }

        public event EventHandler ShowNumberSelector;
        public event EventHandler OnCompletedGame;

        private bool _showSelfHint;
        private HintMode _hintMode;
        private int[,] _blankState;
        private Canvas _canvas;
        private int _rows;
        private int _pieceLength;
        private SodukoPiece[,] _sodukoPieces;
        private int[,] _startingState;
        private int[,] _currentValues;
        private SodukoSolver _solver = new SodukoSolver();
    }
}
