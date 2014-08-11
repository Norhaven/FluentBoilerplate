<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.ServiceModel.dll</Reference>
  <NuGetReference>FluentBoilerplate</NuGetReference>
  <Namespace>FluentBoilerplate</Namespace>
  <Namespace>FluentBoilerplate.Providers</Namespace>
  <Namespace>FluentBoilerplate.Providers.Database</Namespace>
</Query>

void Main()
{
	//You may also want to query a database. There is currently support for executing stored procedures
	//through an ADO.Net type provider, if you so choose, or you could write your own database provider
	//for use with other things (e.g. Entity Framework).
	
	//Let's see how you might use the ADO.Net type provider to connect to a SQL database.
	//This sample will fail because there is no app.config with the named connection string,
	//and no stored procedures by those names in the database that you don't have, 
	//but it serves as an example of how you might go about doing this (should you have all of those things).
	
	var adoProvider = new AdoNetConnectionProvider("Your Connection String Name", DataSource.SQL);
	var accessProvider = new TypeAccessProvider(adoProvider);
	var boilerplate = Boilerplate.New(accessProvider: accessProvider);
	
	boilerplate
		.Open<IAdoNetConnection>()
		.AndDo((context, connection) =>
		{
			var username = connection.CreateParameter("userName", "Bob");
			connection.ExecuteStoredProcedure("SubscribeUserToObviouslyMeaningfulEmails", username);
		});
		
	//You're also welcome to get a sequence of typed objects back from a stored procedure call.
	UseStoredProcedureWithResult(boilerplate);
}

//Presumably, you would like to interpret a row of data as a typed object.
//You can do this fairly easily by implementing FluentBoilerplate.Providers.Database.IDataInterpreter<TResult>.

//Let's say you have a Person class that represents a distinct person in your database.
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

//And you'd like to interpret data as that type.
//Let's implement an interpreter for it!
public class PersonInterpreter:IDataInterpreter<Person>
{
    public IEnumerable<Person> Interpret(IDataReader dataReader)
    {
        while(dataReader.Read())
        {
            var id = dataReader.GetInt32(0);
            var name = dataReader.GetString(1);
            var description = dataReader.GetString(2);
            yield return new Person { Id = id, Name = name, Description = description };
        }
    }
}

private static void UseStoredProcedureWithResult(IBoilerplateContext boilerplate)
{
	//Once those types are defined, it's really not that much different than the previous attempt.
	boilerplate
		.Open<IAdoNetConnection>()
		.AndDo((context, connection) =>
		{
			var reason = connection.CreateParameter("reason", "Just because");
			var people = connection.ExecuteStoredProcedure("GetEveryUserInOurDatabase", new PersonInterpreter(), reason);
			
			foreach(var person in people)
			{
				Console.WriteLine(person.Name);
			}
		});
}