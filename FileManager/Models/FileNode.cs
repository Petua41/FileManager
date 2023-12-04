using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FileManager.Models
{
    public readonly struct FileNode(FileRecord file) : IEquatable<FileNode>
    {
        public FileNode(FileRecord file, IEnumerable<FileNode> subnodes) : this(file)
        {
            SubNodes = new List<FileNode>(subnodes);
        }

        public bool Equals(FileNode other)
        {
            return Equals((object?)other);
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is FileNode node)
            {
                if (node.File.Equals(File))
                {
                    if (SubNodes is not null && node.SubNodes is not null) return SubNodes.SequenceEqual(node.SubNodes);
                    else return SubNodes is null && node.SubNodes is null;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(File, SubNodes);
        }

        public readonly IReadOnlyCollection<FileNode> SubNodes { get; } = [];

        public readonly FileRecord File { get; } = file;

        public static bool operator ==(FileNode left, FileNode right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FileNode left, FileNode right)
        {
            return !(left == right);
        }
    }
}
