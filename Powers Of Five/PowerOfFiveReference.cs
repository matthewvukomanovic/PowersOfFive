using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Powers_Of_Five
{
    public class PowerOfFiveReference
    {
        public PowerOfFiveReference(long upto)
        {
            _powersOfFive = new long[upto];
            _powersOfFives = new PowerOfFive[upto];
            for (var i = 0L; i < upto; i++)
            {
                _powersOfFives[i] = new PowerOfFive(i + 1);
                _powersOfFive[i] = _powersOfFives[i].NumberToTheFifth;
            }
        }

        private readonly long[] _powersOfFive;

        public long[] PowersOfFive { get { return _powersOfFive; } }

        private readonly PowerOfFive[] _powersOfFives;

        public PowerOfFive[] PowersOfFives
        {
            get { return _powersOfFives; }
        }
    }

    public class PowerOfFive
    {
        private readonly long _number;

        public long Number
        {
            get { return _number; }
        }

        private readonly long _numberToTheFifth;

        public long NumberToTheFifth
        {
            get { return _numberToTheFifth; }
        }

        public PowerOfFive( long number)
        {
            _number = number;
            _numberToTheFifth = ToTheFifth(number);
        }

        public static long ToTheFifth(long number)
        {
            var square = number * number;
            var cubed = square * square;
            var final = cubed * number;
            return final;
        }
    }
}
