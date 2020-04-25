using System;
using System.Collections.Generic;

namespace Antijank {

  public static class LinkedListExtensions {

    public static LinkedListNode<T> Find<T>(this LinkedList<T> list, Func<LinkedListNode<T>, bool> predicate, bool fromEnd = false)
      => fromEnd
        ? list.Last.FindPrevious(predicate)
        : list.First.FindNext(predicate);

    public static LinkedListNode<T> FindNext<T>(this LinkedListNode<T> start, Func<LinkedListNode<T>, bool> predicate) {
      var node = start;
      if (node == null)
        return null;

      do {
        if (predicate(node))
          return node;

        node = node.Next;
      } while (node != null);

      return null;
    }

    public static LinkedListNode<T> FindPrevious<T>(this LinkedListNode<T> start, Func<LinkedListNode<T>, bool> predicate) {
      var node = start;
      if (node == null)
        return null;

      do {
        if (predicate(node))
          return node;

        node = node.Previous;
      } while (node != null);

      return null;
    }

    public static LinkedListNode<T> Find<T>(this LinkedList<T> list, Func<T, bool> predicate, bool fromEnd = false)
      => fromEnd
        ? list.Last.FindPrevious(predicate)
        : list.First.FindNext(predicate);

    public static LinkedListNode<T> FindNext<T>(this LinkedListNode<T> start, Func<T, bool> predicate)
      => start.FindNext(node => predicate(node.Value));

    public static LinkedListNode<T> FindPrevious<T>(this LinkedListNode<T> start, Func<T, bool> predicate)
      => start.FindPrevious(node => predicate(node.Value));

    public static LinkedListNode<T> MoveBefore<T>(this LinkedListNode<T> mover, LinkedListNode<T> target) {
      mover.List.Remove(mover);
      var list = target.List;
      list.AddBefore(target, mover);
      return mover;
    }

    public static LinkedListNode<T> MoveAfter<T>(this LinkedListNode<T> mover, LinkedListNode<T> target) {
      mover.List.Remove(mover);
      var list = target.List;
      list.AddAfter(target, mover);
      return mover;
    }

    public static LinkedListNode<T> Navigate<T>(this LinkedListNode<T> mover, int count, bool stopAtEdge = false) {
      if (count > 0) {
        var node = mover;
        while (count > 0 && node.Next != null) {
          node = node.Next;
          --count;
        }

        return count != 0
          ? stopAtEdge
            ? mover.List.Last
            : null
          : node;
      }
      else {
        var node = mover;
        while (count < 0 && node.Previous != null) {
          node = node.Previous;
          ++count;
        }

        return count != 0
          ? stopAtEdge
            ? mover.List.First
            : null
          : node;
      }
    }

  }

}