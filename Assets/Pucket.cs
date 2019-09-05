﻿

public class Pucket {
    public const int Snapshot = 0;
    public const int Input = 1;
    public const int Connection = 2;
       
    public int Topic;
    public bool Ack;
    public long Order;
    public string Data;

    public Pucket(int topic, long order, string data, bool ack) {
        Topic = topic;
        Order = order;
        Data = data;
        Ack = ack;
    }
}