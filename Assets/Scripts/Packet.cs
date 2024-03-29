﻿

public class Packet {
    public const int Snapshot = 0;
    public const int Input = 1;
    public const int Connection = 2;
    public const int UpdatePlayersInfo = 3;
    public const int Login = 4;
    public const int Logined = 5;
    public const int Logut = 6;

       
    public int Topic;
    public bool Ack;
    public long Order;
    public string Data;

    public Packet(int topic, long order, string data, bool ack) {
        Topic = topic;
        Order = order;
        Data = data;
        Ack = ack;
    }

//    topic, ack, order, data
    public override string ToString() {
        return Topic + "," + Ack + "," + Order + "," + Data;
    }
}