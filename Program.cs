using LW5_MathProject.Services.Services;

var service = new MathService();

Console.WriteLine(service.Power(2, 10));
Console.WriteLine(service.Sqrt(144));
Console.WriteLine(service.Average(new[] { 1.0, 2.0, 3.0, 4.0 }));
Console.WriteLine(service.IsPerfectSquare(49));

Console.WriteLine("--- History ---");
foreach (var entry in service.History)
    Console.WriteLine(entry);
