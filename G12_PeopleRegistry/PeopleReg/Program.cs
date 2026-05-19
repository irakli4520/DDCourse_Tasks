using System.Collections;

namespace PeopleReg;

internal class Program
{
    static void Main()
    {
        Person p1 = new Person(1, "p1", "p1", new DateTime(1990, 1, 1));
        Person p2 = new Person(2, "p2", "p2", new DateTime(1990, 1, 1));
        Person p3 = new Person(3, "p3", "p3", new DateTime(1990, 1, 1));

        Person p1_1 = new Person(4, "p1_1", "p1_1", new DateTime(1990, 1, 1));
        Person p1_2 = new Person(5, "p1_2", "p1_2", new DateTime(1990, 1, 1));
        Person p1_3 = new Person(6, "p1_3", "p1_3", new DateTime(1990, 1, 1));
        
        Person p2_1 = new Person(7, "p2_1", "p2_1", new DateTime(1990, 1, 1));
        Person p2_2 = new Person(8, "p2_2", "p2_2", new DateTime(1990, 1, 1));
        Person p2_3 = new Person(9, "p2_3", "p2_3", new DateTime(1990, 1, 1));
        
        Person p3_1 = new Person(10, "p3_1", "p3_1", new DateTime(1990, 1, 1));
        Person p3_2 = new Person(11, "p3_2", "p3_2", new DateTime(1990, 1, 1));
        Person p3_3 = new Person(12, "p3_3", "p3_3", new DateTime(1990, 1, 1));
        
        Person p1_1_1 = new Person(13, "p1_1_1", "p1_1_1", new DateTime(1990, 1, 1));
        Person p1_1_2 = new Person(14, "p1_1_2", "p1_1_2", new DateTime(1990, 1, 1));
        Person p1_1_3 = new Person(15, "p1_1_3", "p1_1_3", new DateTime(1990, 1, 1));
        
        Person p1_1_1_1 = new Person(16, "p1_1_1_1", "p1_1_1_1", new DateTime(1990, 1, 1));
        
        p1.Children.Add(p1_1);
        p1.Children.Add(p1_2);
        p1.Children.Add(p1_3);
        
        p1_1.Children.Add(p1_1_1);
        p1_1.Children.Add(p1_1_2);
        p1_1.Children.Add(p1_1_3);
        
        p1_1_1.Children.Add(p1_1_1_1);
        
        p2.Children.Add(p2_1);
        p2.Children.Add(p2_2);
        p2.Children.Add(p2_3);
        
        p3.Children.Add(p3_1);
        p3.Children.Add(p3_2);
        p3.Children.Add(p3_3);

        PersonList persons = new PersonList();
        persons.Add(p1);
        persons.Add(p2);
        persons.Add(p3);
        
        // foreach (var person in persons)
        // {
        //     Console.WriteLine(person);
        // }
        //
        // persons.Save(@"/Users/irakli/Downloads/Test.txt");

       // PersonList persons = new PersonList();
        persons.Save(@"/Users/Documents/persons/text.txt");
        
        foreach (var person in persons)
        {
            Console.WriteLine(person);
        }

        // PersonList.WriteToFile(@"/Users/irakli/Downloads/Test.txt",persons);

        // Print(persons);

        // PersonList.WriteToFile(@"/Users/irakli/Downloads/Test.txt",persons);
        // string personas = PersonList.ReadFromFile(@"/Users/irakli/Downloads/Test.txt");
        // Console.WriteLine(personas);
        // persons[0] = new Person(16, "p1_1_1", "p1_1_1", new DateTime(1990, 1, 1));

        // PersonList people = new PersonList();
        // people.Add(new Person(1, "John", "Doe", new DateTime(1990, 1, 1)));
        // people.Add(new Person(2, "Jane", "Smith", new DateTime(1995, 5, 15)));
        // people.Add(new Person(3, "Alice", "Johnson", new DateTime(1985, 10, 20)));
        // (people as IList<Person>).Add(new Person(5, "Charlie", "Brown", new DateTime(2000, 12, 31)));
        //
        // Person newPerson = new Person(4, "Bob", "Brown", new DateTime(1988, 3, 30));
        // people[0] = newPerson;
        //
        // PersonList.PrintCollection(people);

        //List<Person> peopleForAddRange = new List<Person>();
        //peopleForAddRange.Add(new Person(4, "QQ", "QQ", new DateTime(1990, 1, 1)));
        //peopleForAddRange.Add(new Person(5, "WW", "WW", new DateTime(1995, 5, 15)));

        //people.AddRange(peopleForAddRange);

        //PersonList.PrintCollection(people);

        //people.Insert(0, new Person(6, "EE", "EE", new DateTime(1985, 10, 20)));

        //PersonList.PrintCollection(people);

        //List<Person> peopleForInsertRange = new List<Person>();
        //peopleForInsertRange.Add(new Person(7, "RR", "RR", new DateTime(1985, 10, 20)));
        //peopleForInsertRange.Add(new Person(8, "TT", "TT", new DateTime(1985, 10, 20)));

        //people.InsertRange(3, peopleForInsertRange);

        //PersonList.PrintCollection(people);

        // Console.WriteLine(people.Count);
    }

