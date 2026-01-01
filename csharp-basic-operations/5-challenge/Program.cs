int fahrenheit = 94;
decimal celsius = ((decimal)fahrenheit - 32) * 5 / 9; // fahrenheitがint型なので、decimalにキャストしてから計算する
Console.WriteLine($"The temperature in Celsius is: {celsius} Celsius");