using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data;

    //Property
    public int Count
    {
        get
        {
            return data.Count;
        }
    }

    //Constructor
    public PriorityQueue()
    {
        this.data = new List<T>();
    }

    //Methods
    public void Enqueue(T item)
    {
        data.Add(item);

        T temp = item;
        int index = data.Count - 1;

        while (index > 0 && (temp.CompareTo(data[(index - 1) / 2]) >= 0))
        {
            data[index] = data[(index - 1) / 2];
            index = (index - 1) / 2;
        }

        data[index] = temp;
    }
    public T Dequeue()
    {
        int size = data.Count - 1;
        int index = 0;
        int child = (index * 2) + 1;

        T temp = data[0];
        data[0] = data[size];
        data.RemoveAt(size);

        while (child < size - 1)
        {
            //if left child has bigger distance than right
            if (data[child].CompareTo(data[child + 1]) < 0)
                child++;
            //if parent has bigger distance than child
            if (data[index].CompareTo(data[child]) < 0)
            {
                T tempTwo = data[child];
                data[child] = data[index];
                data[index] = tempTwo;

                index = child;
                child = (index * 2) + 1;
            }
            else
                break;
        }

        return temp;
    }
    public T Peek()
    {
        return data[0];
    }
    public bool Contains(T item)
    {
        return data.Contains(item);
    }
    public List<T> ToList()
    {
        return data;
    }
}
