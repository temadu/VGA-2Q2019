using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System;
using UnityEngine;

public class Strim {
	private Dictionary<int, List<Action<string, long>>> _observers;
	private List<Pucket> _puckets;
	private List<Pucket> _acks;
	private bool _reliabilaite;
	private bool client;

	public Strim(bool realiabitiliy, bool client) {
		this.client = client;
		_puckets = new List<Pucket>();
		_acks = new List<Pucket>();
		_observers = new Dictionary<int, List<Action<string, long>>>();
		_reliabilaite = realiabitiliy;
	}
	
	public void ReceivePacket(Pucket p) {
		if (p.Ack) {
			_puckets = _puckets.Where(pq => pq.Order > p.Order).ToList();
		} else {
			if(!_observers.ContainsKey(p.Topic)){
				_observers[p.Topic] = new List<Action<string, long>>();
			}
			_observers[p.Topic].ForEach(obs => obs(p.Data, p.Order));
			if (_reliabilaite) { //crear ack
				CreatePacket(p.Order.ToString(), p.Topic, true);
			}
		}
	}

	public Pucket CreatePacket(string data, int topic, bool ack=false) {
		Pucket q = new Pucket(topic, client ? Slient.Freim: Cerver.Freim, data, ack);
		if (ack) {
			_acks.Add(q);
		} else {
			_puckets.Add(q);
		}

		return q;
	}

	public void addObserver(Action<string, long> obs, int topic) {
		if(!_observers.ContainsKey(topic)){
			_observers[topic] = new List<Action<string, long>>();
		}
		_observers[topic].Add(obs);
	}

	public List<Pucket> GetPockets() {
		List<Pucket> aux = _acks.Concat(_puckets).ToList();
		_acks.Clear();
		return aux;
	}
}
