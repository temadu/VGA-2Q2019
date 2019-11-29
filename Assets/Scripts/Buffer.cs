using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System;
using UnityEngine;

public class Buffer {
    public class BufferElem{
        public long order;
        public string positions;

        public BufferElem(long o, string p) {
            this.order = o;
            this.positions = p;
        }
    }
    private SortedList<long, BufferElem> p;
    public int size;

    public Buffer(){
        this.p = new SortedList<long, BufferElem>();
    }

    public void add(long order, string message) {
        p.Add(order, new BufferElem(order, message));
    }

    public BufferElem peak(){
        if(p.Count != 0) {
            return p.Values[0];
        }
        return null;
    }

    public BufferElem peakEnd(){
        if(p.Count == 0) {
            return null;
        }
        return p.Values[p.Count - 1];
    }

    public void removeFirst(){
        if(p.Count != 0) {
            p.RemoveAt(0);
        }
    }


}