<Query Kind="Program" />

void Main()
{
	var files = Directory.GetFiles(@"C:\Users\rahulpnath\Documents\Work\Personal\rahulpnath.com\source\_posts").Select(a => Path.GetFileName(a));
	var dateFiles = files.Select(a => new { Title = a, Date = DateTime.Parse(a.Substring(0, 10)) });
	var filesGroupedByYear = dateFiles.GroupBy(a => a.Date.ToString("yyyy MM"));
	var list = filesGroupedByYear.Select(a => new { Year = a.Key, Posts = a.Aggregate(string.Empty, (p, b) => p = p  + b.Title.Remove(0, 11).Replace("-", " ").Replace(".markdown", "") + "\\n"), NumberOfPosts = a.Count() });
	
	var template = "['{0}',{1},'{2}']";
	var output = list.Aggregate(string.Empty, (a,b) => a = a + string.Format(template, b.Year, b.NumberOfPosts, b.Posts) + ",");
	Console.WriteLine(output);
}

// Define other methods and classes here