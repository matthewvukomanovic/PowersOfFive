using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (Number.Number == AnswerNumber)
            {
                ShowCorrect = true;
                ShowError = false;
                Error = null;
                CorrectText = "Correct: " + s;
                numbersLeft.Remove(a.Number - 1);
                SetNextNumber();
            }
            else
            {
                ShowCorrect = false;
                ShowError = true;
                Error = s;
                CorrectText = "";
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

        #endregion
    }
}
