using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System;
using UnityEngine;

public class Stream {
	private Dictionary<int, List<Action<string, long>>> _observers;
	private Dictionary<int, List<Action<long, long>>> _ackObservers;
	private List<Packet> _packets;
	// private List<Packet> _acks;
	private bool _reliability;
	private bool client;

	public Stream(bool reliability, bool client) {
		this.client = client;
		_packets = new List<Packet>();
		// _acks = new List<Packet>();
		_observers = new Dictionary<int, List<Action<string, long>>>();
		_ackObservers = new Dictionary<int, List<Action<long, long>>>();
		_reliability = reliability;
	}
	
	public void ReceivePacket(Packet p) {
		if (p.Ack) {
			Debug.Log("Ack received, orderSent: " + p.Order + ", frameACK:" + p.Data);
			_packets = _packets.Where(pq => pq.Order > long.Parse(p.Data)).ToList();
			if(!_ackObservers.ContainsKey(p.Topic)){
				_ackObservers[p.Topic] = new List<Action<long, long>>();
			}
			_ackObservers[p.Topic].ForEach(obs => obs(long.Parse(p.Data), p.Order));
		} else {
			if(!_observers.ContainsKey(p.Topic)){
				_observers[p.Topic] = new List<Action<string, long>>();
			}
			_observers[p.Topic].ForEach(obs => obs(p.Data, p.Order));
		}
	}

	public Packet CreatePacket(string data, int topic, bool ack=false) {
		Packet q = new Packet(topic, client ? Client.Frame: Server.Frame, data, ack);
		// if (ack) {
		// 	_acks.Add(q);
		// } else {
		// 	_packets.Add(q);
		// }
		if(!ack){
			_packets.Add(q);
		}

		return q;
	}

	public void addObserver(Action<string, long> obs, int topic) {
		if(!_observers.ContainsKey(topic)){
			_observers[topic] = new List<Action<string, long>>();
		}
		_observers[topic].Add(obs);
	}

	public void addAckObserver(Action<long, long> obs, int topic) {
		if(!_ackObservers.ContainsKey(topic)){
			_ackObservers[topic] = new List<Action<long, long>>();
		}
		_ackObservers[topic].Add(obs);
		Debug.Log("ADDING ACK OBSERVER");
	}

	// public List<Packet> GetBufferElems() {
	// 	List<Packet> aux = _acks.Concat(_packets).ToList();
	// 	_acks.Clear();
	// 	return aux;
	// }
}
