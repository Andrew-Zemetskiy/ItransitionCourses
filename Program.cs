using System;
using System.Linq;
using System.Text;
using ConsoleTables;
using System.Security.Cryptography;


namespace task3
{
	class GameException : Exception
	{
		public GameException(string message) : base(message)
		{ }
	}



	class TableGeneration
	{
		public string[] args;
		public TableGeneration(string[] args)
		{
			this.args = args;
		}

		public void CreateTable()
		{
			int step = args.Length / 2;
			string[] fill = new string[args.Length + 1];
			Array.ForEach(fill, a => a = "");

			var table = new ConsoleTable("PC \\ USER");
			foreach (var arg in args)
			{
				string[] column = { arg };
				table.AddColumn(column);
			}

			for (int k = 0; k < args.Length; k++)
			{
				int targetpos = k;
				string[] values = new string[args.Length];

				for (int i = 0; i < values.Length; i++)
				{
					bool pcwinner = true;
					int currentpos = i;

					if (currentpos == targetpos)
					{
						values[i] = "DRAW";
					}
					else
					{
						for (int j = 0; j < step; j++)
						{
							if (currentpos + 1 == args.Length)
							{
								currentpos = 0;
							}
							else
							{
								currentpos++;
							}
							if (currentpos == targetpos)
							{
								pcwinner = false;
							}
						}
						values[i] = pcwinner ? "WIN" : "LOSE";
					}

				}
				string[] row = { args[k] };
				row = row.Concat(values).ToArray();

				table.AddRow(row);

			}

			table.Write(Format.Alternative);
		}
	}



	class GameRules
	{
		public string[] args;
		public int step;
		public GameRules(string[] args)
		{
			this.args = args;
			step = args.Length / 2;
		}
		public void WinnerFinding(int userindex, int systemindex)
		{
			Console.WriteLine($"{args[--userindex]}");

			Console.WriteLine($"Computer move:{args[systemindex]}");

			int position = userindex;
			bool userWon = false;

			if (userindex == systemindex)
			{
				Console.WriteLine("Nobody win!");
			}
			else
			{
				for (int i = 0; i < step; i++)
				{
					if (position + 1 == args.Length)
					{
						position = 0;
					}
					else
					{
						position++;
					}
					if (position == systemindex)
					{
						userWon = true;
					}
				}
				Console.WriteLine("You " + (userWon ? "win!" : "lose!"));
			}

		}
	}



	class HMACKeyGeneration
	{

		public byte[] key;
		public string[] args;


		public HMACKeyGeneration(string[] args)
		{
			this.args = args;
		}

		static string HMACHASH(string str, byte[] key)
		{
			using (var hmac = new HMACSHA256(key))
			{
				byte[] bstr = Encoding.Default.GetBytes(str);
				var bhash = hmac.ComputeHash(bstr);
				return BitConverter.ToString(bhash).Replace("-", string.Empty).ToUpper();
			}
		}

		public void HMACGeneration(int systemindex)
		{
			key = new byte[16];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(key);
			Console.WriteLine("HMAC: " + HMACHASH(args[systemindex], key));
		}
	}


	class Program
	{

		static void Main(string[] args)
		{
			bool exit = false;
			bool uniqueargs = args.Distinct().Count() == args.Length;
			int step = args.Length / 2;

			if (args.Length % 2 == 1 && args.Length >= 3 && uniqueargs)
			{
				while (!exit)
				{
					try
					{
						var rand = new Random();
						int systemindex = rand.Next(0, args.Length);

						HMACKeyGeneration HKG = new HMACKeyGeneration(args);
						HKG.HMACGeneration(systemindex);

						Console.WriteLine("Available moves:");
						for (int i = 0; i < args.Length; i++) { Console.WriteLine($"{i + 1} - {args[i]}"); }
						Console.WriteLine("0 - exit \n? - help");

						Console.Write("Enter your move: "); string user = Console.ReadLine();
						Console.Write("Your move: ");

						if (int.TryParse(user, out int userindex))
						{
							if (userindex == 0) { Console.WriteLine("0"); exit = true; continue; }

							GameRules GR = new GameRules(args);
							GR.WinnerFinding(userindex, systemindex);
							Console.Write("HMAC key: ");
							Console.WriteLine(BitConverter.ToString(HKG.key).Replace("-", string.Empty));

						}
						else if (user == "?")
						{
							Console.WriteLine("help");

							TableGeneration TG = new TableGeneration(args);
							TG.CreateTable();

							Console.WriteLine();
						}
						else
						{
							throw new GameException("incorrect. Pls, select the correct menu item!");
						}

					}
					catch (GameException ex)
					{
						Console.WriteLine(ex.Message);
					}
					catch (IndexOutOfRangeException)
					{
						Console.WriteLine("incorrect. Pls, select the correct menu item!");
					}
					Console.WriteLine($"{new string('-', 20)}\n");
				}
				Console.WriteLine($"\n{new string('-', 20)}\nThanks for the play!");
			}
			else
			{
				if (args.Length % 2 != 1)
				{
					Console.WriteLine("Enter an odd number of arguments");
				}
				if (!(args.Length >= 3))
				{
					Console.WriteLine("Enter more than two arguments");
				}
				if (!uniqueargs)
				{
					Console.WriteLine("There are dublicate arguments");
				}
			}

		}
	}
}
