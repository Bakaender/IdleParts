using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleRush
{
    public class BigNumber
    {
        private const int SignificantDigits = 4;
        private const long ShowScientificAfter = 999999999;
        //Won't work above 19 (long max digits). Or below number of digits in ShowScientificAfter. Or below SignificantDigits.
        private const int DigitsToTrack = 12;
        private static long AddExponentAfter
        {
            get
            {
                long returnNum = 1;
                for (int i = 0; i < DigitsToTrack; i++)
                {
                    returnNum *= 10;
                }
                return returnNum;
            }
        }

        public long Number;
        public int Exponent = 0;

        public string Scientific
        {
            get
            {
                if (Number > ShowScientificAfter)
                {
                    string tempNum = Number.ToString();
                    string returnNum = tempNum[0] + ".";
                    for (int i = 1; i < SignificantDigits; i++)
                    {
                        returnNum += tempNum[i];
                    }
                    returnNum += "e" + Exponent.ToString();
                    return returnNum;
                }
                else
                {
                    return EntireNumber;
                }      
            }
        }

        //Mostly just for testing.
        //Will eventually break depending on how long number is.
        public string EntireNumber
        {
            get
            {
                if (Exponent >= DigitsToTrack)
                {
                    string wholeNum = Number.ToString();
                    for (int i = 0; i <= Exponent - DigitsToTrack; i++)
                    {
                        wholeNum += 0;
                    }
                    return wholeNum;
                }
                else
                {
                    return Number.ToString();
                }            
            }
        }

        public BigNumber(long number = 1)
        {
            InitializeNumber(number);
        }
      
        public BigNumber(long number, int exponent)
        {
            if (exponent < DigitsToTrack)
            {
                while (number.ToString().Length <= exponent)
                {
                    number *= 10;
                }
                while (number.ToString().Length > exponent + 1)
                {
                    number /= 10;
                }
            }
            else
            {
                while (number.ToString().Length < DigitsToTrack)
                {
                    number *= 10;
                }
                while (number.ToString().Length > DigitsToTrack)
                {
                    number /= 10;
                }
            }

            Number = number;
            Exponent = exponent;
        }

        private void InitializeNumber(long number)
        {
            Exponent = 0;

            while (number >= AddExponentAfter)
            {
                Exponent++;
                number /= 10;
            }

            Number = number;

            while (number >= 10)
            {
                Exponent++;
                number /= 10;
            }
        }

        public void MultiplyNumberBy(float percent)
        {
            if (Exponent >= DigitsToTrack - 1)
            {
                Number = (long)((double)Number * percent);
                while (Number.ToString().Length > DigitsToTrack)
                {
                    Exponent++;
                    Number /= 10;
                }
                while (Number.ToString().Length < DigitsToTrack && Exponent > 0)
                {
                    Exponent--;
                    Number *= 10;
                }
            }
            else
            {
                Number = (long)((double)Number * percent);
                InitializeNumber(Number);
            }
        }

        #region operators

        public static BigNumber operator +(BigNumber bigNumber1, BigNumber bigNumber2)
        {
            BigNumber result = new BigNumber();
            if (bigNumber1.Exponent < DigitsToTrack -1 && bigNumber2.Exponent < DigitsToTrack -1)
            {
                result.Number = bigNumber1.Number + bigNumber2.Number;
                string num = result.Number.ToString();
                result.Exponent = num.Length - 1;
                if (num.Length > DigitsToTrack)
                {
                    result.Number = long.Parse(num.Substring(0, DigitsToTrack));
                }
            }
            else
            {
                if (bigNumber1.Exponent > bigNumber2.Exponent)
                {
                    int shiftDigits = bigNumber1.Exponent - bigNumber2.Exponent;
                    if (shiftDigits >= DigitsToTrack) //Number is to small to change the tracked digits
                    {
                        result = bigNumber1;
                    }
                    else
                    {
                        int digitsToUse = DigitsToTrack - shiftDigits;
                        bigNumber2.Number = long.Parse(bigNumber2.Number.ToString().Substring(0, digitsToUse));

                        result.Exponent = bigNumber1.Exponent;
                        result.Number = bigNumber1.Number + bigNumber2.Number;
                        while (result.Number.ToString().Length > DigitsToTrack)
                        {
                            result.Exponent++;
                            result.Number /= 10;
                        }
                    }
                }
                else if (bigNumber1.Exponent < bigNumber2.Exponent)
                {
                    int shiftDigits = bigNumber2.Exponent - bigNumber1.Exponent;
                    if (shiftDigits >= DigitsToTrack) //Number is to small to change the tracked digits
                    {
                        result = bigNumber2;
                    }
                    else
                    {
                        int digitsToUse = DigitsToTrack - shiftDigits;
                        bigNumber1.Number = long.Parse(bigNumber1.Number.ToString().Substring(0, digitsToUse));

                        result.Exponent = bigNumber2.Exponent;
                        result.Number = bigNumber2.Number + bigNumber1.Number;
                        while (result.Number.ToString().Length > DigitsToTrack)
                        {
                            result.Exponent++;
                            result.Number /= 10;
                        }
                    }
                }
                else //Same size exponent
                {
                    result.Exponent = bigNumber1.Exponent;
                    result.Number = bigNumber1.Number + bigNumber2.Number;
                    while (result.Number.ToString().Length > DigitsToTrack)
                    {
                        result.Exponent++;
                        result.Number /= 10;
                    }
                }
            }
            
            return result;
        }

        public static BigNumber operator -(BigNumber bigNumber1, BigNumber bigNumber2)
        {
            //Initialize to negative number.
            BigNumber result = new BigNumber();

            if (bigNumber1.Exponent < DigitsToTrack - 1 && bigNumber2.Exponent < DigitsToTrack - 1)
            {
                result.Number = bigNumber1.Number - bigNumber2.Number;
                if (result.Number < 0)
                {
                    result.Number = -1;
                    result.Exponent = 0;
                    //TODO implement if ever want to track into negative numbers.
                }
                else
                {
                    result.Exponent = result.Number.ToString().Length - 1;
                }
            }
            else
            {
                if (bigNumber1.Exponent > bigNumber2.Exponent)
                {
                    //Number1 bigger than number2.
                    int lengthDif = bigNumber1.Exponent - bigNumber2.Exponent;
                    if (lengthDif < DigitsToTrack)
                    {
                        int digitsToUse = DigitsToTrack - lengthDif;
                        result.Number = long.Parse(bigNumber2.Number.ToString().Substring(0, digitsToUse));
                        result.Number = bigNumber1.Number - result.Number;
                        result.Exponent = bigNumber1.Exponent;
                        while (result.Number.ToString().Length < DigitsToTrack)
                        {
                            result.Exponent--;
                            result.Number *= 10;
                        }
                    }
                    else //number2 to small to change number1
                    {
                        result = bigNumber1;
                    }
                }
                else if (bigNumber1.Exponent < bigNumber2.Exponent)
                {
                    result.Exponent = 0;
                    result.Number = -1;
                    //TODO implement if ever want to track into negative numbers.
                }
                else
                {
                    result.Number = bigNumber1.Number - bigNumber2.Number;
                    if (result.Number < 0)
                    {
                        result.Number = -1;
                        result.Exponent = 0;
                        //TODO implement if ever want to track into negative numbers.
                    }
                    else
                    {
                        result.Exponent = bigNumber1.Exponent;
                        while (result.Number.ToString().Length < DigitsToTrack)
                        {
                            result.Exponent--;
                            result.Number *= 10;
                        }
                    }
                }
            }
             
            return result;
        }

        public static bool operator <(BigNumber BigNumber1, BigNumber BigNumber2)
        {
            if (BigNumber1.Exponent < BigNumber2.Exponent)
            {
                return true;
            }
            else if (BigNumber1.Exponent == BigNumber2.Exponent)
            {
                if (BigNumber1.Number < BigNumber2.Number)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool operator >(BigNumber BigNumber1, BigNumber BigNumber2)
        {
            if (BigNumber1.Exponent > BigNumber2.Exponent)
            {
                return true;
            }
            else if (BigNumber1.Exponent == BigNumber2.Exponent)
            {
                if (BigNumber1.Number > BigNumber2.Number)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool operator <=(BigNumber BigNumber1, BigNumber BigNumber2)
        {
            if (BigNumber1.Exponent < BigNumber2.Exponent)
            {
                return true;
            }
            else if (BigNumber1.Exponent == BigNumber2.Exponent)
            {
                if (BigNumber1.Number < BigNumber2.Number)
                {
                    return true;
                }
                else if (BigNumber1.Number == BigNumber2.Number)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool operator >=(BigNumber BigNumber1, BigNumber BigNumber2)
        {
            if (BigNumber1.Exponent > BigNumber2.Exponent)
            {
                return true;
            }
            else if (BigNumber1.Exponent == BigNumber2.Exponent)
            {
                if (BigNumber1.Number > BigNumber2.Number)
                {
                    return true;
                }
                else if (BigNumber1.Number == BigNumber2.Number)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool operator ==(BigNumber bigNumber1, BigNumber bigNumber2)
        {
            if (bigNumber1.Exponent == bigNumber2.Exponent)
            {
                if (bigNumber1.Number == bigNumber2.Number)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(BigNumber bigNumber1, BigNumber bigNumber2)
        {
            if (bigNumber1.Exponent == bigNumber2.Exponent)
            {
                if (bigNumber1.Number == bigNumber2.Number)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            BigNumber test = obj as BigNumber;
            if (test != null)
            {
                return test == this;
            }
            return false;
        }

        public override int GetHashCode()
        {
            //Not certain this can't be same with different numbers. Maybe should multiply one number by something.
            return string.Format("{0}_{1}", Number, Exponent).GetHashCode();
            //return base.GetHashCode();
        }

        #region Int Comparisons

        public static bool operator <(BigNumber BigNumber1, int num2)
        {
            if (BigNumber1.Number < num2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >(BigNumber BigNumber1, int num2)
        {
            if (BigNumber1.Number > num2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator <=(BigNumber BigNumber1, int num2)
        {
            if (BigNumber1.Number <= num2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator >=(BigNumber BigNumber1, int num2)
        {
            if (BigNumber1.Number >= num2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #endregion
    }
}