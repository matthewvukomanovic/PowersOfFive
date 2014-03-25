using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Ross.Infrastructure;
using Ross.Windows;

namespace Powers_Of_Five
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private PowerOfFiveReference _reference;
        private readonly Random _random;
        private PowerOfFive _number;
        private bool _showAnswer;
        private List<long> numbersLeft;

        #region MultiplesOfTen

        private List<string> _multiplesOfTen;

        public List<string> MultiplesOfTen
        {
            get { return _multiplesOfTen; }
            set { SetValueNonTracking(ref _multiplesOfTen, value, () => MultiplesOfTen); }
        }

        #endregion

        public MainWindowViewModel()
        {
            _reference = new PowerOfFiveReference(100);

            var list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var m = _reference.PowersOfFives[(i + 1)*10 - 1];
                var v = m.Number + "^5 = " + m.NumberToTheFifth.ToString("N0");
                list.Add(v);
            }
            MultiplesOfTen = list;
            _random = new Random();
            
            SetNextNumber();
            _showAnswer = false;
            MinNumber = 1;
            MaxNumber = 99;
            _showCorrect = false;
            _showError = false;
        }

        #region View

        private IMainWindow _view;

        public IMainWindow View
        {
            get { return _view; }
            set { SetValueNonTracking(ref _view, value, () => View); }
        }

        #endregion

        protected void SetNextNumber()
        {
            if (numbersLeft == null || numbersLeft.Count == 0)
            {
                numbersLeft = new List<long>(99);
                for (int i = 0; i < 99; i++)
                {
                    numbersLeft.Add(i);
                }
            }
            var offset = _random.Next(0, numbersLeft.Count - 1);
            var original = Number;
            do
            {
                var offsetNumberToUse = numbersLeft[offset];
                Number = _reference.PowersOfFives[offsetNumberToUse];
            } while (Number == original);
        }

        public PowerOfFive Number
        {
            get { return _number; }
            set { SetValueNonTracking(ref _number, value, () => Number); }
        }       
        
        public bool ShowAnswer
        {
            get { return _showAnswer; }
            set { SetValueNonTracking(ref _showAnswer, value, () => ShowAnswer); }
        }

        private long _answerNumber;

        public long AnswerNumber
        {
            get { return _answerNumber; }
            set { SetValueNonTracking(ref _answerNumber, value, () => AnswerNumber); }
        }

        private long _maxNumber;

        public long MaxNumber
        {
            get { return _maxNumber; }
            set { SetValueNonTracking(ref _maxNumber, value, () => MaxNumber); }
        }

        private long _minNumber;

        public long MinNumber
        {
            get { return _minNumber; }
            set { SetValueNonTracking(ref _minNumber, value, () => MinNumber); }
        }

        private SimpleCommand _checkAnswer;

        public SimpleCommand CheckAnswer
        {
            get { return _checkAnswer ?? (_checkAnswer = new SimpleCommand(ExecuteCheckAnswer)); }
        }

        private void ExecuteCheckAnswer()
        {
            var a = new PowerOfFive(AnswerNumber);
            string s = a.Number.ToString() + "^5 = " + a.NumberToTheFifth.ToString("N0");
            //Check the answer
            TotalAnswered = TotalAnswered + 1;
            if (Number.Number == AnswerNumber)
            {
                ShowCorrect = true;
                ShowError = false;
                Error = null;
                CorrectText = "Correct: " + s;
                numbersLeft.Remove(a.Number - 1);
                SetNextNumber();
                CurrentCorrectCount = CurrentCorrectCount + 1;
                HighestCorrectCount = Math.Max(HighestCorrectCount, CurrentCorrectCount);
                TotalCorrect = TotalCorrect + 1;
            }
            else
            {
                ShowCorrect = false;
                ShowError = true;
                Error = s;
                CorrectText = "";
                CurrentCorrectCount = 0;
                TotalIncorrect = TotalIncorrect + 1;
            }

            PercentageCorrect = _totalCorrect*100/_totalAnswered;
            PercentageIncorrect = _totalIncorrect*100/_totalAnswered;

            if (View != null)
            {
                View.SelectTextboxText();
            }
        }

        #region CorrectText

        private string _correctText = "";

        public string CorrectText
        {
            get { return _correctText; }
            set { SetValueNonTracking(ref _correctText, value, () => CorrectText); }
        }

        #endregion
        #region ShowError

        private bool _showError = false;

        public bool ShowError
        {
            get { return _showError; }
            set { SetValueNonTracking(ref _showError, value, () => ShowError); }
        }

        #endregion
        #region Error

        private string _error;

        public string Error
        {
            get { return _error; }
            set { SetValueNonTracking(ref _error, value, () => Error); }
        }

        #endregion
        #region ShowCorrect

        private bool _showCorrect = false;

        public bool ShowCorrect
        {
            get { return _showCorrect; }
            set { SetValueNonTracking(ref _showCorrect, value, () => ShowCorrect); }
        }

        #endregion
        #region ShowHelp

        private bool _showHelp;

        public bool ShowHelp
        {
            get { return _showHelp; }
            set { SetValueNonTracking(ref _showHelp, value, () => ShowHelp); }
        }

        #region HighestCorrectCount

        private long _highestCorrectCount = 0;

        public long HighestCorrectCount
        {
            get { return _highestCorrectCount; }
            set { SetValueNonTracking(ref _highestCorrectCount, value, () => HighestCorrectCount); }
        }

        #endregion

        #region CurrentCorrectCount

        private long _currentCorrectCount = 0;

        public long CurrentCorrectCount
        {
            get { return _currentCorrectCount; }
            set { SetValueNonTracking(ref _currentCorrectCount, value, () => CurrentCorrectCount); }
        }

        #endregion

        #region TotalCorrect

        private long _totalCorrect = 0;

        public long TotalCorrect
        {
            get { return _totalCorrect; }
            set { SetValueNonTracking(ref _totalCorrect, value, () => TotalCorrect); }
        }

        #endregion

        #region TotalAnswered

        private long _totalAnswered = 0;

        public long TotalAnswered
        {
            get { return _totalAnswered; }
            set { SetValueNonTracking(ref _totalAnswered, value, () => TotalAnswered); }
        }

        #endregion

        #region PercentageCorrect

        private long _percentageCorrect = 0;

        public long PercentageCorrect
        {
            get { return _percentageCorrect; }
            set { SetValueNonTracking(ref _percentageCorrect, value, () => PercentageCorrect); }
        }

        #endregion

        #region TotalIncorrect

        private long _totalIncorrect = 0;

        public long TotalIncorrect
        {
            get { return _totalIncorrect; }
            set { SetValueNonTracking(ref _totalIncorrect, value, () => TotalIncorrect); }
        }

        #endregion

        #region PercentageIncorrect

        private long _percentageIncorrect = 0;

        public long PercentageIncorrect
        {
            get { return _percentageIncorrect; }
            set { SetValueNonTracking(ref _percentageIncorrect, value, () => PercentageIncorrect); }
        }

        #endregion

        #endregion
    }
}
