using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace Soduko_App.Game_Logic
{
    class SodukoSolver
    {
        private const string SAVE_ADDRESS = "PuzzleAns";
        private SequentialNumberGenerator _generator = new SequentialNumberGenerator();
        private int[,] _currentSolveState = new int[9, 9];
        private static uint Timeout = 100000;
        private const uint _scalingFactor = 1000;

        public static void SetTimeout(uint timeout)
        {
            Timeout = timeout * _scalingFactor;
        }

        public static uint GetTimeout()
        {
            return Timeout / _scalingFactor;
        }


        public SodukoSolver()
        {
            _generator.SetRange(1, 9);
        }

        public void SaveState()
        {
            var state = new ApplicationDataCompositeValue();
            StringBuilder sb = new StringBuilder();

            foreach(int i in _currentSolveState)
            {
                sb.Append(i);
                sb.Append(";");
            }
            string finalSaveData = sb.ToString();

            Serilizer.SaveDataToAddress(SAVE_ADDRESS, finalSaveData);
        }

        public void LoadState()
        {
            string data = (string)Serilizer.RestoreDataFromAddress(SAVE_ADDRESS);
            string[] pieceData = data.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (81 != pieceData.Length)
                throw new IOException();

            int loc = 0;
            for (int i = 0; i < 9; ++i)
            {
                for (int j = 0; j < 9; ++j)
                {
                    _currentSolveState[i, j] = Int32.Parse(pieceData[loc]);
                    ++loc;
                }
            }
        }

        public int[,] BruteSolve(int[,] states)
        {
            for (uint i = 0; i < Timeout; ++i)
            {
                if (IsSolvable(states, 9))
                {
                    return GetSolvableState();
                }
            }

            return null;
        }

        public int[,] GetSolvableState()
        {
            return _currentSolveState;
        }
        
        public bool IsSolvable(int[,] states,int rows)
        {
            int[,] tempStates = (int[,])states.Clone();

            int blankPieceIndex = 0;

            // Just a simple guess and check algorithim.
            // Do the entire check 3 times for improved accuracy.
            
            for (int c = 0; c < 3; ++c)
            {
                for (int i = 0; i < rows; ++i)
                {
                    for (int j = 0; j < rows; ++j)
                    {
                        if (tempStates[i, j] == -1)
                        {
                            _generator.ResetRandomNumber();
                            int value = _generator.GetNext();
                            int counter = 0;

                            while (!EnsureValidMove(tempStates, i, j, value, rows) && counter < 10)
                            {
                                value = _generator.GetNext();
                                ++counter;
                            }
                            if (counter >= 10)
                            {
                                return false;
                            }
                            tempStates[i, j] = value;
                            ++blankPieceIndex;
                        }
                    }
                }
            }
            // Cache the puzzle's solve state in case the user ever wants it.
            _currentSolveState = (int[,])tempStates.Clone();
            return true;
        }

        public int GetNumberOfState(int[,] states,int state, int rows)
        {
            int count = 0;

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < rows; ++j)
                {
                    if (states[i, j] == state)
                    {
                        ++count;
                    }
                }
            }

            return count;
        }

        public bool AreSpacesEmpty(int[,] states,int rows)
        {
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < rows; ++j)
                {
                    if (states[i, j] == -1)
                        return false;
                }
            }
            return true;
        }

        private bool EnsureValidMove(int[,] states, int pieceRow, int pieceCol, int value, int _rows)
        {
            bool[] row = new bool[9];
            bool[] col = new bool[9];
            bool[] box = new bool[9];


            row[value - 1] = true;
            col[value - 1] = true;
            box[value - 1] = true;

            // Check that the column is valid.
            for (int i = 0; i < _rows; ++i)
            {
                if (states[pieceRow, i] == -1)
                    continue;
                if (col[states[pieceRow, i] - 1] == true)
                {
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
                        return false;
                    }
                    box[states[i, j] - 1] = true;
                }
            }


            return true;
        }
    }

    class SequentialNumberGenerator
    {
        public SequentialNumberGenerator()
        {
        }

        /// <summary>
        /// Sets the range of the random numbers to be gotten.
        /// </summary>
        /// <param name="min">The minimum value, this can be included in a GetNext.</param>
        /// <param name="max">The maximum value, and this can also be inlcuded in a GetNext</param>
        public void SetRange(int min, int max)
        {
            _minRange = min;
            _maxRange = max;
            _currentNumber = _rand.Next(_minRange, _maxRange);
        }

        public void ResetRandomNumber()
        {
            _currentNumber = _rand.Next(_minRange, _maxRange);
        }

        public int GetNext()
        {
            if (_currentNumber > _maxRange)
            {
                _currentNumber = _minRange;
            }
            return _currentNumber++;
        }

        private int _currentNumber;
        private int _minRange;
        private int _maxRange;
        private Random _rand = new Random();
    };
}
