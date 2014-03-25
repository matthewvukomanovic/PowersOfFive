using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Powers_Of_Five.Infrastructure;
using Powers_Of_Five.Windows;

namespace Powers_Of_Five
{
    public class MainWindowViewModel : PropertyChangedBase
    {
        private PowerOfFiveReference _reference;
        private readonly Random _random;
        private PowerOfFive _number;
        private bool _showAnswer;
        private List<long> numbersLeft;

        #region HelpNumbers

        private List<string> _helpNumbers;

        public List<string> HelpNumbers
        {
            get { return _helpNumbers; }
            set { SetValue(ref _helpNumbers, value, () => HelpNumbers); }
        }

        #endregion

        public MainWindowViewModel()
        {
            _reference = new PowerOfFiveReference(100);

            var list = new List<string>();
            for (int i = 0; i < 10; i++)
            {
                var m = _reference.PowersOfFives[(i + 1)*10 - 1];
                var v = m.Number + "⁵ = " + m.NumberToTheFifth.ToString("N0");
                list.Add(v);
            }
            HelpNumbers = list;
            _random = new Random();
            
            SetNextNumber();
            _showAnswer = false;
            MinNumber = 1;
            MaxNumber = 99;
            _showCorrect = false;
            _isCorrect = false;
        }

        #region View

        private IMainWindow _view;

        public IMainWindow View
        {
            get { return _view; }
            set { SetValue(ref _view, value, () => View); }
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
            set { SetValue(ref _number, value, () => Number); }
        }       
        
        public bool ShowAnswer
        {
            get { return _showAnswer; }
            set { SetValue(ref _showAnswer, value, () => ShowAnswer); }
        }

        private long _answerNumber;

        public long AnswerNumber
        {
            get { return _answerNumber; }
            set { SetValue(ref _answerNumber, value, () => AnswerNumber); }
        }

        private long _maxNumber;

        public long MaxNumber
        {
            get { return _maxNumber; }
            set { SetValue(ref _maxNumber, value, () => MaxNumber); }
        }

        private long _minNumber;

        public long MinNumber
        {
            get { return _minNumber; }
            set { SetValue(ref _minNumber, value, () => MinNumber); }
        }

        private SimpleCommand _checkAnswer;

        public SimpleCommand CheckAnswer
        {
            get { return _checkAnswer ?? (_checkAnswer = new SimpleCommand(ExecuteCheckAnswer)); }
        }

        private void ExecuteCheckAnswer()
        {
            var a = new PowerOfFive(AnswerNumber);
            ShowText = true;
            PowerText = a.Number.ToString() + "⁵ = " + a.NumberToTheFifth.ToString("N0");
            //Check the answer
            TotalAnswered = TotalAnswered + 1;
            if (Number.Number == AnswerNumber)
            {
                ShowCorrect = true;

                IsCorrect = true;
                numbersLeft.Remove(a.Number - 1);
                SetNextNumber();
                CurrentCorrectCount = CurrentCorrectCount + 1;
                HighestCorrectCount = Math.Max(HighestCorrectCount, CurrentCorrectCount);
                TotalCorrect = TotalCorrect + 1;
            }
            else
            {
                ShowCorrect = false;
                IsCorrect = false;
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

        #region IsCorrect

        private bool _isCorrect = false;

        public bool IsCorrect
        {
            get { return _isCorrect; }
            set { SetValue(ref _isCorrect, value, () => IsCorrect); }
        }

        #endregion

        #region ShowCorrect

        private bool _showCorrect = false;

        public bool ShowCorrect
        {
            get { return _showCorrect; }
            set { SetValue(ref _showCorrect, value, () => ShowCorrect); }
        }

        #endregion
        #region ShowHelp

        private bool _showHelp;

        public bool ShowHelp
        {
            get { return _showHelp; }
            set { SetValue(ref _showHelp, value, () => ShowHelp); }
        }

        #region HighestCorrectCount

        private long _highestCorrectCount = 0;

        public long HighestCorrectCount
        {
            get { return _highestCorrectCount; }
            set { SetValue(ref _highestCorrectCount, value, () => HighestCorrectCount); }
        }

        #endregion

        #region CurrentCorrectCount

        private long _currentCorrectCount = 0;

        public long CurrentCorrectCount
        {
            get { return _currentCorrectCount; }
            set { SetValue(ref _currentCorrectCount, value, () => CurrentCorrectCount); }
        }

        #endregion

        #region TotalCorrect

        private long _totalCorrect = 0;

        public long TotalCorrect
        {
            get { return _totalCorrect; }
            set { SetValue(ref _totalCorrect, value, () => TotalCorrect); }
        }

        #endregion

        #region TotalAnswered

        private long _totalAnswered = 0;

        public long TotalAnswered
        {
            get { return _totalAnswered; }
            set { SetValue(ref _totalAnswered, value, () => TotalAnswered); }
        }

        #endregion

        #region PercentageCorrect

        private long _percentageCorrect = 0;

        public long PercentageCorrect
        {
            get { return _percentageCorrect; }
            set { SetValue(ref _percentageCorrect, value, () => PercentageCorrect); }
        }

        #endregion

        #region TotalIncorrect

        private long _totalIncorrect = 0;

        public long TotalIncorrect
        {
            get { return _totalIncorrect; }
            set { SetValue(ref _totalIncorrect, value, () => TotalIncorrect); }
        }

        #endregion

        #region PercentageIncorrect

        private long _percentageIncorrect = 0;

        public long PercentageIncorrect
        {
            get { return _percentageIncorrect; }
            set { SetValue(ref _percentageIncorrect, value, () => PercentageIncorrect); }
        }

        #endregion

        #endregion
        #region ShowText

        private bool _showText = false;

        public bool ShowText
        {
            get { return _showText; }
            set { SetValue(ref _showText, value, () => ShowText); }
        }

        #endregion

        #region PowerText

        private string _powerText;

        public string PowerText
        {
            get { return _powerText; }
            set { SetValue(ref _powerText, value, () => PowerText); }
        }

        #endregion
    }
}
