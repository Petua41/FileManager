using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FileManager.Models
{
    public readonly struct Node<T>(T value) : IEquatable<Node<T>>
    {
        public Node(T value, IEnumerable<Node<T>> subnodes) : this(value)
        {
            SubNodes = new(subnodes);
        }

        public bool Equals(Node<T> other)
        {
            return Equals((object?)other);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is Node<T> node)
            {
                if (node.Value is null) return Value is null;
                if (node.Value.Equals(Value))
                {
                    if (SubNodes is not null && node.SubNodes is not null) return SubNodes.SequenceEqual(node.SubNodes);
                    else return SubNodes is null && node.SubNodes is null;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value, SubNodes);
        }

        public readonly ObservableCollection<Node<T>> SubNodes { get; } = [];

        public readonly T Value { get; } = value;

        public static bool operator ==(Node<T> left, Node<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Node<T> left, Node<T> right)
        {
            return !(left == right);
        }
    }
}
