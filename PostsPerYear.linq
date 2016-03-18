<Query Kind="Program" />

void Main()
{
	var files = Directory.GetFiles(@"C:\Users\rahulpnath\Documents\Work\Personal\rahulpnath.com\source\_posts").Select(a => Path.GetFileName(a));
	var filesGroupedByYear = files.GroupBy(a => a.Substring(0, 4));
	Console.WriteLine(filesGroupedByYear.Select(a => new { Year = a.Key, NumberOfPosts = a.Count()}));
}

// Define other methods and classes here
