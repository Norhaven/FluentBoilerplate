<Query Kind="Program">
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
</Query>

public class FromType
{	
	public string Text { get; set; }
	
	[MapsTo(typeof(ToType), "FullDescription")]
	public string Description { get; set; }
}

public class ToType
{
	public string Text { get; set; }
	public string FullDescription { get; set; }
}

void Main()
{
	//First off, we need a boilerplate to work with.
	var boilerplate = Boilerplate.New();
	
	//Let's create an instance of a convertible type. 
	var instance = new FromType
	{
		Text = "Some text",
		Description = "Information about this object"
	};
	
	//Now we need to convert the FromType instance to the ToType instance.
	var convertedInstance = boilerplate.Use(instance).As<ToType>();
	
	Console.WriteLine(convertedInstance.Text);
	Console.WriteLine(convertedInstance.FullDescription);
	
	//You may notice that there wasn't any explicit [MapsTo] attribute on the Text property but it converted just fine.
	//By default, the converter will just look for properties with the same name and type.
	//The [MapsTo] attribute is specific to a type and will only override the conversion for that type.
}