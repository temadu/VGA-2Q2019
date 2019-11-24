using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System;
using UnityEngine;

public class Bafer {
    public class Pocket{
        public long horder;
        public string possisions;

        public Pocket(long o, string p) {
            this.horder = o;
            this.possisions = p;
        }
    }
    private SortedList<long, Pocket> p;
    public int size;

    public Bafer(){
        this.p = new SortedList<long, Pocket>();
    }

    public void add(long order, string message) {
        p.Add(order, new Pocket(order, message));
    }

    public Pocket peak(){
        return p.Values[0];
    }

    public Pocket peakEnd(){
        return p.Values[p.Count - 1];
    }

    public void removeFirst(){
        p.RemoveAt(0);
    }


}