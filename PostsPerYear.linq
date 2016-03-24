<Query Kind="Program" />

void Main()
{
	var files = Directory.GetFiles(@"C:\Users\rahulpnath\Documents\Work\Personal\rahulpnath.com\source\_posts").Select(a => Path.GetFileName(a));
	var dateFiles = files.Select(a => new { Title = a, Date = DateTime.Parse(a.Substring(0, 10))});
	var filesGroupedByYear = dateFiles.GroupBy(a =>  a.Date.ToString("yyyy MM"));
	Console.WriteLine(filesGroupedByYear.Select(a => new { Year = a.Key, NumberOfPosts = a.Count()}));
}

// Define other methods and classes here