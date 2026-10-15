<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Namespace>System.Web</Namespace>
  <Namespace>System.Drawing</Namespace>
</Query>

void Main()
{
	var gallery = "Family";
	(from file in Directory.EnumerateFiles($@"C:\Code\Miniblog.Core\src\wwwroot\img\{gallery}\")
	let dimensions = GetDimensions(file)
	let ratio = GetFactoredDimensions(dimensions)
	 let fileName = Path.GetFileName(file)
	 where ratio.height == 83 || ratio.width == 83
	 let className = ratio.height == 83 ? "image__wide" : "image__tall"
	 select $@"<img src='~/img/{gallery}/{fileName}' class='{className}' height='{dimensions.height}' width='{dimensions.width}' />"
	 )
	.Dump();
}

public (int height, int width) GetDimensions(string filePath)
{
	using (var image = Image.FromFile(filePath)) {
		return (image.Height, image.Width);
	}
}

public (int height, int width) GetFactoredDimensions((int height, int width) input)
{
	var heightFactors = FindFactors(input.height).ToList();
	var widthFactors = FindFactors(input.width).ToList();
	foreach (var factor in heightFactors.ToArray())
	{
		if (widthFactors.Contains(factor)) {
			heightFactors.Remove(factor);
			widthFactors.Remove(factor);
		}
	}

	var factoredHeight = heightFactors.Any() ? heightFactors.Aggregate((a, b) => a * b) : 1;
	var factoredWidth = widthFactors.Any() ? widthFactors.Aggregate((a, b) => a * b) : 1;
	return (factoredHeight, factoredWidth);
}

private IEnumerable<int> FindFactors(int num)
{
	while (num % 2 == 0)
	{
		yield return 2;
		num /= 2;
	}

	// Take out other primes.
	int factor = 3;
	while (factor * factor <= num)
	{
		if (num % factor == 0)
		{
			// This is a factor.
			yield return factor;
			num /= factor;
		}
		else
		{
			// Go to the next odd number.
			factor += 2;
		}
	}

	// If num is not 1, then whatever is left is prime.
	if (num > 1) yield return num;
}