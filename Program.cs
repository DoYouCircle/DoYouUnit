using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DoYouUnit
{
	class Unit
	{
		public List<string> numerator = new List<string>();
		public List<string> denominator = new List<string>();

		public Unit(string input)
		{
			input = Cleanup(input);
			List<string> input_list = ParseMath(input);
			input_list.RemoveAll(s => string.IsNullOrEmpty(s));

			Console.WriteLine("Input: " + String.Join(" ", input_list));
			input_list = ResolveExponents(input_list);
			input_list = ResolveParentheses(input_list);
			Console.WriteLine("Parsed: " + String.Join(" ", input_list));
			ParseUnit(input_list);

			Console.WriteLine("Numerator:  " + String.Join(" ", numerator));
			Console.WriteLine("Denominator:  " + String.Join(" ", denominator));
			Console.WriteLine("\n");
		}

		private List<string> ParseMath(string input)
		{
			return new List<string>(Regex.Split(input, @"([*()\^\/]|(?<!E)[\+\-])"));
		}

		private List<string> ListCleanup(List<string> l)
		{
			return l.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
		}

		private string Cleanup(string input)
		{
			return input.Replace(" ", "");

		}

		private List<string> ResolveExponents(List<string> input)
		{
			// Converting exponents (^2, ^3, ...) to multiplications

			for (int i = 0; i < input.Count; i++)
			{
				if (input[i] == "^")
				{
					string exponent_base = input[i - 1];
					string exponent_raw = input[i + 1];

					// We try to convert the exponent to an int
					// If it fails we shout at the user
					try
                    {
						int exponent = Convert.ToInt32(exponent_raw);

						if (exponent > 1)
						{
							// replace ^ by * and add multiplications
							input[i] = "*";
							exponent_raw = string.Concat(Enumerable.Repeat(exponent_base + '*', exponent - 1));
							exponent_raw = exponent_raw.Remove(exponent_raw.Length - 1);
						}
					}
                    catch (FormatException)
                    {
						Console.WriteLine("The {0} value '{1}' is not in a recognizable format. Use a natural number!", exponent_raw.GetType().Name, exponent_raw);
					}
				}
			}

			// Join and split list again
			string t = string.Join("", input.ToArray());
			return ListCleanup(ParseMath(t));
		}

		private bool isDivision(List<bool> l)
		{
			bool m = false;
			foreach (bool e in l)
			{
				if (e == true)
					m = !m;
			}

			return m;
		}

		private List<string> ResolveParentheses(List<string> input)
		{
			//false -> multiplication, true -> division
			List<string> output = new List<string>(input);

			List<bool> levelList = new List<bool>();
			for (int i = 0; i < input.Count; i++)
			{
				if (input[i] == "(")
				{
					// if new bracket starts, it is deleted and in levelList it is saved if the current bracket is multiplied or divided
					bool multiplicationOrDivision;
					if (i == 0)
						multiplicationOrDivision = false;
					else if (input[i - 1] == "*")
						multiplicationOrDivision = false;
					else if (input[i - 1] == "/")
						multiplicationOrDivision = true;
					else
						throw new Exception("parentheses issue");

					levelList.Add(multiplicationOrDivision);

					output[i] = "";
				}
				else if (input[i] == ")")
				{
					levelList.RemoveAt(levelList.Count - 1);
					output[i] = "";

				}
				// multiplications and divisions are reversed if is Division(levelList)
				else if (input[i] == "*" && isDivision(levelList))
				{
					output[i] = "/";
				}
				else if (input[i] == "/" && isDivision(levelList))
				{
					output[i] = "*";
				}
			}

			return ListCleanup(output);
		}




		private void ParseUnit(List<string> input)
		{
			for (int i = 0; i < input.Count(); i++)
			{
				if (i == 0)
					numerator.Add(input[i]);
				else if (input[i - 1] == "*")
					numerator.Add(input[i]);
				else if (input[i - 1] == "/")
					denominator.Add(input[i]);
			}
		}
	}


	class Program
	{
		static void Main(string[] args)
		{
			new Unit("m/s^3");
			new Unit("V/A*m/(N^2/s)");
			new Unit("V/A*m/(N^2/(s*kg/m))");

			Console.ReadLine();
		}
	}
}
