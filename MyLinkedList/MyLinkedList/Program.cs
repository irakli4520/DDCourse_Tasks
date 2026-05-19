namespace MyLinkedList
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MyLinkedList<string> list = new();
            list.AddFirst("Daniel");
            MyNode<string> myNode = new("Mari");
            list.AddLast(myNode);
            list.AddAfter(myNode, "Irakli");
            list.AddBefore(myNode, "Keti");
            list.Find("Keti");
            bool contains = list.Contains("Keti");
            Console.WriteLine($"List contains \"Keti\" : {contains}");
            bool removed = list.Remove("Daniel");
            Console.WriteLine($"\"Daniel\" removed : {removed}");
            Console.WriteLine();
            foreach (var item in list)
            {
                Console.WriteLine(item);
            }

            // string[] myString = new string[list.Count];
            // list.CopyTo(myString,0);

        }

        public static void PrintList(MyNode<string> head)
        {
            MyNode<string>? current = head;
            while (current != null)
            {
                Console.WriteLine(current);
                current = current.Next;
            }
        }
    }
}