    public static void Print(IEnumerable<Person> persons, int level = 0)
    {
        foreach (var person in persons)
        {
            Console.WriteLine($"{(new String(' ', 2 * level))}{person}");
            Print(person.Children, level + 1);
        }
    }
}

class Person
{
    private string _firstName;
    private string _lastName;

    public Person(int id, string firstName, string lastName, DateTime birthDate)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Id must be greater than zero.");
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
    }

    public int Id { get; }

    public string FirstName
    {
        get => _firstName;
        set => _firstName = value ?? throw new ArgumentNullException(nameof(value), "First name cannot be null.");
    }

    public string LastName
    {
        get => _lastName;
        set => _lastName = value ?? throw new ArgumentNullException(nameof(value), "Last name cannot be null.");
    }

    public DateTime BirthDate { get; set; }

    public ICollection<Person> Children { get; set; } = new List<Person>();

    public override string ToString()
    {
        return $"{Id}: {FirstName} {LastName}, Born on {BirthDate.ToShortDateString()}";
    }
}

class PersonList : IList<Person>
{
    private readonly List<Person> _list = new();

    private readonly HashSet<int> _ids = new();

    public void Add(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);
        ValidateAndAddIds(person);
        _list.Add(person);
    }

    public void Clear()
    {
        _list.Clear();
        _ids.Clear();
    }

    public bool Contains(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);
        return _list.Contains(person);
    }

    public void CopyTo(Person[] array, int arrayIndex)
    {
        ArgumentNullException.ThrowIfNull(array);
        _list.CopyTo(array, arrayIndex);
    }

    public bool Remove(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        if (_list.Remove(person))
        {
            RemoveIds(person);
            return true;
        }

        return false;
    }

    public int Count => _list.Count;
    bool ICollection<Person>.IsReadOnly => false;

    public void AddRange(IEnumerable<Person> persons)
    {
        ArgumentNullException.ThrowIfNull(persons);
        foreach (var person in persons)
        {
            ValidateAndAddIds(person);
        }

        _list.AddRange(persons);
    }


    public int IndexOf(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);
        return _list.IndexOf(person);
    }

    public int IndexOf(Person person, int startIndex)
    {
        ArgumentNullException.ThrowIfNull(person);
        return _list.IndexOf(person, startIndex);
    }

    public void Insert(int index, Person person)
    {
        ArgumentNullException.ThrowIfNull(person);
        ValidateAndAddIds(person);
        _list.Insert(index, person);
    }

    public void RemoveAt(int index)
    {
        RemoveIds(_list[index]);
        _list.RemoveAt(index);
    }

    public void InsertRange(int index, IEnumerable<Person> persons)
    {
        ArgumentNullException.ThrowIfNull(persons);
        foreach (var person in persons)
        {
            ValidateAndAddIds(person);
        }

        _list.InsertRange(index, persons);
    }

    public Person this[int index]
    {
        get => _list[index];
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            RemoveIds(_list[index]);
            ValidateAndAddIds(value);
            _list[index] = value;
        }
    }

    private void ValidateAndAddIds(Person person)
    {
        var newIds = new HashSet<int>();
        CollectIds(person, newIds);
        CheckAndAddToGlobalIds(newIds);
    }

    private static void CollectIds(Person person, HashSet<int> ids)
    {
        if (!ids.Add(person.Id))
            throw new InvalidOperationException($"Duplicate ID in input: {person.Id}");

        foreach (var child in person.Children)
        {
            CollectIds(child, ids);
        }
    }

    private void CheckAndAddToGlobalIds(HashSet<int> newIds)
    {
        foreach (var id in newIds)
        {
            if (!_ids.Add(id))
                throw new InvalidOperationException($"This Id - {id} is already taken");
        }
    }

    private void RemoveIds(Person person)
    {
        _ids.Remove(person.Id);
        foreach (var child in person.Children)
        {
            RemoveIds(child);
        }
    }
    
    public void Save(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        using FileStream stream = new FileStream(filePath, FileMode.Create);
        Save(stream);
    }

    public void Save(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanWrite) throw new InvalidOperationException("Stream is not writable");

        using StreamWriter writer = new StreamWriter(stream, leaveOpen: true);

        foreach (var person in this)
        {
            Save(writer, person);
        }
    }

    private void Save(StreamWriter writer, Person person)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(person);
        writer.WriteLine($"{person.Id}\t{person.FirstName}\t{person.LastName}\t{person.BirthDate}");
        foreach (var child in person.Children)
        {
            Save(writer, child);
            
        }
    }

    public void Load(string filePath)
    {
        ArgumentNullException.ThrowIfNull(filePath);
        if (!File.Exists(filePath)) throw new FileNotFoundException(filePath);

        FileStream stream = new FileStream(filePath, FileMode.Open);
        Load(stream);
    }

    public void Load(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        if (!stream.CanRead) throw new InvalidOperationException("Stream is not readable");
        
        Clear();
        using StreamReader reader = new StreamReader(stream,leaveOpen: true);
        
        while (!reader.EndOfStream)
        {
            string line = reader.ReadLine()!;
            string[] parts = line.Split('\t');
            Person person = new Person(
                int.Parse(parts[0]), 
                parts[1], 
                parts[2],
                DateTime.Parse(parts[3]));
            Add(person);
        }
    }

    public IEnumerator<Person> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}