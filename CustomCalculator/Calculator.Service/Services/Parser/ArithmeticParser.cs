using System;

namespace Calculator.Service.Services.Parser
{
    internal class ArithmeticParser : IParser
    {
        private string _expression;
        private int _position;

        public ArithmeticParser()
        {
            _position = 0;
        }

        // 解析并计算表达式的值
        public double Parse(string expression)
        {
            try
            {
                _expression = expression.Replace(" ", ""); // 去除空格
                _position = 0;

                double result = ParseExpression();
                if (_position < _expression.Length)
                {
                    throw new Exception("Unexpected character at position " + _position);
                }
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        // 解析表达式（加减）
        private double ParseExpression()
        {
            double left = ParseTerm();
            while (_position < _expression.Length)
            {
                char op = _expression[_position];
                if (op == '+' || op == '-')
                {
                    _position++;
                    double right = ParseTerm();
                    if (op == '+')
                        left += right;
                    else
                        left -= right;
                }
                else
                {
                    break;
                }
            }
            return left;
        }

        // 解析项（乘除）
        private double ParseTerm()
        {
            double left = ParseFactor();
            while (_position < _expression.Length)
            {
                char op = _expression[_position];
                if (op == '*' || op == '/')
                {
                    _position++;
                    double right = ParseFactor();
                    if (op == '*')
                        left *= right;
                    else
                        left /= right;
                }
                else
                {
                    break;
                }
            }
            return left;
        }

        // 解析因子（数字或括号表达式）
        private double ParseFactor()
        {
            if (_position >= _expression.Length)
                throw new Exception("Unexpected end of expression");

            char currentChar = _expression[_position];
            if (currentChar == '(')
            {
                _position++; // 跳过 '('
                double result = ParseExpression(); // 递归解析括号内的表达式
                if (_position >= _expression.Length || _expression[_position] != ')')
                    throw new Exception("Expected ')' at position " + _position);
                _position++; // 跳过 ')'
                return result;
            }
            else if (char.IsDigit(currentChar) || currentChar == '.')
            {
                return ParseNumber();
            }
            else
            {
                throw new Exception("Unexpected character at position " + _position);
            }
        }

        // 解析数字
        private double ParseNumber()
        {
            int startPos = _position;
            while (_position < _expression.Length && (char.IsDigit(_expression[_position]) || _expression[_position] == '.'))
            {
                _position++;
            }
            string numberStr = _expression.Substring(startPos, _position - startPos);
            if (double.TryParse(numberStr, out double result))
            {
                return result;
            }
            throw new Exception("Invalid number at position " + startPos);
        }
    }

}
